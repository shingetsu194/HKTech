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
[PLANNED] Các bug/task cần làm tiếp
================================================================

[ ] Phase 2: Kết nối database (connection string SQL Server LocalDB)
[ ] Phase 2: Tạo Migration lần đầu
[ ] Phase 2: Seed data 30 sản phẩm mẫu
[ ] Phase 3: CartController — Session
[ ] Phase 4: API compatibility/performance endpoints
[ ] Polish: Responsive mobile — navbar collapse chưa test kỹ

================================================================
