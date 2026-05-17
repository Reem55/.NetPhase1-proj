using CleanArchitectureDemo.Domain;

namespace CleanArchitectureDemo.Infrastructure;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _products.ToList().AsReadOnly();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            _products.Add(product);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Product {product.Id} not found.");
            _products[index] = product;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var product = _products.FirstOrDefault(p => p.Id == id)
                ?? throw new KeyNotFoundException($"Product {id} not found.");
            _products.Remove(product);
        }
        finally
        {
            _lock.Release();
        }
    }
}
