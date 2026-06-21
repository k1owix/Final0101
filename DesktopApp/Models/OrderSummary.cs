namespace DesktopApp.Models;

public sealed class OrderSummary
{
    public int OrderId { get; set; }
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? ClientName { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public IReadOnlyCollection<OrderItemSummary> Items { get; set; } = [];
    public decimal TotalAmount => Items.Sum(item => item.LineTotal);
}
