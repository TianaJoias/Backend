using System;
using Application.Common;
using FluentValidation;

namespace Application.Products.Commands
{
    public class CreateProductCommand : ICommand
    {
        public string Title { get; set; }
        public string HtmlBody { get; set; }
        public Guid[] Categories { get; set; }
        public Guid Id { get; set; }
    }

    public class CreateProductCommndValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommndValidator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.HtmlBody).NotEmpty();
            RuleFor(c => c.Categories).NotEmpty();
        }
    }
}
