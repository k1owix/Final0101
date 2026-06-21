namespace WebApi.Contracts;

public sealed record OrderItemResponse(
    int OrderItemId,
    string ProductId,
    string ProductName,
    string ManufacturerName,
    int Quantity,
    decimal UnitPrice);
