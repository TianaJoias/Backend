using System;

namespace Domain.Products.Write
{
    public class Tag: BaseEntity
    {
        public string Title { get; private set; }
        public DateTime UpdateAt { get; private set; }
        public DateTime CreateAt { get; private set; }

        protected Tag()
        {

        }
        public Tag(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));
            }
            Title = title;
            UpdateAt = CreateAt = Clock.Now;
        }
    }
}
