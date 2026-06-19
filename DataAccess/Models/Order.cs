using DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? CustomerId { get; set; }

        [StringLength(100)]
        public string? CustomerName { get; set; }

        [Required]
        [StringLength(3)]
        public string? PickupCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string? Status { get; set; } = "Новый";

        [ForeignKey("CustomerId")]
        public virtual User? Customer { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}