namespace WebApi.Contracts;

public sealed record UpdateOrderRequest(DateTime? DeliveryDate, string StatusName);
