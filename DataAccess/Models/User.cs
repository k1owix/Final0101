using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

[Table("Users")]
public sealed class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [Column("Role")]
    [StringLength(50)]
    public string RoleName { get; set; } = string.Empty;
}
