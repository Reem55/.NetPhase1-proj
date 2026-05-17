namespace CleanArchitectureDemo.Domain;

public delegate void ProductChangedHandler(string eventName, Product product);

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IProductService
{
    event ProductChangedHandler? ProductAdded;
    event ProductChangedHandler? ProductPriceUpdated;
    event ProductChangedHandler? ProductDeleted;

    Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetExpensiveProductsAsync(decimal minPrice, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByCategoryAsync(Category category, CancellationToken cancellationToken = default);
    Task<ProductSummary> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<Product> AddProductAsync(string name, decimal price, Category category, CancellationToken cancellationToken = default);
    Task UpdatePriceAsync(Guid id, decimal newPrice, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed record ProductSummary(
    int TotalCount,
    decimal AveragePrice,
    decimal HighestPrice,
    decimal LowestPrice,
    string MostExpensiveProduct
);
