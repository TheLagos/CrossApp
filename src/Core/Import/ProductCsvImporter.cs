using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Core.Dto;

namespace Core.Import;

public static class ProductCsvImporter
{
    private const char Separator = ';';

    public static ImportResult<ProductDto> Load(string path)
    {
        var items = new List<ProductDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int lineNumber = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (lineNumber == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                continue;

            switch (ParseLine(line))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"line {lineNumber}: {failed.Reason}");
                    break;
            }
        }

        return new ImportResult<ProductDto>(items, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            { Length: < 5 } => new ParseFailed($"expected at least 5 columns, got {parts.Length}"),
            ["", _, _, _, _] or [_, "", _, _, _] => new ParseFailed("SKU or Name cannot be empty"),
            [_, _, _, _, var qty] when !int.TryParse(qty, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q) || q < 0
                => new ParseFailed($"quantity '{qty}' is not a valid non-negative integer"),
            [var id, var sku, var name, var unit, var qty]
                => new ParseOk(new ProductDto(id, sku, name, unit, int.Parse(qty, CultureInfo.InvariantCulture))),
            _ => new ParseFailed($"too many columns: {parts.Length}")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseOk(ProductDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}