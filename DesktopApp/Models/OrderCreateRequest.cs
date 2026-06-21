namespace DesktopApp.Models;

public sealed class OrderCreateRequest
{
    public int? UserId { get; set; }
    public IReadOnlyCollection<OrderCreateItemRequest> Items { get; set; } = [];
}
