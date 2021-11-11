using System;

namespace Domain.Products.Write
{
    public class Image : BaseEntity
    {
        public string Src { get; private set; }
        public string Alt { get; private set; }
        public DateTime CreateAt { get; private set; }
        protected Image()
        {

        }

        public Image(string src, string alt)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                throw new ArgumentException($"'{nameof(src)}' cannot be null or whitespace.", nameof(src));
            }

            if (string.IsNullOrWhiteSpace(alt))
            {
                throw new ArgumentException($"'{nameof(alt)}' cannot be null or whitespace.", nameof(alt));
            }
            Src = src;
            Alt = alt;
        }
    }
}
