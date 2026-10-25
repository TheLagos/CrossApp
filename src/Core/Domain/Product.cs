using System;
using Core.Dto;

namespace Core.Domain;

public sealed class Product
{
    private int _quantity;

    public string Id { get; }
    public string Sku { get; }
    public string Name { get; }
    public string Unit { get; }
    public ProductStatus Status { get; private set; }
    public int Quantity => _quantity;

    private Product(string id, string sku, string name, string unit, int quantity, ProductStatus status)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Unit = unit;
        _quantity = quantity;
        Status = status;
    }

    public static Product Create(
        string id, 
        string sku, 
        string name, 
        string unit, 
        int quantity, 
        ProductStatus status = ProductStatus.Active)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Product ID is required.", nameof(id));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be empty.", nameof(sku));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Measurement unit is required.", nameof(unit));

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Initial stock quantity cannot be negative.");

        return new Product(
            id.Trim(), 
            sku.Trim().ToUpperInvariant(), 
            name.Trim(), 
            unit.Trim(), 
            quantity, 
            status);
    }

    public void RegisterArrival(int amount)
    {
        if (Status == ProductStatus.Archived)
            throw new InvalidOperationException($"Cannot register arrival for archived product '{Sku}'.");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Arrival amount must be strictly greater than zero.");

        _quantity += amount;
    }

    public void Issue(int amount)
    {
        if (Status != ProductStatus.Active)
            throw new InvalidOperationException($"Cannot issue product '{Sku}' in '{Status}' state. Product must be Active.");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Issue amount must be strictly greater than zero.");

        if (amount > _quantity)
            throw new InvalidOperationException($"Cannot issue {amount} {Unit} of '{Sku}'. Current stock balance is {_quantity}.");

        _quantity -= amount;
    }

    public void ChangeStatus(ProductStatus newStatus)
    {
        bool isValidTransition = (Status, newStatus) switch
        {
            (ProductStatus.Active, ProductStatus.Discontinued) => true,
            (ProductStatus.Active, ProductStatus.Archived) => true,
            (ProductStatus.Discontinued, ProductStatus.Archived) => true,
            (ProductStatus.Discontinued, ProductStatus.Active) => true,
            _ => false
        };

        if (!isValidTransition)
            throw new InvalidOperationException($"Invalid status transition from '{Status}' to '{newStatus}' for product '{Sku}'.");

        Status = newStatus;
    }

    public ProductDto ToDto() => new(Id, Sku, Name, Unit, Quantity, $"Status: {Status}");

    public static Product FromDto(ProductDto dto) =>
        Create(dto.Id, dto.Sku, dto.Name, dto.Unit, dto.Quantity);

    public override string ToString() => $"{Id} [{Sku}] {Name} - {Quantity} {Unit} ({Status})";
}