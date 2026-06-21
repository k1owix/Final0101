namespace DesktopApp.Models;

public sealed class OrderUpdateRequest
{
    public DateTime? DeliveryDate { get; set; }
    public string StatusName { get; set; } = string.Empty;
}
