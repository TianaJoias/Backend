using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using FluentResults;
using MediatR;

namespace WebApi.Aplication.Catalog
{
    public class CatalogClosePackageCommandHandler : ICommandHandler<CatalogClosePackageCommand>
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
            catalog.StartClosing();
            foreach (var item in request.Items)
                catalog.ReturnItem(item.LotId, item.Quantity);
            if (request.CompletedClosing)
                catalog.CompleteClosing();
            await _catalogRepository.Update(catalog);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }

    public record CatalogClosePackageCommand : ICommand
    {
        public Guid CatalogId { get; init; }
        public Guid OwnerId { get; init; }
        public IList<CatalogCloseItemCommand> Items { get; init; }
        public bool CompletedClosing { get; set; }
    }

    public record CatalogCloseItemCommand
    {
        public Guid LotId { get; set; }
        public decimal Quantity { get; set; }
    }
}
