using System.Collections.Generic;

namespace Core.Dto;

public sealed record ImportResult<T>(
    IReadOnlyList<T> Items,
    IReadOnlyList<string> Errors)
{
    public int TotalProcessed => Items.Count + Errors.Count;
    public double ErrorPercentage => TotalProcessed == 0 ? 0.0 : (double)Errors.Count / TotalProcessed * 100;
}