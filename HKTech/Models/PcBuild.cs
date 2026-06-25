using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKTech.Models;

public class PcBuild
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Name { get; set; } = "My Build";

    [Column(TypeName = "decimal(18,0)")]
    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<PcBuildItem> Items { get; set; } = new List<PcBuildItem>();
}

public class PcBuildItem
{
    public int Id { get; set; }

    public int BuildId { get; set; }
    public PcBuild Build { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>Slot key: cpu | mainboard | ram | gpu | storage | psu | case | cooling</summary>
    [MaxLength(20)]
    public string Slot { get; set; } = string.Empty;
}
