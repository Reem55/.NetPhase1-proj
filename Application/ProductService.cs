using CleanArchitectureDemo.Domain;

namespace CleanArchitectureDemo.Application;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public event ProductChangedHandler? ProductAdded;
    public event ProductChangedHandler? ProductPriceUpdated;
    public event ProductChangedHandler? ProductDeleted;

    public static readonly Action<string> Log =
        message => Console.WriteLine($"  [log {DateTime.Now:HH:mm:ss}] {message}");

    public static readonly Func<Product, string> FormatProduct =
        p => $"{p.Name} ({p.Category}) — ${p.Price:F2}";

    public ProductService(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products.OrderBy(p => p.Name).ToList();
    }

    public async Task<IReadOnlyList<Product>> GetExpensiveProductsAsync(decimal minPrice, CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products
            .Where(p => p.Price > minPrice)
            .OrderByDescending(p => p.Price)
            .ToList();
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products
            .Where(p => p.Category == category)
            .OrderBy(p => p.Price)
            .ToList();
    }

    public async Task<ProductSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);

        if (!products.Any())
            return new ProductSummary(0, 0, 0, 0, "N/A");

        return new ProductSummary(
            TotalCount:           products.Count,
            AveragePrice:         products.Average(p => p.Price),
            HighestPrice:         products.Max(p => p.Price),
            LowestPrice:          products.Min(p => p.Price),
            MostExpensiveProduct: products.MaxBy(p => p.Price)!.Name
        );
    }

    public async Task<Product> AddProductAsync(string name, decimal price, Category category, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetAllAsync(cancellationToken);
        if (existing.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A product named '{name}' already exists.");

        var product = new Product(Guid.NewGuid(), name, price, category);
        await _repository.AddAsync(product, cancellationToken);
        ProductAdded?.Invoke("ProductAdded", product);
        return product;
    }

    public async Task UpdatePriceAsync(Guid id, decimal newPrice, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product {id} not found.");

        product.UpdatePrice(newPrice);
        await _repository.UpdateAsync(product, cancellationToken);
        ProductPriceUpdated?.Invoke("ProductPriceUpdated", product);
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product {id} not found.");

        await _repository.DeleteAsync(product.Id, cancellationToken);
        ProductDeleted?.Invoke("ProductDeleted", product);
    }
}
