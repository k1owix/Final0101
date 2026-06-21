namespace WebApi.Contracts;

public sealed record CreateOrderRequest(int? UserId, IReadOnlyCollection<CreateOrderItemRequest> Items);
