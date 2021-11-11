using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Products.Collections.Commands;
using Application.Products.Repositories;
using Application.Specification;
using Domain.Products.Write;
using FluentResults;

namespace Application.Products.Collections
{
    public class CollectionCommandHandler : ICommandHandler<CreateCollectionCommand>
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CollectionCommandHandler(ICollectionRepository collectionRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _collectionRepository = collectionRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
        {
            var byIds = SpecificationBuilder<Product>.Where(it => request.ProductIds.Contains(it.Id)).Build();
            var products = await _productRepository.Find(byIds);
            var images = request.Image.Select(it => new Image(it.src, it.alt)).ToArray();
            var collection = new Collection(request.Title);
            collection.AddProducts(products);
            collection.AddImages(images);
            await _collectionRepository.Add(collection);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }
}
