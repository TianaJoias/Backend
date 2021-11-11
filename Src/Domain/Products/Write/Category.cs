using System;
using System.Collections.Generic;

namespace Domain.Products.Write
{
    public class Category : BaseEntity
    {
        protected Category()
        {

        }

        public Category(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));
            }
            Title = title;
            UpdateAt = CreateAt = Clock.Now;
        }

        public string Title { get; private set; }
        public DateTime UpdateAt { get; private set; }
        public DateTime CreateAt { get; private set; }
        public IList<Product> Products { get; set; }
    }
}
