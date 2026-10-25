using System;
using System.Collections.Generic;
using Core.Dto;

namespace Core.Domain;

public static class ProductBatchConverter
{
    public static ImportResult<Product> ToEntities(ImportResult<ProductDto> dtoResult)
    {
        var entities = new List<Product>();
        var errors = new List<string>(dtoResult.Errors);

        for (int i = 0; i < dtoResult.Items.Count; i++)
        {
            var dto = dtoResult.Items[i];
            try
            {
                var entity = Product.FromDto(dto);
                entities.Add(entity);
            }
            catch (Exception ex)
            {
                errors.Add($"Domain Invariant Error on DTO [{dto.Sku}]: {ex.Message}");
            }
        }

        return new ImportResult<Product>(entities, errors);
    }
}