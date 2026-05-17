namespace CleanArchitectureDemo.Domain;

public sealed class Product
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public Category Category { get; private set; }
    public DateTime CreatedAt { get; }

    public Product(Guid id, string name, decimal price, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        Id = id;
        Name = name;
        Price = price;
        Category = category;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative.");
        Price = newPrice;
    }

    public override string ToString() =>
        $"[{Category}] {Name} — ${Price:F2}";
}

public enum Category { Electronics, Accessories, Furniture }
