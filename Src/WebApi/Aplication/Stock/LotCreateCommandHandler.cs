using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Portifolio;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Stock
{
    public class LotCreateCommandHandler : ICommandHandler<LotCreateCommand>,
        ICommandHandler<LotUpdateCommand>
    {
        private readonly ILotRepository _lotRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IEANRepository _iEANRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LotCreateCommandHandler(ILotRepository lotRepository, ISupplierRepository supplierRepository, IEANRepository iEANRepository, IUnitOfWork unitOfWork)
        {
            _lotRepository = lotRepository;
            _supplierRepository = supplierRepository;
            _iEANRepository = iEANRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(LotCreateCommand request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierRepository.List(it => request.SuppliersId.Contains(it.Id));
            var lot = new Lot(request.ProductId, request.CostValue, request.SaleValue, request.Quantity, request.Number, suppliers)
            {
                Weight = request.Weight,
                Date = request.Date,
                EAN = await NextEAN()
            };
            await _lotRepository.Add(lot);
            await _unitOfWork.Commit();
            return Result.Ok();
        }

        public async Task<Result> Handle(LotUpdateCommand request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierRepository.List(it => request.SuppliersId.Contains(it.Id));
            var lot = await _lotRepository.GetById(request.Id);
            lot.Weight = request.Weight;
            lot.Date = request.Date;
            lot.Number = request.Number;
            lot.Suppliers.Clear();
            suppliers.ForEach(lot.Suppliers.Add);
            await _lotRepository.Update(lot);
            await _unitOfWork.Commit();
            return Result.Ok();
        }

        private async Task<string> NextEAN()
        {
            var EAN = await _iEANRepository.GetByQuery(it => it.IsActive);
            if (EAN is null)
            {
                var newEan = new EAN();
                await _iEANRepository.Add(newEan);
                EAN = newEan;
            }
            else
            if (!EAN.CanGenereate(1))
            {
                EAN.Deactivate();
                await _iEANRepository.Update(EAN);
                var newEan = new EAN();
                await _iEANRepository.Add(newEan);
                EAN = newEan;
            }
            return EAN.Generate(1).First();
        }
    }
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
    public record LotCreateCommand(Guid ProductId, decimal CostValue, decimal SaleValue, decimal Quantity, string Number, IList<Guid> SuppliersId, decimal? Weight, DateTime Date) : ICommand;

    public record LotUpdateCommand(Guid Id, Guid ProductId, decimal CostValue, decimal SaleValue, decimal Quantity, string Number, IList<Guid> SuppliersId, decimal? Weight, DateTime Date) : ICommand;

    public record LotSearchQuery(string ean) : IQuery<LotResult>;


    public record LotResult
    {
        public Guid Id { get; set; }
        public string Number { get; init; }
        public Guid ProductId { get; init; }
        public string SKU { get; init; }
        public string Description { get; init; }
        public decimal CostPrice { get; init; }
        public string EAN { get; init; }
    }
}
