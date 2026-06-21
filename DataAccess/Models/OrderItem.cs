using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

[Table("OrderProducts")]
public sealed class OrderItem
{
    [Key]
    [Column("Id")]
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    [StringLength(20)]
    public string ProductId { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public Order? Order { get; set; }

    public Product? Product { get; set; }
}
