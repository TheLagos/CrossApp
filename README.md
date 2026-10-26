# CrossApp
CrossApp - Cross-Platform Programming Lab.
Domain: Order ( Customer, Product, Order)
Purpose: Processing orders and calculating totals.

## Structure
* `data/` — Sample input and generated persistent files (`sample.csv`, `catalog.json`).
* `src/Core/` - class library.
  * `Core/Dto/` — Data Transfer Objects (`ProductDto`, `ImportResult`).
  * `Core/Import/` — Resilient file parsers (`ProductCsvImporter`, `ProductJsonImporter`).
  * `Core/Domain/` — Encapsulated Domain Entities (`Product`, `ProductStatus`, `ProductBatchConverter`).
  * `Core/Abstractions/` — Storage Interfaces (`ICatalogStore`).
  * `Core/Services/` — Business Application Services (`CatalogService`).
  * `Core/Events/` — Domain Events Infrastructure (`CatalogChangeKind`, `CatalogChangedEventArgs`).
* `src/Cli/` - console entry point (`ProjectReference` to `Core`).

## Domain: Warehouse Management
Accounting for goods (`Product`), stock batches (`StockBatch`), storage units (`Warehouse`), and transfer operations (`Movement`).

## Domain Invariants (`Product` Entity)
1. **Required String Identifiers**: `Id`, `Sku`, `Name`, and `Unit` cannot be null or whitespace (`ArgumentException`).
2. **Non-negative Stock Quantity**: Initial stock and current balance cannot be less than zero (`ArgumentOutOfRangeException`).
3. **Positive Operation Amounts**: Stock arrival and issue operations require amount > 0 (`ArgumentOutOfRangeException`).
4. **Sufficient Stock Balance**: Issue amount cannot exceed current stock balance (`InvalidOperationException`).
5. **State Transition Rules**: Status changes are constrained via pattern matching state transitions (`InvalidOperationException`).

## Storage Engine Modes
* **In-Memory Store (Default)**: Uses transient `InMemoryCatalogStore` initialized with `SampleData`. Data is reset on exit.
* **File Store (`--file`)**: Uses persistent `FileCatalogStore` pointing to `data/catalog.json`. Persists state across runs.

## Commands

### Build & Basic Run
```bash
dotnet build
dotnet run --project src/Cli