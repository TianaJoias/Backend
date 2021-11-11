using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Application.Products.Categories.Commands;
using Application.Products.Repositories;
using Domain.Products.Write;
using FluentResults;

namespace Application.Products.Categories
{
    public class CategoryCommandHandler : ICommandHandler<CreateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }
        public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category(request.Title);
            await _categoryRepository.Add(category);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }
}
