using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Products.Write
{
    public class Collection : BaseEntity
    {
        private readonly List<Image> _images = new();
        private readonly List<Product> _products = new();
        public DateTime UpdateAt { get; private set; }
        public DateTime CreateAt { get; private set; }
        public string Title { get; private set; }
        public IReadOnlyList<Image> Images => _images;
        public IReadOnlyList<Product> Products => _products;
        protected Collection()
        {

        }

        public Collection(string title)
        {
            UpdateTitle(title);
            CreateAt = UpdateAt = TimeProvider.Current.UtcNow;
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));
            }
            Title = title;
            UpdateAt = TimeProvider.Current.UtcNow;
        }
        public void AddProducts(params Product[] products)
        {
            var exists = _products.Any(it => products.Any(c => c.Id == it.Id));
            if (exists)
            {
                throw new ArgumentException($"'{nameof(products)}' aready added.", nameof(products));
            }
            _products.AddRange(products);
            UpdateAt = TimeProvider.Current.UtcNow;
        }
        public void RemoveProducts(params Product[] products)
        {
            var exists = products.All(it => _products.Any(c => c.Id == it.Id));
            if (!exists)
            {
                throw new ArgumentException($"'{nameof(products)}' not added.", nameof(products));
            }
            foreach (var category in products)
            {
                _products.Remove(category);
            }
            UpdateAt = TimeProvider.Current.UtcNow;
        }
        public void AddImages(params Image[] images)
        {
            var exists = _images.Any(it => images.Any(c => c.Id == it.Id));
            if (exists)
            {
                throw new ArgumentException($"'{nameof(images)}' aready added.", nameof(images));
            }
            _images.AddRange(images);
            UpdateAt = TimeProvider.Current.UtcNow;
        }
        public void RemoveImages(params Image[] images)
        {
            var exists = images.All(it => _images.Any(c => c.Id == it.Id));
            if (!exists)
            {
                throw new ArgumentException($"'{nameof(images)}' not added.", nameof(images));
            }
            foreach (var image in images)
            {
                _images.Remove(image);
            }
            UpdateAt = TimeProvider.Current.UtcNow;
        }

    }
}
