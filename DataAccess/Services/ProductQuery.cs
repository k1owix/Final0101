namespace DataAccess.Services;

public sealed record ProductQuery(
    string? Search,
    string? Manufacturer,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    bool Descending);
