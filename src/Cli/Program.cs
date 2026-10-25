using System;
using System.IO;
using Core.Domain;
using Core.Dto;
using Core.Import;

Console.WriteLine("========================================================");
Console.WriteLine("SCENARIO 1: Successful Domain Operations");
Console.WriteLine("========================================================");

Product product = Product.Create("P-001", "SKU-001", "Cement M400 25kg", "pcs", 100);
Console.WriteLine($"Initial Entity : {product}");

product.RegisterArrival(50);
Console.WriteLine($"After Arrival  : {product}");

product.Issue(30);
Console.WriteLine($"After Issue    : {product}");

product.ChangeStatus(ProductStatus.Discontinued);
Console.WriteLine($"After Status   : {product}");
Console.WriteLine();

Console.WriteLine("========================================================");
Console.WriteLine("SCENARIO 2: Domain Invariant Failure Handling");
Console.WriteLine("========================================================");

TryDo("Issue exceeds stock balance", () => 
{
    var activeProduct = Product.Create("P-TEST", "SKU-TEST", "Test Item", "pcs", 50);
    activeProduct.Issue(1000);
});
TryDo("Create product with empty SKU", () => Product.Create("P-002", "", "Sand", "t", 10));
TryDo("Create negative initial stock", () => Product.Create("P-003", "SKU-003", "Red Brick", "pcs", -5));
TryDo("Illegal transition Archived->Active", () => 
{
    var archivedProd = Product.Create("P-004", "SKU-004", "Paint 10L", "pcs", 10, ProductStatus.Archived);
    archivedProd.ChangeStatus(ProductStatus.Active);
});

Console.WriteLine();
Console.WriteLine($"Final Product State Check (Quantity remains untouched): {product}");
Console.WriteLine();

Console.WriteLine("========================================================");
Console.WriteLine("SCENARIO 3: Batch Import & Domain Conversion (Lab 3 + 4)");
Console.WriteLine("========================================================");

string csvPath = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");
if (File.Exists(csvPath))
{
    ImportResult<ProductDto> dtoResult = ProductCsvImporter.Load(csvPath);
    ImportResult<Product> domainResult = ProductBatchConverter.ToEntities(dtoResult);

    Console.WriteLine($"Imported Domain Entities : {domainResult.Items.Count}");
    Console.WriteLine($"Total Domain Errors      : {domainResult.Errors.Count}");

    if (domainResult.Errors.Count > 0)
    {
        Console.WriteLine("Domain Errors Log:");
        foreach (string err in domainResult.Errors)
        {
            Console.WriteLine($" ! {err}");
        }
    }
}
else
{
    Console.WriteLine($"CSV Sample File Not Found: {Path.GetFullPath(csvPath)}");
}

return 0;

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" [FAIL] {title}: Invariant was NOT enforced!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [SUCCESS] {title}: Catch {ex.GetType().Name} -> {ex.Message}");
    }
}