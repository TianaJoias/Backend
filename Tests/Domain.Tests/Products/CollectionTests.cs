using System;
using Domain.Products.Write;
using FluentAssertions;
using Moq;
using Xunit;

namespace Domain.Tests.Products
{

    public class CollectionTests
    {

        [Fact]
        public void ShouldCreateCollection()
        {
            var title = "Title";
            var collection = new Collection(title);

            collection.Title.Should().Be(title);
            collection.Products.Should().BeEmpty();
            collection.Images.Should().BeEmpty();
            collection.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue);
            collection.CreateAt.Should().BeSameDateAs(collection.UpdateAt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ShouldCanNotCreateCollection(string title)
        {
            Action createCollection = () => new Collection(title);

            createCollection.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ShouldAddProductsOnceTime()
        {
            var title = "Title";
            var collection = new Collection(title);

            var categoryMock = new Mock<Product>();
            categoryMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var product = categoryMock.Object;

            Action add = () => collection.AddProducts(product);
            add();

            collection.Products.Should().Contain(product).And.HaveCount(1);
            add.Should().ThrowExactly<ArgumentException>();
            collection.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue).And.BeBefore(collection.UpdateAt);
        }

        [Fact]
        public void ShouldAddImagesOnceTime()
        {
            var title = "Title";
            var collection = new Collection(title);

            var imageMock = new Mock<Image>();
            imageMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var image = imageMock.Object;

            Action add = () => collection.AddImages(image);
            add();

            collection.Images.Should().Contain(image).And.HaveCount(1);
            add.Should().ThrowExactly<ArgumentException>();
            collection.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue).And.BeBefore(collection.UpdateAt);
        }


        [Fact]
        public void ShouldRemoveOnlyInsertedImages()
        {
            var title = "Title";
            var collection = new Collection(title);

            var imageMock = new Mock<Image>();
            imageMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var image = imageMock.Object;
            var image2 = Mock.Of<Image>();
            collection.AddImages(image);
            collection.AddImages(image2);

            collection.Images.Should().HaveCount(2).And.Contain(image).And.Contain(image2);
            collection.RemoveImages(image);
            collection.Images.Should().HaveCount(1).And.Contain(image2).And.NotContain(image);
            Action retryRemove = () => collection.RemoveImages(image);
            retryRemove.Should().ThrowExactly<ArgumentException>();
            collection.RemoveImages(image2);
            collection.Images.Should().BeEmpty();
            collection.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue).And.BeBefore(collection.UpdateAt);
        }

        [Fact]
        public void ShouldRemoveOnlyInsertedProducts()
        {
            var title = "Title";
            var collection = new Collection(title);

            var productMock = new Mock<Product>();
            productMock.Setup(it => it.Id).Returns(Guid.NewGuid());
            var product = productMock.Object;
            var product2 = Mock.Of<Product>();
            collection.AddProducts(product);
            collection.AddProducts(product2);

            collection.Products.Should().HaveCount(2).And.Contain(product).And.Contain(product2);
            collection.RemoveProducts(product);
            collection.Products.Should().HaveCount(1).And.Contain(product2).And.NotContain(product);
            Action retryRemove = () => collection.RemoveProducts(product);
            retryRemove.Should().ThrowExactly<ArgumentException>();
            collection.RemoveProducts(product2);
            collection.Products.Should().BeEmpty();
            collection.CreateAt.Should().NotBeSameDateAs(DateTime.MinValue).And.BeBefore(collection.UpdateAt);
        }
    }
}
