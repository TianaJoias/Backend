using Xunit;
using FluentAssertions;
using System;
using Moq;
using Domain.Products.Write;

namespace Domain.Tests.Products
{
    [Collection("TESTE")]
    public class ProductTests
    {

        [Fact]
        public void ShouldCreateProduct()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            Clock.SetDateTime(DateTime.Now);
            var product = new Product(title, htmlBody);

            product.Title.Should().Be(title);
            product.HtmlBody.Should().Be(htmlBody);
            product.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue);
            product.UpdateAt.Should().BeSameDateAs(product.CreateAt);
            
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Product Title", "")]
        [InlineData("", "Html Body")]
        [InlineData(null, null)]
        [InlineData("Product Title", null)]
        [InlineData("    ", null)]
        [InlineData("Product Title", "  ")]
        public void ShoulCanNotCreateProduct(string title, string htmlBody)
        {
            Action productCreate = () => new Product(title, htmlBody);

            productCreate.Should().ThrowExactly<ArgumentException>();
        }


        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void ShouldCanNotUpdateTitle(string newTitle)
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var createTime = DateTime.Now;
            var product = new Product(title, htmlBody);

            Action updateTitle =
                () => product.UpdateTitle(newTitle);

            updateTitle.Should().ThrowExactly<ArgumentException>();
            product.Title.Should().Be(title);
            product.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue);
            product.CreateAt.Should().BeSameDateAs(product.UpdateAt);
        }

        [Fact]
        public void ShouldUpdateTitle()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var newTitle = "new Title";
            product.UpdateTitle(newTitle);


            product.Title.Should().Be(newTitle);
            product.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue);
            product.CreateAt.Should().BeBefore(product.UpdateAt);
        }


        [Fact]
        public void ShouldUpdateHtmlBody()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var newBody = "new HtmlBody";
            product.UpdateBody(newBody);


            product.HtmlBody.Should().Be(newBody);
            product.CreateAt.Should().BeBefore(product.UpdateAt);
        }


        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void ShouldCanNotUpdateBody(string newBody)
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);
            Action updateTitle =
                () => product.UpdateBody(newBody);

            updateTitle.Should().ThrowExactly<ArgumentException>();
            product.HtmlBody.Should().Be(htmlBody);
            product.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue);
            product.CreateAt.Should().BeSameDateAs(product.UpdateAt);
        }

        [Fact]
        public void ShouldAddCategories()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var categoryMock = new Mock<Category>();
            categoryMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var category = categoryMock.Object;
            product.AddCategories(category);

            product.Categories.Should().Contain(category).And.HaveCount(1);
        }

        [Fact]
        public void ShouldCanNotAddSameCategory()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var categoryMock = new Mock<Category>();
            categoryMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var category = categoryMock.Object;
            Action addAction = () => product.AddCategories(category);
            addAction();
            addAction.Should().ThrowExactly<ArgumentException>();
            product.Categories.Should().Contain(category).And.HaveCount(1);
        }

        [Fact]
        public void ShouldAddTags()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var tagMock = new Mock<Tag>();
            tagMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var tag = tagMock.Object;
            product.AddTags(tag);

            product.Tags.Should().Contain(tag).And.HaveCount(1);
        }

        [Fact]
        public void ShouldCanNotAddSameTag()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var tagMock = new Mock<Tag>();
            tagMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var tag = tagMock.Object;
            Action addAction = () => product.AddTags(tag);
            addAction();
            addAction.Should().ThrowExactly<ArgumentException>();
            product.Tags.Should().Contain(tag).And.HaveCount(1);
        }

        [Fact]
        public void ShoulRemoveTag()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var tagMock = new Mock<Tag>();
            tagMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var tag = tagMock.Object;
            product.AddTags(tag);

            product.Tags.Should().Contain(tag).And.HaveCount(1);

            product.RemoveTags(tag);

            product.Tags.Should().BeEmpty();
        }

        [Fact]
        public void ShouldCanNotRemoveNotExistingTag()
        {
            var title = "Product Title";
            var htmlBody = "Html Body";
            var product = new Product(title, htmlBody);

            var tagMock = new Mock<Tag>();
            tagMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var tag = tagMock.Object;
            product.AddTags(tag);
            var otherTag = Mock.Of<Tag>();
            Action removeTag = () => product.RemoveTags(otherTag);

            removeTag.Should().ThrowExactly<ArgumentException>();
            product.Tags.Should().Contain(tag).And.HaveCount(1);
        }
    }
}
