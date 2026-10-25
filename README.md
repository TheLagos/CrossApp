# CrossApp
CrossApp - Cross-Platform Programming Lab.
Domain: Order ( Customer, Product, Order)
Purpose: Processing orders and calculating totals.

## Structure
* `src/Core/` - class library.
  * `Core/Dto/` — Data Transfer Objects (`ProductDto`, `ImportResult`).
  * `Core/Import/` — Resilient file parsers (`ProductCsvImporter`, `ProductJsonImporter`).
* `src/Cli/` - console entry point (`ProjectReference` to `Core`).

## Domain: Warehouse Management
Accounting for goods (`Product`), stock batches (`StockBatch`), storage units (`Warehouse`), and transfer operations (`Movement`).

## Commands

### Build & Basic Run
```bash
dotnet build
dotnet run --project src/Cli