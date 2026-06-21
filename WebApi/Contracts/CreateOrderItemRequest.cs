namespace WebApi.Contracts;

public sealed record CreateOrderItemRequest(string ProductId, int Quantity);
