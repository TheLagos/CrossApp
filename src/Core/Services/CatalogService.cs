using System;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class CatalogService
{
    private readonly ICatalogStore _store;

    public CatalogService(ICatalogStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public Product Add(string sku, string name, string unit, int initialQuantity)
    {
        string generatedId = $"P-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var product = Product.Create(generatedId, sku, name, unit, initialQuantity);
        _store.Add(product);
        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Product with ID '{id}' was not found.");

        product.RegisterArrival(quantity);
        _store.Update(product);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Product with ID '{id}' was not found.");

        product.Issue(quantity);
        _store.Update(product);
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);

    public IReadOnlyList<Product> FilterLowStock(int threshold) =>
        _store.Find(p => p.Quantity <= threshold);
}