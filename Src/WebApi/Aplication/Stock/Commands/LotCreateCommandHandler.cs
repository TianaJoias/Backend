using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Stock;
using FluentResults;

namespace WebApi.Aplication.Stock.Commands
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
    public record LotCreateCommand(Guid ProductId, decimal CostValue, decimal SaleValue, decimal Quantity, string Number, IList<Guid> SuppliersId, decimal? Weight, DateTime Date) : ICommand;

    public record LotUpdateCommand(Guid Id, Guid ProductId, decimal CostValue, decimal SaleValue, decimal Quantity, string Number, IList<Guid> SuppliersId, decimal? Weight, DateTime Date) : ICommand;
}
