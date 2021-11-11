using Microsoft.EntityFrameworkCore;
using Infra.Products.Mappers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq.Expressions;
using Application;
using Domain.Products.Write;
using MediatR;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infra.Products
{
    internal static class InitialDBHelper
    {
        internal static string GetMemberNameFor<T, TProperty>(Expression<Func<T, TProperty>> expression) where T : class
        {
            if (expression.Body is not MemberExpression memberExpression)
                throw new Exception(string.Format("Property missing in '{0}'", expression), new ArgumentException("Argument should be a MemberExpression", "expression"));

            if (memberExpression.Expression.ToString().Contains("."))
                throw new Exception(string.Format("Nested property {0} not allowed", expression), new ArgumentException("Argument should be a direct property of the object being constructed", "expression"));

            return memberExpression.Member.Name;
        }

        internal static void SetPrivatePropertyValue<T, TProperty>(this T dest, Expression<Func<T, TProperty>> expression, TProperty newValue) where T : class
        {
            var propertyName = GetMemberNameFor(expression);

            var property = dest.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            property.SetValue(dest, newValue, null);
        }
    }

    /// <summary>
    /// https://blog.tekspace.io/code-first-multiple-db-context-migration/
    /// </summary>
    public sealed class ProductContextDB : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductContextDB> _logger;

        public ProductContextDB(DbContextOptions<ProductContextDB> options, IMediator mediator, ILogger<ProductContextDB> logger) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public DbSet<Product> Products { get; set; }

        public bool HasTransaction => _currentTransaction != null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductMapper).Assembly);
        }

        public async Task Seeding()
        {

            var logger = this.GetInfrastructure().GetRequiredService<ILoggerFactory>();
            try
            {
                Database.Migrate();
                var guid = Guid.Parse("{0963682F-4DBC-4827-B500-B7F45A6345C3}");
                if (!await Set<Product>().AnyAsync(b => b.Id == guid))
                {
                    var category = new Category("My Category");
                    var product = new Product("Primeiro", "BODY");
                    product.AddCategories(category);
                    product.SetPrivatePropertyValue(it => it.Id, guid);
                    Add(product);
                    await Commit();
                }
            }
            catch (Exception ex)
            {
                logger.CreateLogger<ProductContextDB>().LogError(ex, "Entity Framework migration error");
            }
        }

        public async Task<bool> Commit(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this, cancellationToken);
            return await SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(cancellationToken);
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            return _currentTransaction;
        }

        private IDbContextTransaction _currentTransaction;
        public Task Transaction(Func<Task> act, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is not null)
                throw new DBConcurrencyException("Transaction already exists.");
            var strategy = Database.CreateExecutionStrategy();
            return strategy.ExecuteAsync(async (ct) =>
            {
                using var transaction = await BeginTransactionAsync(ct);
                await act();
                await CommitTransactionAsync(transaction, ct);
            }, cancellationToken);
        }
    }
}
