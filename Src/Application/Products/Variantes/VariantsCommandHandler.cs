using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Products.Repositories;
using Application.Products.Variantes.Commands;
using Domain.Products.Write;
using FluentResults;

namespace Application.Products.Variantes
{
    public class VariantsCommandHandler : ICommandHandler<CreateVariantCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IVariantRepository _variantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VariantsCommandHandler(IProductRepository productRepository, IVariantRepository variantRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.Find(request.ProductId);
            var variante = new Variant(request.Title, product);
            await _variantRepository.Add(variante);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }
}
