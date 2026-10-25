using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class InMemoryCatalogStore : ICatalogStore
{
    private readonly Dictionary<string, Product> _items;

    public InMemoryCatalogStore(IEnumerable<Product>? seed = null)
    {
        _items = (seed ?? [])
            .ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<Product> List() => _items.Values.ToList();

    public Product? GetById(string id) => _items.GetValueOrDefault(id);

    public void Add(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_items.ContainsKey(item.Id))
            throw new InvalidOperationException($"Record with ID '{item.Id}' already exists in memory store.");

        _items.Add(item.Id, item);
    }

    public void Update(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_items.ContainsKey(item.Id))
            throw new InvalidOperationException($"Cannot update. Record with ID '{item.Id}' does not exist.");

        _items[item.Id] = item;
    }

    public bool Remove(string id) => _items.Remove(id);

    public IReadOnlyList<Product> Find(Func<Product, bool> predicate) =>
        _items.Values.Where(predicate).ToList();
}