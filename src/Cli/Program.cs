using System;
using System.IO;
using System.Linq;
using Core.Dto;
using Core.Import;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"File not found: {Path.GetFullPath(path)}");
    return 1;
}

string extension = Path.GetExtension(path).ToLowerInvariant();

ImportResult<ProductDto> result = extension switch
{
    ".csv"  => ProductCsvImporter.Load(path),
    ".json" => ProductJsonImporter.Load(path),
    _ => throw new InvalidOperationException($"Unsupported format '{extension}'. Only .csv and .json are supported.")
};

Console.WriteLine($"CrossApp - Data Import ({extension.ToUpperInvariant()})");
Console.WriteLine(new string('-', 56));
Console.WriteLine($"Loaded records: {result.Items.Count}");

foreach (ProductDto p in result.Items.Take(5))
{
    Console.WriteLine($" {p.Id,-6} {p.Sku,-10} {p.Name,-28} {p.Quantity,5} {p.Unit}");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine($"Skipped lines : {result.Errors.Count}");
    foreach (string error in result.Errors)
    {
        Console.WriteLine($" ! {error}");
    }
}

Console.WriteLine(new string('-', 56));
Console.WriteLine($"STATS | Total: {result.TotalProcessed} | Imported: {result.Items.Count} | Skipped: {result.Errors.Count} | Error Rate: {result.ErrorPercentage:F1}%");

return 0;