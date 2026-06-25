using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKTech.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    [Column(TypeName = "decimal(18,0)")]
    public decimal TotalPrice { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = OrderStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(300)]
    public string? ShippingAddress { get; set; }

    // Navigation
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

public static class OrderStatus
{
    public const string Pending   = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Shipping  = "Shipping";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";
}
