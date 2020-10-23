using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using Domain.Stock;
using MediatR;

namespace WebApi.Aplication.Stock
{
    public class ProductSoldEventHandler : INotificationHandler<ProductSoldEvent>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSoldEventHandler(ILotRepository lotRepository, IUnitOfWork unitOfWork)
        {
            _lotRepository = lotRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ProductSoldEvent notification, CancellationToken cancellationToken)
        {
            var lot = await _lotRepository.GetById(notification.LotId);
            lot.Withdraw(notification.Quantity);
            await _lotRepository.Update(lot);
            await _unitOfWork.Commit();
        }
    }
}
