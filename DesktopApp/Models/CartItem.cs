namespace DesktopApp.Models;

public sealed class CartItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Total => Product.DiscountedPrice * Quantity;
}
