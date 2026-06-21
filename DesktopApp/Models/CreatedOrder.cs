namespace DesktopApp.Models;

public sealed class CreatedOrder
{
    public int OrderId { get; set; }
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
}
