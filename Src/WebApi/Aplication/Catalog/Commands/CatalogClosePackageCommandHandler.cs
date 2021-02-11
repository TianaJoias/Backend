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

namespace WebApi.Aplication.Catalog.Commands
{
    public class CatalogClosePackageCommandHandler : ICommandHandler<CatalogClosePackageCommand>,
        ICommandHandler<CatalogNextStatusCommand>,
        ICommandHandler<CatalogTransferItemsCommand>

    {
        private readonly IProductRepository _productRespository;
        private readonly ILotRepository _lotRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CatalogClosePackageCommandHandler(IProductRepository productRespository, ILotRepository lotRepository, ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
        {
            _productRespository = productRespository;
            _lotRepository = lotRepository;
            _catalogRepository = catalogRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CatalogClosePackageCommand request, CancellationToken cancellationToken)
        {
            var catalog = await _catalogRepository.GetByQuery(it => it.Agent.Id == request.OwnerId && it.Id == request.CatalogId);
            if (catalog is null)
                return Result.Fail("CATALOG_NOT_FOUND");
            catalog.Next();
            foreach (var item in request.Items)
                catalog.ReturnItem(item.LotId, item.Quantity);
            if (request.Done)
                catalog.Close();
            await _catalogRepository.Update(catalog);
            await _unitOfWork.Commit();
            return Result.Ok();
        }

        public async Task<Result> Handle(CatalogNextStatusCommand request, CancellationToken cancellationToken)
        {
            var catalog = await _catalogRepository.GetByQuery(it => it.Agent.Id == request.AgentId && it.Id == request.CatalogId);
            if (catalog is null)
                return Result.Fail("CATALOG_NOT_FOUND");
            catalog.Next();
            await _catalogRepository.Update(catalog);
            await _unitOfWork.Commit();
            return Result.Ok();
        }

        public async Task<Result> Handle(CatalogTransferItemsCommand request, CancellationToken cancellationToken)
        {
            var fromCatalogTask = _catalogRepository.GetByQuery(it => it.Agent.Id == request.AgentId && it.Id == request.FromCatalogId);
            var toCatalogTask = _catalogRepository.GetByQuery(it => it.Agent.Id == request.AgentId && it.Id == request.ToCatalogId);
            var lotsIds = request.Items.Select(it => it.LotId);
            var lotsTask = _lotRepository.List(it => lotsIds.Contains(it.Id));
            await Task.WhenAll(fromCatalogTask, toCatalogTask, lotsTask);
            var lots = await lotsTask;
            var productsIds = lots.Select(it => it.ProductId);
            var products = await _productRespository.List(it => productsIds.Contains(it.Id));
            var fromCatalog = await fromCatalogTask;
            var toCatalog = await toCatalogTask;
            if (fromCatalog is null || toCatalog is null)
                return Result.Fail("CATALOG_NOT_FOUND");
            foreach (var item in request.Items)
            {
                var lot = lots.First(it => it.Id == item.LotId);
                var product = products.First(it => it.Id == lot.ProductId);
                fromCatalog.ReturnItem(item.LotId, item.Quantity);
                toCatalog.AddItem(product, lot, item.Quantity);
            }
            await _catalogRepository.Update(fromCatalog);
            await _catalogRepository.Update(toCatalog);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }
    public record CatalogNextStatusCommand(Guid CatalogId, Guid AccountableId, Guid AgentId, string Comment) : ICommand;

    public record CatalogClosePackageCommand : ICommand
    {
        public Guid CatalogId { get; init; }
        public Guid OwnerId { get; init; }
        public IList<CatalogClosePackageItemCommand> Items { get; init; }
        public bool Done { get; set; }
    }

    public record CatalogTransferItemsCommand : ICommand
    {
        public Guid FromCatalogId { get; init; }
        public Guid AgentId { get; init; }
        public Guid ToCatalogId { get; init; }
        public IList<CatalogClosePackageItemCommand> Items { get; init; }
    }

    public record CatalogClosePackageItemCommand
    {
        public Guid LotId { get; set; }
        public decimal Quantity { get; set; }
    }
}
