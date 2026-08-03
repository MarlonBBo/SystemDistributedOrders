using SystemDistributedOrders.Domain.Commons;

namespace SystemDistributedOrders.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    private Product()
    {
        Name = string.Empty;
    }

    public Product(string name, decimal price)
    {
        ValidateName(name);
        ValidatePrice(price);

        Name = name.Trim();
        Price = price;
    }

    public void Rename(string name)
    {
        ValidateName(name);

        Name = name.Trim();
        MarkAsUpdate();
    }

    public void ChangePrice(decimal price)
    {
        ValidatePrice(price);

        Price = price;
        MarkAsUpdate();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("O preço deve ser maior que zero.", nameof(price));
    }
}
