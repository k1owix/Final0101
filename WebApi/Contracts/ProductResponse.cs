namespace WebApi.Contracts;

public sealed record ProductResponse(
    string ProductId,
    string Article,
    string Name,
    string Unit,
    decimal Price,
    string? Author,
    string ManufacturerName,
    string CategoryName,
    int Discount,
    int StockQuantity,
    string? Description,
    string? PhotoFileName);
