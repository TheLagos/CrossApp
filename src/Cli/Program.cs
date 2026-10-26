using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Storage;

bool useFile = args.Any(a => a.Equals("--file", StringComparison.OrdinalIgnoreCase));
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");
string logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");

Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

ICatalogStore store = useFile
    ? new FileCatalogStore(dataPath)
    : new InMemoryCatalogStore(SampleData.Products());

var service = new CatalogService(store);

Console.WriteLine("========================================================");
Console.WriteLine($"Catalog Service Active (Storage: {store.GetType().Name})");
Console.WriteLine("========================================================");

int eventCounter = 0;

void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
{
    string logLine = $"{e.At:HH:mm:ss} | {e.Kind,-8} | ID: {e.Id,-8} | Delta: {e.Delta,5:+0;-0} | Stock After: {e.QuantityAfter,5}";
    Console.WriteLine($" [LOG EVENT] {logLine}");
    File.AppendAllText(logPath, logLine + Environment.NewLine);
}

// 1. Subscribe local method logger and counter lambda
service.Changed += OnCatalogChanged;
service.Changed += (_, _) => eventCounter++;

Console.WriteLine("--> Executing operations WITH active event subscription:");
Product created = service.Add("SKU-888", "Shielded Cable Cat7", "m", 60);
service.Receive(created.Id, 40);
service.Issue(created.Id, 15);

Console.WriteLine($"Total Events Recorded (Counter): {eventCounter}");
Console.WriteLine();

// 2. Unsubscribe logger method
Console.WriteLine("--> Unsubscribing console/file logger (-=) ...");
service.Changed -= OnCatalogChanged;

Console.WriteLine("--> Executing operation AFTER unsubscription (Silent Mode):");
service.Receive(created.Id, 100); // Executed silently by logger, but counter still fires
Console.WriteLine($"Total Events Recorded (Counter): {eventCounter}");
Console.WriteLine($"Current Product Stock Balance  : {service.Find(created.Id)?.Quantity} m");
Console.WriteLine();

// 3. Error Scenario Handling (Broken Invariants -> NO Event Raised)
Console.WriteLine("========================================================");
Console.WriteLine("ERROR SCENARIO: Broken Invariants Must NOT Raise Events");
Console.WriteLine("========================================================");

int eventsBeforeFailure = eventCounter;

try
{
    Console.WriteLine("--> Attempting invalid stock issue (-9999 m) ...");
    service.Issue(created.Id, 9999);
}
catch (Exception ex)
{
    Console.WriteLine($" [CATCH EXPECTED ERROR] {ex.GetType().Name} -> {ex.Message}");
}

Console.WriteLine($"Events count before failure: {eventsBeforeFailure} | after failure: {eventCounter}");
if (eventsBeforeFailure == eventCounter)
{
    Console.WriteLine(" [VERIFIED] No event was raised during invariant failure!");
}
Console.WriteLine();

// 4. Querying using Func<Product, bool> delegate
Console.WriteLine("========================================================");
Console.WriteLine("QUERYING DATA VIA Func<Product, bool> DELEGATE");
Console.WriteLine("========================================================");

Console.WriteLine("--> Filtering products with stock balance <= 50:");
var lowStockItems = service.FindAll(p => p.Quantity <= 50);

foreach (var item in lowStockItems.Take(5))
{
    Console.WriteLine($"    {item.Id,-8} {item.Sku,-10} {item.Name,-25} {item.Quantity,5} {item.Unit}");
}

Console.WriteLine();
Console.WriteLine($"Log file successfully created at: {Path.GetFullPath(logPath)}");
return 0;