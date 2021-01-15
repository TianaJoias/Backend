using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using FluentResults;

namespace WebApi.Aplication.Catalog
{
    public class CatalogClosePackageCommandHandler : ICommandHandler<CatalogClosePackageCommand>,
        ICommandHandler<CatalogNextStatusCommand>
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CatalogClosePackageCommandHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
        {
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
                catalog.Next();
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
    }
    public record CatalogNextStatusCommand(Guid CatalogId, Guid AccountableId, Guid AgentId, string Comment) : ICommand;

    public record CatalogClosePackageCommand : ICommand
    {
        public Guid CatalogId { get; init; }
        public Guid OwnerId { get; init; }
        public IList<CatalogClosePackageItemCommand> Items { get; init; }
        public bool Done { get; set; }
    }

    public record CatalogClosePackageItemCommand
    {
        public Guid LotId { get; set; }
        public decimal Quantity { get; set; }
    }
}
