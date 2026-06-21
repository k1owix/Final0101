using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services;

public sealed class OrderService : IOrderService
{
    private readonly AppDatabaseContext _context;

    public OrderService(AppDatabaseContext context) => _context = context;

    public async Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await BuildOrdersQuery()
            .AsNoTracking()
            .OrderByDescending(order => order.OrderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await BuildOrdersQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.OrderId == orderId, cancellationToken);
    }

    public async Task<Order> CreateOrderAsync(CreateOrderModel model, CancellationToken cancellationToken = default)
    {
        if (model.Items.Count == 0)
        {
            throw new InvalidOperationException("Order cannot be empty.");
        }

        var productIds = model.Items
            .Select(item => item.ProductId.Trim())
            .Where(productId => !string.IsNullOrWhiteSpace(productId))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (productIds.Length != model.Items.Count)
        {
            throw new InvalidOperationException("Each order item must contain a product identifier.");
        }

        var existingProductIds = await _context.Products
            .AsNoTracking()
            .Where(product => productIds.Contains(product.ProductId))
            .Select(product => product.ProductId)
            .ToListAsync(cancellationToken);

        if (existingProductIds.Count != productIds.Length)
        {
            throw new InvalidOperationException("One or more products were not found in the database.");
        }

        User? user = null;
        if (model.UserId.HasValue)
        {
            user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(existingUser => existingUser.UserId == model.UserId.Value, cancellationToken) ?? throw new InvalidOperationException($"User with id {model.UserId.Value} was not found.");
        }

        var order = new Order
        {
            OrderDate = DateTime.Now,
            UserId = model.UserId,
            CustomerName = user?.FullName,
            ReceiptCode = Random.Shared.Next(100, 1000).ToString(),
            StatusName = "Новый",
            OrderItems = [.. model.Items
                .Select(item => new OrderItem
                {
                    ProductId = item.ProductId.Trim(),
                    Quantity = item.Quantity
                })]
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetOrderAsync(order.OrderId, cancellationToken)
            ?? throw new InvalidOperationException("The created order could not be loaded.");
    }

    public async Task<Order?> UpdateOrderAsync(int orderId, UpdateOrderModel model, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(existingOrder => existingOrder.OrderId == orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        var statusName = model.StatusName.Trim();
        if (string.IsNullOrWhiteSpace(statusName))
        {
            throw new InvalidOperationException("Status name is required.");
        }

        order.DeliveryDate = model.DeliveryDate?.Date;
        order.StatusName = statusName;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetOrderAsync(orderId, cancellationToken);
    }

    private IQueryable<Order> BuildOrdersQuery()
    {
        return _context.Orders
            .Include(order => order.OrderItems)
            .ThenInclude(item => item.Product);
    }
}
