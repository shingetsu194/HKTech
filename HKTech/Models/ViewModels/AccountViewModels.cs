using System.ComponentModel.DataAnnotations;

namespace HKTech.Models.ViewModels;

// ── Login ─────────────────────────────────────────────────────────────────
public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Nhớ đăng nhập")]
    public bool RememberMe { get; set; }
}

// ── Register ──────────────────────────────────────────────────────────────
public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [MaxLength(150)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

// ── Admin: Product Create/Edit ────────────────────────────────────────────
public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm bắt buộc")]
    [MaxLength(200)]
    [Display(Name = "Tên sản phẩm")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Giá bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
    [Display(Name = "Giá (VNĐ)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Danh mục bắt buộc")]
    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    [Display(Name = "TDP (W)")]
    public int TdpWatt { get; set; }

    [Display(Name = "Benchmark Score")]
    public int BenchmarkScore { get; set; }

    [MaxLength(30)]
    [Display(Name = "Socket")]
    public string? Socket { get; set; }

    [MaxLength(10)]
    [Display(Name = "Loại RAM")]
    public string? RamType { get; set; }

    [MaxLength(10)]
    [Display(Name = "Form Factor")]
    public string? FormFactor { get; set; }

    [Display(Name = "Số lượng tồn kho")]
    public int StockQuantity { get; set; }

    [Display(Name = "Đang bán")]
    public bool IsActive { get; set; } = true;

    // Upload ảnh
    [Display(Name = "Ảnh sản phẩm")]
    public List<IFormFile>? ImageFiles { get; set; }

    // Dùng khi edit — ảnh hiện tại
    public List<ProductImage> ExistingImages { get; set; } = new();
    public List<int> DeleteImageIds { get; set; } = new();
}
