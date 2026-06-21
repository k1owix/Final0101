using DataAccess.Models;

namespace DataAccess.Services;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default);

    Task<Order?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default);

    Task<Order> CreateOrderAsync(CreateOrderModel model, CancellationToken cancellationToken = default);

    Task<Order?> UpdateOrderAsync(int orderId, UpdateOrderModel model, CancellationToken cancellationToken = default);
}
