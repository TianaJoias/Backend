using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using Domain.Portifolio;
using Domain.Stock;
using MediatR;

namespace WebApi.Aplication.Stock
{
    public class ProductConfirmedSaleEventHandler : INotificationHandler<ProductConfirmedSaleEvent>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IProductStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductConfirmedSaleEventHandler(ILotRepository lotRepository, IProductStockRepository productStockRepository, IUnitOfWork unitOfWork)
        {
            _lotRepository = lotRepository;
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ProductConfirmedSaleEvent notification, CancellationToken cancellationToken)
        {
            var lot = await _lotRepository.GetById(notification.LotId);
            lot.ConfirmSale(notification.Quantity);
            await _lotRepository.Update(lot);
            var product = await _productStockRepository.GetById(notification.ProdutoId);
            product.ConfirmSale(notification.Quantity);
            await _productStockRepository.Update(product);
            await _unitOfWork.Commit();
        }
    }
    public class ProductReturnedEventHandler : INotificationHandler<ProductReturnedEvent>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IProductStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductReturnedEventHandler(ILotRepository lotRepository, IProductStockRepository productStockRepository, IUnitOfWork unitOfWork)
        {
            _lotRepository = lotRepository;
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ProductReturnedEvent notification, CancellationToken cancellationToken)
        {
            var lot = await _lotRepository.GetById(notification.LotId);
            lot.Return(notification.Quantity);
            await _lotRepository.Update(lot);
            var product = await _productStockRepository.GetById(notification.ProdutoId);
            product.Return(notification.Quantity);
            await _productStockRepository.Update(product);
            await _unitOfWork.Commit();
        }
    }
    public class ProductReservedEventHandler : INotificationHandler<ProductReservedEvent>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IProductStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductReservedEventHandler(ILotRepository lotRepository, IProductStockRepository productStockRepository, IUnitOfWork unitOfWork)
        {
            _lotRepository = lotRepository;
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ProductReservedEvent notification, CancellationToken cancellationToken)
        {
            var lot = await _lotRepository.GetById(notification.LotId);
            lot.Reserve(notification.Quantity);
            await _lotRepository.Update(lot);
            var product = await _productStockRepository.GetByQuery(it=> it.ProductId ==  notification.ProdutoId);
            product.Reserve(notification.Quantity);
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
            var product = await _productStockRepository.GetByQuery(it=> it.ProductId == notification.ProductId);
            product.Deposit(notification.Quantity);
            await _productStockRepository.Update(product);
            await _unitOfWork.Commit();
        }
    }
}
