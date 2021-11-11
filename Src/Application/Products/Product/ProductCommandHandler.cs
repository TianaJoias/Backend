using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Products.Repositories;
using Application.Specification;
using Domain.Products.Write;
using FluentResults;
using FluentValidation;

namespace Application.Products.Commands
{
    public class ProductCommandHandler : ICommandHandler<CreateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoriesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductCommandHandler(IProductRepository productRepository,
            ICategoryRepository categoriesRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _categoriesRepository = categoriesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Title, request.HtmlBody);
            var byIds = SpecificationBuilder<Category>.Where(it => request.Categories.Contains(it.Id)).Build();
            var categorias = await _categoriesRepository.Filter(byIds);
            product.AddCategories(categorias.ToArray());
            await _productRepository.Add(product);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }

    public class ProductQueryHandler : IQueryPagedHandler<ProductQuery, ProductQueryResult>
    {
        private readonly IProductRepository _productRepository;

        public ProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Result<PagedList<ProductQueryResult>>> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            var builder = SpecificationBuilder<Product>.Where(it => it.Title.Contains(request.SearchTerm));
            if (String.IsNullOrWhiteSpace(request.SearchTerm))
                builder = SpecificationBuilder<Product>.All();

            var bySearchTerm = builder.Paged(request)
                .Build();
            var products = await _productRepository.FilterPaged(bySearchTerm);
            return Result.Ok(products.To<ProductQueryResult>());

        }
    }


    public class ProductQueryResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string HtmlBody { get; set; }
    }
    
    public class ProductQueryValidator : QueryPagedValidator<ProductQuery>
    {
        
    }

    public class QueryPagedValidator<T> : AbstractValidator<T> where T : QueryPaged
    {
        public QueryPagedValidator()
        {
            RuleFor(c => c.PageNumber).GreaterThan(0);
            RuleFor(c => c.PageSize).GreaterThan(0);
        }
    }
}
