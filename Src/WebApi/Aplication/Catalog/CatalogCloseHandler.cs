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
    public class CatalogCloseHandler : ICommandHandler<CatalogCloseCommand>
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CatalogCloseHandler(ICatalogRepository catalogRepository, IUnitOfWork unitOfWork)
        {
            _catalogRepository = catalogRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CatalogCloseCommand request, CancellationToken cancellationToken)
        {
            var catalog = await _catalogRepository.GetByQuery(it=> it.Agent.Id == request.OwnerId && it.Id == request.CatalogId);
            if(catalog is null)
                return Result.Fail("CATALOG_NOT_FOUND");
            foreach (var item in request.Items)
                catalog.Return(item.LotId, item.Quantity);
            catalog.Close();
            await _catalogRepository.Update(catalog);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }

    public record CatalogCloseCommand : ICommand
    {
        public Guid CatalogId { get; init; }
        public Guid OwnerId { get; init; }
        public IList<CatalogCloseItemCommand> Items { get; init; }
    }

    public record CatalogCloseItemCommand
    {
        public Guid LotId { get; set; }
        public decimal Quantity { get; set; }
    }
}
