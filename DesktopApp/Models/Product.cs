namespace DesktopApp.Models;

public sealed class Product
{
    public string ProductId { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Author { get; set; }
    public string ManufacturerName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Discount { get; set; }
    public int StockQuantity { get; set; }
    public string? Description { get; set; }
    public string? PhotoFileName { get; set; }
    public decimal DiscountedPrice => Price * (1 - Discount / 100m);
    public bool HasDiscount => Discount > 0;
    public bool IsInStock => StockQuantity > 0;
}
