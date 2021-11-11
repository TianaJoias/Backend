using System;
using Application.Common;

namespace Application.Products.Variantes.Commands
{
    public class CreateVariantCommand : ICommand
    {
        public string Title { get; set; }
        public Guid ProductId { get; set; }
    }
}
