using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using Domain.Portifolio;
using Domain.Stock;
using FluentResults;
using static Domain.Catalog.Catalog;

namespace WebApi.Aplication.Catalog
{
    public class CatalogAddPackageCommandHandler : ICommandHandler<CatalogAddPackageCommand>
    {
        private readonly IProductRepository _product;
        private readonly ILotRepository _lotRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IAgentRepository _agentRepository;

        public CatalogAddPackageCommandHandler(IProductRepository product, ILotRepository lotRepository, IUnitOfWork unitOfWork, ICatalogRepository catalogRepository, IAgentRepository channelRepository)
        {
            _product = product;
            _lotRepository = lotRepository;
            _unitOfWork = unitOfWork;
            _catalogRepository = catalogRepository;
            _agentRepository = channelRepository;
        }

        public async Task<Result> Handle(CatalogAddPackageCommand request, CancellationToken cancellationToken)
        {
            var (agent, products, lots, catalog) = await GetData(request);
            var newCatalog = catalog is null;
            catalog ??= new Domain.Catalog.Catalog(agent);
            var errors = new List<Result>();
            if (agent is null)
                errors.Add(Result.Fail("AGENT_REQUIRED"));
            if (errors.Any())
                return Result.Merge(errors.ToArray());
            foreach (var item in request.Items)
            {
                var lot = lots.First(it => it.Id == item.LotId);
                var produt = products.First(it => it.Id == lot.ProductId);
                catalog.AddItem(produt, lot, item.Quantity);
            }
            if (request.CompletePreparing)
                catalog.Next();
            if (newCatalog)
                await _catalogRepository.Add(catalog);
            else
                await _catalogRepository.Update(catalog);
            await _agentRepository.Update(agent);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
        private async Task<(Agent agent, IList<Product> products, IList<Lot> lots, Domain.Catalog.Catalog catalog)> GetData(CatalogAddPackageCommand request)
        {
            var lotsIds = request.Items.Select(it => it.LotId).ToList();
            var lotsTask = _lotRepository.List(it => lotsIds.Contains(it.Id));
            var agentTask = _agentRepository.GetByQuery(it => it.Id == request.AgentId && it.AccountableId == request.AccountableId);
            var catalogTask = _catalogRepository.GetByQuery(it => it.Agent.AccountableId == request.AccountableId && it.Agent.Id == request.AgentId && it.State == States.Preparation);
            await Task.WhenAll(lotsTask, agentTask, catalogTask);
            var lots = await lotsTask;
            var productsIds = lots.Select(it => it.ProductId).ToList();
            var productsTask = _product.List(it => productsIds.Contains(it.Id));
            return (agent: await agentTask, products: await productsTask, lots, catalog: await catalogTask);
        }
    }

    public record CatalogAddPackageCommand(Guid AgentId, Guid AccountableId, IList<CatalogAddPackageItemCommand> Items, bool CompletePreparing) : ICommand;

    public record CatalogAddPackageItemCommand
    {
        public Guid LotId { get; init; }
        public decimal Quantity { get; init; }
    }
}
