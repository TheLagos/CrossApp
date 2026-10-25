using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

Console.WriteLine("========================================================");
Console.WriteLine($"Active Catalog Storage Engine: {store.GetType().Name}");
Console.WriteLine("========================================================");

Console.WriteLine($"Total Products in Catalog: {service.All().Count}");
Console.WriteLine();

Console.WriteLine("--> Creating new product via CatalogService...");
Product created = service.Add("SKU-999", "UTP Cable Cat6", "m", 100);
Console.WriteLine($"    Created: {created}");

Console.WriteLine("--> Registering arrival (+50 m)...");
service.Receive(created.Id, 50);
Console.WriteLine($"    Updated: {service.Find(created.Id)}");

Console.WriteLine("--> Issuing stock (-30 m)...");
service.Issue(created.Id, 30);
Console.WriteLine($"    Updated: {service.Find(created.Id)}");

Console.WriteLine();
Console.WriteLine("--> Current Top 5 Catalog Items:");
foreach (var p in service.All().Take(5))
{
    Console.WriteLine($"    {p.Id,-8} {p.Sku,-10} {p.Name,-25} {p.Quantity,5} {p.Unit}");
}

Console.WriteLine();
Console.WriteLine("========================================================");
Console.WriteLine("SERVICE ERROR SCENARIO HANDLING");
Console.WriteLine("========================================================");

TryDo("Receive stock for non-existing product ID", () => service.Receive("NON-EXISTENT-ID", 100));
TryDo("Add duplicate product entity direct to store", () => store.Add(created));

Console.WriteLine();
Console.WriteLine($"Final Store Catalog Count: {service.All().Count}");
return 0;

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" [FAIL] {title}: Exception was NOT thrown!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [SUCCESS] {title}: Catch {ex.GetType().Name} -> {ex.Message}");
    }
}