using CleanArchitectureDemo.Application;
using CleanArchitectureDemo.Domain;
using CleanArchitectureDemo.Infrastructure;
using CleanArchitectureDemo.Presentation;

var repository = new InMemoryProductRepository();
var service    = new ProductService(repository);
var ui         = new ConsoleUI(service);

await SeedDataAsync(service);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await ui.RunAsync(cts.Token);

static async Task SeedDataAsync(IProductService svc)
{
    await svc.AddProductAsync("Laptop",              1800m, Category.Electronics);
    await svc.AddProductAsync("Wireless Mouse",        45m, Category.Accessories);
    await svc.AddProductAsync("Monitor",              320m, Category.Electronics);
    await svc.AddProductAsync("Standing Desk",        550m, Category.Furniture);
    await svc.AddProductAsync("USB-C Hub",             35m, Category.Accessories);
    await svc.AddProductAsync("Mechanical Keyboard",  130m, Category.Accessories);
}
