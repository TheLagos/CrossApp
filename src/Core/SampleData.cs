using System.Collections.Generic;
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static List<Product> Products() =>
    [
        Product.Create("P-001", "SKU-001", "Cement M400 25kg", "pcs", 120),
        Product.Create("P-002", "SKU-002", "Building Sand", "t", 18),
        Product.Create("P-003", "SKU-003", "Red Brick", "pcs", 4200),
        Product.Create("P-004", "SKU-004", "Water Paint 10L", "pcs", 36),
        Product.Create("P-005", "SKU-005", "Finish Plaster", "kg", 250),
        Product.Create("P-006", "SKU-006", "Profile CD-60", "m", 800),
        Product.Create("P-007", "SKU-007", "Self-tapping Screws", "pack", 140),
        Product.Create("P-008", "SKU-008", "Primer Penetration", "l", 60),
        Product.Create("P-009", "SKU-009", "Ceramic Tile", "m2", 310),
        Product.Create("P-010", "SKU-010", "Tile Adhesive", "kg", 540),
        Product.Create("P-011", "SKU-011", "Drywall 12.5mm", "sheet", 95),
        Product.Create("P-012", "SKU-012", "Reinforcing Tape", "m", 300),
        Product.Create("P-013", "SKU-013", "Mineral Wool 50mm", "m2", 150),
        Product.Create("P-014", "SKU-014", "Mounting Foam 750ml", "pcs", 80),
        Product.Create("P-015", "SKU-015", "Concrete Cleaner 1L", "l", 45)
    ];
}