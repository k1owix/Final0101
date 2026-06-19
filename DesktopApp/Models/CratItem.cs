namespace DesktopApp.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Total => Product.Price * Quantity * (1 - Product.Discount / 100m);
    }
}