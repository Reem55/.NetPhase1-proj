using CleanArchitectureDemo.Application;
using CleanArchitectureDemo.Domain;

namespace CleanArchitectureDemo.Presentation;

public sealed class ConsoleUI
{
    private readonly IProductService _service;

    public ConsoleUI(IProductService service)
    {
        _service = service;

        _service.ProductAdded        += (evt, p) => Console.WriteLine($"\n  ✅ [{evt}] {ProductService.FormatProduct(p)}");
        _service.ProductPriceUpdated += (evt, p) => Console.WriteLine($"\n  ✏️  [{evt}] {ProductService.FormatProduct(p)}");
        _service.ProductDeleted      += (evt, p) => Console.WriteLine($"\n  🗑️  [{evt}] {p.Name} removed");
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║     🛍️  Product Manager Console       ║");
        Console.WriteLine("╚══════════════════════════════════════╝");

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n Menu:");
            Console.WriteLine("  1. List all products");
            Console.WriteLine("  2. Filter expensive products");
            Console.WriteLine("  3. Filter by category");
            Console.WriteLine("  4. View summary");
            Console.WriteLine("  5. Add product");
            Console.WriteLine("  6. Update product price");
            Console.WriteLine("  7. Delete product");
            Console.WriteLine("  0. Exit");
            Console.Write("\n> Choose: ");

            var input = Console.ReadLine()?.Trim();

            try
            {
                switch (input)
                {
                    case "1": await ListAllAsync(cancellationToken);          break;
                    case "2": await FilterExpensiveAsync(cancellationToken);  break;
                    case "3": await FilterByCategoryAsync(cancellationToken); break;
                    case "4": await ShowSummaryAsync(cancellationToken);      break;
                    case "5": await AddProductAsync(cancellationToken);       break;
                    case "6": await UpdatePriceAsync(cancellationToken);      break;
                    case "7": await DeleteProductAsync(cancellationToken);    break;
                    case "0":
                        Console.WriteLine(" Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private async Task ListAllAsync(CancellationToken ct)
    {
        var products = await _service.GetAllProductsAsync(ct);
        PrintProducts("All Products", products);
    }

    private async Task FilterExpensiveAsync(CancellationToken ct)
    {
        Console.Write("Minimum price: $");
        if (!decimal.TryParse(Console.ReadLine(), out var min))
        {
            Console.WriteLine(" Invalid price.");
            return;
        }
        var products = await _service.GetExpensiveProductsAsync(min, ct);
        PrintProducts($"Products above ${min}", products);
    }

    private async Task FilterByCategoryAsync(CancellationToken ct)
    {
        Console.WriteLine("Categories: Electronics, Accessories, Furniture");
        Console.Write("Enter category: ");
        if (!Enum.TryParse<Category>(Console.ReadLine(), true, out var cat))
        {
            Console.WriteLine(" Invalid category.");
            return;
        }
        var products = await _service.GetByCategoryAsync(cat, ct);
        PrintProducts($"Category: {cat}", products);
    }

    private async Task ShowSummaryAsync(CancellationToken ct)
    {
        var s = await _service.GetSummaryAsync(ct);
        Console.WriteLine(" Summary:");
        Console.WriteLine($"  Total products : {s.TotalCount}");
        Console.WriteLine($"  Average price  : ${s.AveragePrice:F2}");
        Console.WriteLine($"  Highest price  : ${s.HighestPrice:F2}");
        Console.WriteLine($"  Lowest price   : ${s.LowestPrice:F2}");
        Console.WriteLine($"  Most expensive : {s.MostExpensiveProduct}");
    }

    private async Task AddProductAsync(CancellationToken ct)
    {
        Console.Write("Name: ");
        var name = Console.ReadLine() ?? "";

        Console.Write("Price: $");
        if (!decimal.TryParse(Console.ReadLine(), out var price))
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        Console.WriteLine("Category (Electronics / Accessories / Furniture):");
        Console.Write("> ");
        if (!Enum.TryParse<Category>(Console.ReadLine(), true, out var cat))
        {
            Console.WriteLine(" Invalid category.");
            return;
        }

        var product = await _service.AddProductAsync(name, price, cat, ct);
        Console.WriteLine($"Added: {product}");
    }

    private async Task UpdatePriceAsync(CancellationToken ct)
    {
        var products = await _service.GetAllProductsAsync(ct);
        PrintProductsWithIndex(products);

        Console.Write("Pick number: ");
        if (!int.TryParse(Console.ReadLine(), out var idx) || idx < 1 || idx > products.Count)
        {
            Console.WriteLine(" Invalid selection.");
            return;
        }

        Console.Write("New price: $");
        if (!decimal.TryParse(Console.ReadLine(), out var newPrice))
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        await _service.UpdatePriceAsync(products[idx - 1].Id, newPrice, ct);
        Console.WriteLine("Price updated.");
    }

    private async Task DeleteProductAsync(CancellationToken ct)
    {
        var products = await _service.GetAllProductsAsync(ct);
        PrintProductsWithIndex(products);

        Console.Write("Pick number to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var idx) || idx < 1 || idx > products.Count)
        {
            Console.WriteLine(" Invalid selection.");
            return;
        }

        await _service.DeleteProductAsync(products[idx - 1].Id, ct);
        Console.WriteLine("Product deleted.");
    }

    private static void PrintProducts(string title, IReadOnlyList<Product> products)
    {
        Console.WriteLine($"\n {title} ({products.Count}):");
        if (!products.Any())
        {
            Console.WriteLine("  (none)");
            return;
        }
        foreach (var p in products)
            Console.WriteLine($"  • {p}");
    }

    private static void PrintProductsWithIndex(IReadOnlyList<Product> products)
    {
        Console.WriteLine();
        for (var i = 0; i < products.Count; i++)
            Console.WriteLine($"  {i + 1}. {products[i]}");
    }
}
