namespace DataAccess.Services;

public sealed record CreateOrderModel(int? UserId, IReadOnlyCollection<CreateOrderItemModel> Items);
