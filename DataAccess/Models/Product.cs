using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

[Table("Products")]
public sealed class Product
{
    [Key]
    [Column("ArticleNumber")]
    [StringLength(20)]
    public string ProductId { get; set; } = string.Empty;

    [NotMapped]
    public string Article
    {
        get => ProductId;
        set => ProductId = value;
    }

    [Required]
    [Column("Name")]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [StringLength(200)]
    public string? Author { get; set; }

    [Column("Manufacturer")]
    [StringLength(200)]
    public string ManufacturerName { get; set; } = string.Empty;

    [Column("Category")]
    [StringLength(200)]
    public string CategoryName { get; set; } = string.Empty;

    public int Discount { get; set; }

    [Column("StockQuantity")]
    public int StockQuantity { get; set; }

    public string? Description { get; set; }

    [Column("Photo")]
    [StringLength(100)]
    public string? PhotoFileName { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
