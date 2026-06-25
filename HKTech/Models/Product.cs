using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKTech.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required, Column(TypeName = "decimal(18,0)")]
    public decimal Price { get; set; }

    // ── Category ──────────────────────────────────────────────
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // ── Smart Build Intelligence fields ───────────────────────
    /// <summary>Công suất tiêu thụ (W) — dùng cho PSU check</summary>
    public int TdpWatt { get; set; }

    /// <summary>Điểm benchmark tổng hợp — dùng cho Bottleneck Analyzer</summary>
    public int BenchmarkScore { get; set; }

    /// <summary>Socket CPU/Mainboard (VD: LGA1851, AM5)</summary>
    [MaxLength(30)]
    public string? Socket { get; set; }

    /// <summary>Loại RAM hỗ trợ (DDR4 | DDR5)</summary>
    [MaxLength(10)]
    public string? RamType { get; set; }

    /// <summary>Form factor case/mainboard (ATX | mATX | ITX)</summary>
    [MaxLength(10)]
    public string? FormFactor { get; set; }

    // ── Inventory ─────────────────────────────────────────────
    public int StockQuantity { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // ── Navigation ────────────────────────────────────────────
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public ICollection<PcBuildItem> PcBuildItems { get; set; } = new List<PcBuildItem>();

    // ── Computed helpers ──────────────────────────────────────
    [NotMapped]
    public string? PrimaryImageUrl => Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                                    ?? Images.FirstOrDefault()?.ImageUrl;
}
