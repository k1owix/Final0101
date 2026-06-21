using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

[Table("Orders")]
public sealed class Order
{
    [Key]
    public int OrderId { get; set; }

    [NotMapped]
    public int OrderNumber => OrderId;

    public DateTime OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    [Column("CustomerId")]
    public int? UserId { get; set; }

    [StringLength(200)]
    public string? CustomerName { get; set; }

    [Required]
    [Column("PickupCode")]
    [StringLength(10)]
    public string ReceiptCode { get; set; } = string.Empty;

    [Column("Status")]
    [StringLength(50)]
    public string StatusName { get; set; } = string.Empty;

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
