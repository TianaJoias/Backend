using System.Collections.Generic;

namespace Domain.Portifolio
{
    public class Tag : BaseEntity
    {
        public enum TagType { Gender, Typology, Plated, Color, Group }
        public string Name { get; private set; }
        public TagType Type { get; private set; }
        protected Tag()
        {

        }

        public Tag(string name, TagType type)
        {
            Name = name;
            Type = type;
        }
        public IList<Product> Products { get; private set; }
    }
}
