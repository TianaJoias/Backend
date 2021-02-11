using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Portifolio;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Stock
{
    public class LotSearchQueryHandler : IQueryHandler<LotSearchQuery, LotResult>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IProductRepository _productRepository;

        public LotSearchQueryHandler(ILotRepository lotRepository, IProductRepository productRepository)
        {
            _lotRepository = lotRepository;
            _productRepository = productRepository;
        }

        public async Task<Result<LotResult>> Handle(LotSearchQuery request, CancellationToken cancellationToken)
        {
            var lot = await _lotRepository.GetByQuery(it => it.EAN.Equals(request.ean));
            if (lot is null)
                return Result.Ok<LotResult>(null);
            var product = await _productRepository.GetById(lot.ProductId);
            var lotResult = product.Adapt<LotResult>();
            lotResult = lot.Adapt(lotResult);
            return Result.Ok(lotResult);
        }
    }
}
