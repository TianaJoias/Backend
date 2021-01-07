using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using Domain.Portifolio;
using Domain.Stock;
using MediatR;

namespace WebApi.Aplication.Stock
{
    public class ProductSoldEventHandler : INotificationHandler<ProductSoldEvent>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IProductStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSoldEventHandler(ILotRepository lotRepository, IProductStockRepository productStockRepository, IUnitOfWork unitOfWork)
        {
            _lotRepository = lotRepository;
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ProductSoldEvent notification, CancellationToken cancellationToken)
        {
            var lot = await _lotRepository.GetById(notification.LotId);
            lot.Withdraw(notification.Quantity);
            await _lotRepository.Update(lot);
            var product = await _productStockRepository.GetById(notification.ProdutoId);
            product.Withdraw(notification.Quantity);
            await _productStockRepository.Update(product);
            await _unitOfWork.Commit();
        }
    }
    public class NewProductEventHandler : INotificationHandler<NewProductEvent>
    {
        private readonly IProductStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;
        public NewProductEventHandler(IProductStockRepository productStockRepository, IUnitOfWork unitOfWork)
        {
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(NewProductEvent notification, CancellationToken cancellationToken)
        {
            var product = new ProductStock(notification.ProductId);
            await _productStockRepository.Add(product);
            await _unitOfWork.Commit();
        }
    }
    public class NewLotEventHandler : INotificationHandler<NewLotEvent>
    {
        private readonly IProductStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NewLotEventHandler(IProductStockRepository productStockRepository, IUnitOfWork unitOfWork)
        {
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(NewLotEvent notification, CancellationToken cancellationToken)
        {
            var product = await _productStockRepository.GetById(notification.ProductId);
            product.Deposit(notification.Quantity);
            await _productStockRepository.Update(product);
            await _unitOfWork.Commit();
        }
    }
}
