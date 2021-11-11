using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Products.Write
{
    public class Variant : BaseEntity
    {
        private readonly List<Image> _images = new();
        public DateTime UpdateAt { get; private set; }
        public DateTime CreateAt { get; private set; }
        public Product Product { get; private set; }
        public string Title { get; private set; }
        public string SKU { get; private set; }
        public string Barcode { get; private set; }
        public IReadOnlyList<Image> Images => _images;
        protected Variant()
        {

        }

        public Variant(string title, Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            UpdateTitle(title);
            Product = product;
            CreateAt = UpdateAt = Clock.Now;
        }
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));
            }
            Title = title;
            UpdateAt = Clock.Now;
        }
        public void AddImages(params Image[] images)
        {
            var exists = _images.Any(it => images.Any(c => c.Id == it.Id));
            if (exists)
            {
                throw new ArgumentException($"'{nameof(images)}' aready added.", nameof(images));
            }
            _images.AddRange(images);
            UpdateAt = Clock.Now;
        }
        public void RemoveImages(params Image[] images)
        {
            var exists = images.All(it => _images.Any(c => c.Id == it.Id));
            if (exists)
            {
                throw new ArgumentException($"'{nameof(images)}' not added.", nameof(images));
            }
            foreach (var image in images)
            {
                _images.Remove(image);
            }
            UpdateAt = Clock.Now;
        }
    }
}
