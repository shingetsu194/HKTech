# BUG & FIX NOTES — HKTech PC Builder
# Tạo ngày: 2026-06-01

================================================================
[BUG-001] Font tiếng Việt vỡ trên pixel font
================================================================
Trạng thái : [x] ĐÃ FIX (2026-06-01)
Mức độ     : Medium — Ảnh hưởng hiển thị, không crash app
Trang bị ảnh hưởng: Tất cả trang (Home hero, Build, Product, Recommend...)

Mô tả:
  Font "Press Start 2P" (Google Fonts) không có glyph cho các ký tự
  tiếng Việt có dấu (Ủ, Ạ, Ẩ, Ề, Ổ...). Kết quả là các ký tự này
  bị render bằng fallback font hệ thống trông rất xấu và không đồng nhất.

  Ví dụ lỗi nhìn thấy:
  - "CỦA BẠN." → chữ Ủ, Ạ bị vỡ
  - "CHUẨN."   → chữ Ẩ hiển thị bằng font khác

Giải pháp:
  Tách biệt 2 trường hợp dùng font pixel:
  1. --font-pixel    : "Press Start 2P" → CHỈ dùng cho text ASCII thuần
                       (nav labels: CPU/RAM/GPU, badge: NEW/SAVE, nút tiếng Anh)
  2. --font-pixel-vn : "VT323" → Dùng cho text tiếng Việt cần pixel style
                       (hero title, section headers, dialog box tiếng Việt)

  VT323 là font terminal/pixel từ Google Fonts, hỗ trợ đầy đủ Unicode
  + tiếng Việt, nhìn vẫn rất retro không thua gì Press Start 2P.

Files đã sửa:
  - wwwroot/css/site.css (thêm --font-pixel-vn, sửa hero + section styles)
  - Views/Home/Index.cshtml (hero title dùng font-pixel-vn)
  - Views/Build/Index.cshtml (dialog box, slot labels VN)
  - Views/Shared/_Layout.cshtml (navbar brand)

================================================================
[BUG-002] Form Thêm Sản phẩm không nhận được file ảnh (ImageUrl null)
================================================================
Trạng thái : [x] ĐÃ FIX
Mức độ     : High — Lỗi lưu dữ liệu
Mô tả      : Input file sử dụng `asp-for="ImageUrl"` gây ra lỗi validation/model binding vì `ImageUrl` trong model là string, nhưng form gửi lên `IFormFile`.
Giải pháp  : Đổi thành `name="imageUrl"` thay vì dùng `asp-for`, bắt tham số IFormFile ở Controller.

================================================================
[BUG-003] Mismatch tên hàm bất đồng bộ trong Repository
================================================================
Trạng thái : [x] ĐÃ FIX
Mức độ     : High — Lỗi build CS1061
Mô tả      : Controller gọi `GetAllProducts()` nhưng Repository implement `GetAllAsync()`.
Giải pháp  : Đồng bộ hóa Controller sử dụng await và gọi đúng method `GetAllAsync()`.

================================================================
[BUG-004] Category Dropdown bị trống khi thêm sản phẩm
================================================================
Trạng thái : [x] ĐÃ FIX
Mức độ     : Medium
Mô tả      : Quên gọi database seeding cho Category ở `Program.cs`.
Giải pháp  : Thêm Seed Data cho Category khi app startup.

================================================================
[BUG-005] UI chữ quá nhỏ khó đọc (site.css)
================================================================
Trạng thái : [x] ĐÃ FIX
Mức độ     : Low
Mô tả      : Font pixel-art nhỏ, khó nhìn đối với navbar, slot label và buttons.
Giải pháp  : Tăng `font-size` trong `site.css` (nav-link-hk 0.8->1rem, btn-vn 1->1.25rem, etc.)

================================================================
[PLANNED] Các bug/task cần làm tiếp
================================================================

[ ] Phase 3: CartController — Session
[ ] Phase 4: API compatibility/performance endpoints
[ ] Polish: Responsive mobile — navbar collapse chưa test kỹ

================================================================
