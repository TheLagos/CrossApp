using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileCatalogStore : ICatalogStore
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    private readonly Dictionary<string, Product> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _filePath;
    private bool _loaded;

    public FileCatalogStore(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));

        _filePath = Path.GetFullPath(filePath);
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;

        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);
            var dtos = JsonSerializer.Deserialize<List<ProductDto>>(json, JsonOpts) ?? [];
            foreach (var dto in dtos)
            {
                var product = Product.FromDto(dto);
                _cache[product.Id] = product;
            }
        }

        _loaded = true;
    }

    private void Flush()
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        List<ProductDto> dtos = _cache.Values.Select(p => p.ToDto()).ToList();
        string json = JsonSerializer.Serialize(dtos, JsonOpts);
        File.WriteAllText(_filePath, json);
    }

    public IReadOnlyList<Product> List()
    {
        EnsureLoaded();
        return _cache.Values.ToList();
    }

    public Product? GetById(string id)
    {
        EnsureLoaded();
        return _cache.GetValueOrDefault(id);
    }

    public void Add(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();

        if (_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Record with ID '{item.Id}' already exists in file store.");

        _cache.Add(item.Id, item);
        Flush();
    }

    public void Update(Product item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();

        if (!_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Cannot update. Record with ID '{item.Id}' does not exist.");

        _cache[item.Id] = item;
        Flush();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();
        if (!_cache.Remove(id)) return false;

        Flush();
        return true;
    }

    public IReadOnlyList<Product> Find(Func<Product, bool> predicate)
    {
        EnsureLoaded();
        return _cache.Values.Where(predicate).ToList();
    }
}