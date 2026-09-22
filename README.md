# CrossApp
CrossApp - Cross-Platform Programming Lab.
Domain: Order ( Customer, Product, Order)
Purpose: Processing orders and calculating totals.

## Structure
* `src/Core/` - class library.
* `Core/Dto/`, `Core/Domain/`, `Core/Storage/` - planned.
* `src/Cli/` - console entry point (`ProjectReference` to `Core`).

## Run
dotnet build
dotnet run --project src/Cli

## Env
.NET SDK 10.0, Windows Vista x64

## Size table
Mode                Size (MB) Files
----                --------- -----
Self-Contained          76,85   194
Framework-Dependent      0,19     7
Single-File             70,15     3
Trimmed                 19,33    31