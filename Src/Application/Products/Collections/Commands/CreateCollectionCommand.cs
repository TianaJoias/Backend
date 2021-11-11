using System;
using Application.Common;

namespace Application.Products.Collections.Commands
{
    public class CreateCollectionCommand : ICommand
    {
        public string Title { get; set; }
        public Guid[] ProductIds { get; set; }

        public (string src, string alt)[] Image { get; set; }
    }
}
