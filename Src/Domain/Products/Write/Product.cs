using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Products.Write
{
    public class Product : BaseEntity
    {
        private readonly List<Tag> _tags = new();
        private readonly List<Category> _categories = new();
        private readonly List<Variant> _variants = new();
        public string Title { get; private set; }
        public string HtmlBody { get; private set; }
        public DateTime UpdateAt { get; private set; }
        public DateTime CreateAt { get; private set; }
        public IReadOnlyList<Category> Categories => _categories;
        public IReadOnlyList<Variant> Variants => _variants;
        public IReadOnlyList<Tag> Tags => _tags;
        public string CorrelationId { get; private set; }
        public IList<Collection> Collections { get; set; }

        protected Product()
        {
        }
        public Product(string title, string htmlBody, string correlationId = null)
        {
            UpdateTitle(title);
            UpdateBody(htmlBody);
            CorrelationId = correlationId;
            UpdateAt = CreateAt = TimeProvider.Current.UtcNow;
            AddEvent(new NewProductEvent(this));
        }
        public void AddCategories(params Category[] categories)
        {
            var exists = _categories.Any(it => categories.Any(c => c.Id == it.Id));
            if (exists)
            {
                throw new ArgumentException($"'{nameof(categories)}' aready added.", nameof(categories));
            }
            _categories.AddRange(categories);
            UpdateAt = TimeProvider.Current.UtcNow;
        }
        public void RemoveCategories(params Category[] categories)
        {
            var exists = categories.All(it => _categories.Any(c => c.Id == it.Id));
            if (!exists)
            {
                throw new ArgumentException($"'{nameof(categories)}' not added.", nameof(categories));
            }
            foreach (var category in categories)
            {
                _categories.Remove(category);
            }
            UpdateAt = Clock.Now;
        }
        public void AddTags(params Tag[] tags)
        {
            var exists = _tags.Any(it => tags.Any(c => c.Id == it.Id));
            if (exists)
            {
                throw new ArgumentException($"'{nameof(tags)}' aready added.", nameof(tags));
            }
            _tags.AddRange(tags);
            UpdateAt = Clock.Now;
        }
        public void RemoveTags(params Tag[] tags)
        {
            var exists = tags.All(it => _tags.Any(c => c.Id == it.Id));
            if (!exists)
            {
                throw new ArgumentException($"'{nameof(tags)}' not added.", nameof(tags));
            }
            foreach (var tag in tags)
            {
                _tags.Remove(tag);
            }
            UpdateAt = Clock.Now;
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
        public void UpdateBody(string htmlBody)
        {
            HtmlBody = htmlBody;
            UpdateAt = TimeProvider.Current.UtcNow;
        }

    }

    public class NewProductEvent : BaseEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string HtmlBody { get; set; }
        public NewProductEvent()
        {

        }
        public NewProductEvent(Product product)
        {
            Id = product.Id;
            Title = product.Title;
            HtmlBody = product.HtmlBody;
        }
    }
}
