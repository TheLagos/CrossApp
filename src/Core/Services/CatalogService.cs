using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class CatalogService
{
    private readonly ICatalogStore _store;

    public event EventHandler<CatalogChangedEventArgs>? Changed;

    public CatalogService(ICatalogStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    private void OnChanged(Product p, CatalogChangeKind kind, int delta)
    {
        Changed?.Invoke(this, new CatalogChangedEventArgs(p.Id, kind, delta, p.Quantity));
    }

    public Product Add(string sku, string name, string unit, int initialQuantity)
    {
        string generatedId = $"P-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var product = Product.Create(generatedId, sku, name, unit, initialQuantity);

        _store.Add(product);
        OnChanged(product, CatalogChangeKind.Added, initialQuantity);

        return product;
    }

    public void Receive(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Product with ID '{id}' was not found.");

        product.RegisterArrival(quantity);
        _store.Update(product);

        OnChanged(product, CatalogChangeKind.Received, quantity);
    }

    public void Issue(string id, int quantity)
    {
        var product = _store.GetById(id)
            ?? throw new InvalidOperationException($"Product with ID '{id}' was not found.");

        product.Issue(quantity);
        _store.Update(product);

        OnChanged(product, CatalogChangeKind.Issued, -quantity);
    }

    public bool Remove(string id)
    {
        var product = _store.GetById(id);
        if (product == null) return false;

        int previousQuantity = product.Quantity;
        bool removed = _store.Remove(id);

        if (removed)
        {
            OnChanged(product, CatalogChangeKind.Removed, -previousQuantity);
        }

        return removed;
    }

    public IReadOnlyList<Product> All() => _store.List();

    public Product? Find(string id) => _store.GetById(id);

    public IReadOnlyList<Product> FindAll(Func<Product, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}