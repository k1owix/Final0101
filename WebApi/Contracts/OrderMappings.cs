using DataAccess.Models;

namespace WebApi.Contracts;

public static class OrderMappings
{
    public static OrderResponse ToResponse(Order order)
    {
        return new OrderResponse(
            order.OrderId,
            order.OrderNumber,
            order.OrderDate,
            order.DeliveryDate,
            order.CustomerName,
            order.ReceiptCode,
            order.StatusName,
            [.. order.OrderItems.Select(
                    item => new OrderItemResponse(
                        item.OrderItemId,
                        item.ProductId,
                        item.Product?.ProductName ?? string.Empty,
                        item.Product?.ManufacturerName ?? string.Empty,
                        item.Quantity,
                        (item.Product?.Price ?? 0m) * (1m - (item.Product?.Discount ?? 0) / 100m)))
            ]);
    }
}
