using System.ComponentModel.DataAnnotations.Schema;

namespace HKTech.Models;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,0)")]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal SubTotal => UnitPrice * Quantity;
}
