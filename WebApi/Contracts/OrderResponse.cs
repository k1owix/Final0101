namespace WebApi.Contracts;

public sealed record OrderResponse(
    int OrderId,
    int OrderNumber,
    DateTime OrderDate,
    DateTime? DeliveryDate,
    string? ClientName,
    string ReceiptCode,
    string StatusName,
    IReadOnlyCollection<OrderItemResponse> Items);
