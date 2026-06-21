namespace DesktopApp.Models;

public sealed class OrderCreateItemRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
