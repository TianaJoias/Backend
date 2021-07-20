using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain;
using Domain.Specification;
using Domain.Stock;
using FluentResults;

namespace WebApi.Aplication.Stock.Commands
{
    public class LotCreateCommandHandler : ICommandHandler<LotCreateCommand>,
        ICommandHandler<LotUpdateCommand>,
         ICommandHandler<LotCreateBulkCommand>
    {
        private readonly ILotRepository _lotRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IEANRepository _iEANRepository;
        private readonly ISupplierProductRepository _supplierProductRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LotCreateCommandHandler(ILotRepository lotRepository, ISupplierRepository supplierRepository, IEANRepository iEANRepository,
            ISupplierProductRepository supplierProductRepository, IUnitOfWork unitOfWork)
        {
            _lotRepository = lotRepository;
            _supplierRepository = supplierRepository;
            _iEANRepository = iEANRepository;
            _supplierProductRepository = supplierProductRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(LotCreateCommand request, CancellationToken cancellationToken)
        {
            await LotAdd(request);
            await _unitOfWork.Commit();
            return Result.Ok();
        }

        private async Task LotAdd(LotCreateCommand request)
        {
            var suppliers = await _supplierRepository.Filter(it => request.SuppliersId.Contains(it.Id));
            var lot = new Lot(request.ProductId, request.CostValue, request.SaleValue, request.Quantity, request.Number, suppliers)
            {
                Weight = request.Weight,
                Date = request.Date,
                EAN = await NextEAN()
            };
            await _lotRepository.Add(lot);
        }

        public async Task<Result> Handle(LotUpdateCommand request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierRepository.Filter(it => request.SuppliersId.Contains(it.Id));
            var lot = await _lotRepository.Find(request.Id);
            lot.Weight = request.Weight;
            lot.Date = request.Date;
            lot.Number = request.Number;
            lot.CostPrice = request.CostValue;
            lot.SalePrice = request.SaleValue;
            lot.Suppliers.Clear();
            suppliers.ForEach(lot.Suppliers.Add);
            await _lotRepository.Update(lot);
            await _unitOfWork.Commit();
            return Result.Ok();
        }

        public async Task<Result> Handle(LotCreateBulkCommand request, CancellationToken cancellationToken)
        {
            await Task.WhenAll(request.Batch.Select(BatchExec).ToList());
            await _unitOfWork.Commit();
            return Result.Ok();
        }

        private async Task BatchExec(BatchLotCreateItemCommand item)
        {
            var supplier = await _supplierProductRepository.Find(new SuplierProductByProductCodeAndId(item.ProductCode, item.SuppliersId));
            if (supplier is not null)
            {
                var command = new LotCreateCommand(supplier.Product.Id, item.CostValue, item.SaleValue, item.Quantity, item.Number, new List<Guid>(1) { item.SuppliersId }, item.Weight, item.Date);
                await LotAdd(command);
            }
        }

        private async Task<string> NextEAN()
        {
            var EAN = await _iEANRepository.Find(it => it.IsActive);
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

    public class SuplierProductByProductCodeAndId : BaseSpecifcation<SupplierProduct>
    {
        public SuplierProductByProductCodeAndId(string code, Guid id) : base(it => it.Code == code && it.Supplier.Id == id)
        {
            AddInclude(x => x.Product);
            AddInclude(x => x.Supplier);
        }
    }

    public record LotCreateBulkCommand(IList<BatchLotCreateItemCommand> Batch) : ICommand;
    public record BatchLotCreateItemCommand
    {
        public Guid SuppliersId { get; set; }
        public string ProductCode { get; set; }
        public decimal CostValue { get; set; }
        public decimal SaleValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal Weight { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
    }

    public record LotCreateCommand(Guid ProductId, decimal CostValue, decimal SaleValue, decimal Quantity, string Number, IList<Guid> SuppliersId, decimal? Weight, DateTime Date) : ICommand;

    public record LotUpdateCommand(Guid Id, Guid ProductId, decimal CostValue, decimal SaleValue, decimal Quantity, string Number, IList<Guid> SuppliersId, decimal? Weight, DateTime Date) : ICommand;

    public class ProductSupplierCommandHandler : ICommandHandler<ProductSupplierCommand>
    {
        private readonly ISupplierProductRepository _supplierProductRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductStockRepository _productStock;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSupplierCommandHandler(ISupplierProductRepository supplierProductRepository,
            ISupplierRepository supplierRepository,
            IProductStockRepository productStock,
            IUnitOfWork unitOfWork)
        {
            _supplierProductRepository = supplierProductRepository;
            _supplierRepository = supplierRepository;
            _productStock = productStock;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(ProductSupplierCommand request, CancellationToken cancellationToken)
        {
            var productSupplierExists = await _supplierProductRepository.Contains(it => it.Product.Id == request.ProductId && it.Supplier.Id == request.SupplierId && it.Code == request.Code);
            if (productSupplierExists)
                return Result.Fail("Supplier with code aready registered.");
            var product = await _productStock.Find(request.ProductId);
            var supplier = await _supplierRepository.Find(it => it.Id == request.SupplierId);
            var productSupplier = new SupplierProduct(supplier, product, request.Code);
            await _supplierProductRepository.Add(productSupplier);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }

    public record ProductSupplierCommand(Guid ProductId, Guid SupplierId, string Code) : ICommand;
}
