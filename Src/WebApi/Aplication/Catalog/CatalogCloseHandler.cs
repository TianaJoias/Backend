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
            var catalog = await _catalogRepository.GetById(request.CatalogId);
            foreach (var item in request.Items)
                catalog.Remaining(item.ProductId, item.Quantity);
            catalog.Close();
            await _catalogRepository.Update(catalog);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }

    public record CatalogCloseCommand : ICommand
    {
        public Guid CatalogId { get; init; }
        public IList<CatalogCloseItemCommand> Items { get; init; }
    }

    public record CatalogCloseItemCommand
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
