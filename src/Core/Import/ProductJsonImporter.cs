using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Core.Dto;

namespace Core.Import;

public static class ProductJsonImporter
{
    public static ImportResult<ProductDto> Load(string path)
    {
        var items = new List<ProductDto>();
        var errors = new List<string>();

        try
        {
            string json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var rawItems = JsonSerializer.Deserialize<List<ProductDto>>(json, options) ?? [];

            for (int i = 0; i < rawItems.Count; i++)
            {
                var item = rawItems[i];
                if (string.IsNullOrWhiteSpace(item.Id) || string.IsNullOrWhiteSpace(item.Sku) || string.IsNullOrWhiteSpace(item.Name))
                {
                    errors.Add($"item #{i + 1}: required fields (Id, SKU, Name) are missing");
                    continue;
                }

                if (item.Quantity < 0)
                {
                    errors.Add($"item #{i + 1} ({item.Sku}): quantity cannot be negative ({item.Quantity})");
                    continue;
                }

                items.Add(item);
            }
        }
        catch (Exception ex)
        {
            errors.Add($"JSON processing failed: {ex.Message}");
        }

        return new ImportResult<ProductDto>(items, errors);
    }
}