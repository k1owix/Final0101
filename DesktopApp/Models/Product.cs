namespace DesktopApp.Models
{
    public class Product
    {
        public string ArticleNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Author { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Discount { get; set; }
        public int StockQuantity { get; set; }
        public string? Description { get; set; }
        public string? Photo { get; set; }
    }
}