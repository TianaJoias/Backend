using Application.Common;

namespace Application.Products.Categories.Commands
{
    public class CreateCategoryCommand : ICommand
    {
        public string Title { get; set; }
    }
}
