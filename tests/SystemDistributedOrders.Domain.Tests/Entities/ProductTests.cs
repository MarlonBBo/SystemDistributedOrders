using SystemDistributedOrders.Domain.Entities;

namespace SystemDistributedOrders.Domain.Tests.Entities;

public sealed class ProductTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProduct()
    {
        var product = new Product("Coca-Cola", 10.60m);

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal("Coca-Cola", product.Name);
        Assert.Equal(10.60m, product.Price);
        Assert.NotEqual(default, product.CreatedAt);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Product(" ", 10.60m));

        Assert.StartsWith("O nome do produto é obrigatório.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidPrice_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Product("Coca-Cola", 0));

        Assert.StartsWith("O preço deve ser maior que zero.", exception.Message);
    }

    [Fact]
    public void Rename_WithValidName_ShouldChangeNameAndMarkAsUpdated()
    {
        var product = new Product("Coca-Cola", 10.60m);

        product.Rename("Coca-Cola Zero");

        Assert.Equal("Coca-Cola Zero", product.Name);
        Assert.NotEqual(default, product.UpdatedAt);
    }

    [Fact]
    public void ChangePrice_WithValidPrice_ShouldChangePriceAndMarkAsUpdated()
    {
        var product = new Product("Coca-Cola", 10.60m);

        product.ChangePrice(12.90m);

        Assert.Equal(12.90m, product.Price);
        Assert.NotEqual(default, product.UpdatedAt);
    }
}
