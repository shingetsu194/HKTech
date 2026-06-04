================================================================
   HKTECH PC BUILDER — WEEKLY PROGRESS TRACKER
   Môn: Lập trình Web | Thời gian: 8 tuần
   Tạo ngày: 2026-06-01 | Cập nhật: 2026-06-01
================================================================

LEGEND:
  [x] Hoàn thành
  [/] Đang làm
  [ ] Chưa làm
  [!] Cần chú ý / Bug

================================================================
TUẦN 1-2 — Chọn đề tài & Lên kế hoạch
================================================================
Status  : [x] HOÀN THÀNH
Kết quả : Chọn đề tài PC Builder (HKTech), lên lộ trình 5 giai đoạn
Note    : Đề tài được duyệt

================================================================
TUẦN 3-4 — Nghiên cứu & Phân tích
================================================================
Status  : [x] HOÀN THÀNH
Kết quả : Phân tích yêu cầu, chọn tech stack (.NET 10 + SQL Server)
Note    : Đã xác định tính năng độc đáo "Smart Build Intelligence"

================================================================
TUẦN 5-6 — PHASE 1: Project Setup + Giao diện
================================================================
Status  : [x] HOÀN THÀNH
Bắt đầu : 2026-06-01

  SETUP
  [x] Khởi tạo ASP.NET Core MVC project (.NET 10)
  [x] Cài NuGet packages (EF Core, Identity, ImageSharp)
  [x] Cấu hình appsettings.json (connection string SQL Server)

  LAYOUT & DESIGN (Undertale Theme)
  [x] _Layout.cshtml: Header + Navigation + Footer
  [x] CSS Design System (Press Start 2P font, gold/dark/red)
  [x] Pixel-style border và animation effects
  [x] Responsive (mobile menu)
  [x] Tinh chỉnh font chữ to hơn, dễ nhìn hơn

  VIEWS TĨNH
  [x] Home/Index.cshtml — Hero banner + PC nổi bật + Linh kiện mới
  [x] Product/Index.cshtml — Danh sách linh kiện (có filter)
  [x] Build/Index.cshtml — Slot-based PC builder UI
  [x] Recommend/Index.cshtml — Form chọn nhu cầu/budget

  SEED DATA PLACEHOLDER
  [x] Seed 6 danh mục (CPU, GPU, RAM, PSU, Case, Mainboard)
  [x] Seed ~30 sản phẩm mẫu với benchmark data

Ghi chú tuần này:
  - ___________________________________________
  - ___________________________________________

================================================================
TUẦN 6-7 — PHASE 2: Models + Database + CRUD + Auth
================================================================
Status  : [x] HOÀN THÀNH

  DATABASE & EF CORE
  [x] Tạo ApplicationDbContext
  [x] Models: Category, Product, ProductImage
  [x] Models: Order, OrderDetail, PcBuild, PcBuildItem
  [x] Code-First Migration + Update Database
  [x] Seed data thật vào DB (KATO\MSSQLSERVER01)

  IDENTITY & AUTH
  [x] ApplicationUser (extend IdentityUser)
  [x] Đăng ký / Đăng nhập / Đăng xuất
  [x] Phân quyền: Admin vs Customer
  [x] Trang Profile user
  [x] Đăng nhập Admin phong cách Undertale Battle Screen

  CRUD ADMIN
  [x] Area Admin setup (Giao diện Dark Dashboard)
  [x] ProductController CRUD + upload nhiều ảnh
  [x] CategoryController CRUD
  [x] Validation (server-side + client-side)

Ghi chú tuần này:
  - ___________________________________________
  - ___________________________________________

================================================================
TUẦN 7 — PHASE 3: Core Features (Build PC + Recommend + Cart)
================================================================
Status  : [x] HOÀN THÀNH

  BUILD PC
  [x] BuildController — Hiển thị slot builder
  [x] Chọn linh kiện từng slot (CPU, GPU, RAM, MB, PSU, Case)
  [x] Lưu cấu hình tạm thời (Session)
  [x] Nút "Thêm toàn bộ vào giỏ hàng"
  [x] Lưu build của user đã đăng nhập

  RECOMMEND PC
  [x] RecommendController
  [x] Form: Budget + Mục đích sử dụng
  [x] Thuật toán query combo linh kiện tối ưu từ DB
  [x] Hiển thị top 3 gợi ý cấu hình (Budget / Balanced / Performance)

  GIỎ HÀNG & ĐẶT HÀNG
  [x] CartService (Session-based)
  [x] CartController: Xem, Thêm, Xóa, Cập nhật
  [x] OrderController: Thanh toán + Tạo Order/OrderDetail
  [x] Trang xác nhận đơn hàng
  [x] Lịch sử đơn hàng của user

  BUG FIX & POLISH (thêm vào trong quá trình)
  [x] Cart badge navbar đọc đúng từ CartService (không còn luôn = 0)
  [x] Product page: addToCartQuick, search/sort/filter hoạt động thật
  [x] Recommend View: form submit thật tới controller, render combo cards
  [x] Connection string về đúng instance KATO\MSSQLSERVER01

Ghi chú tuần này:
  - Phase 3 hoàn chỉnh 100%. Tất cả luồng Build → Cart → Order → History hoạt động.
  - Recommend Engine: 3 combo (Budget/Balanced/Performance) với bottleneck, thêm vào giỏ 1 click.

================================================================
TUẦN 7-8 — PHASE 4: Smart Build Intelligence ⭐
================================================================
Status  : [x] HOÀN THÀNH

  API ENDPOINTS
  [x] POST /api/compatibility — Socket, RAM type, PSU wattage, Form factor
  [x] POST /api/performance  — Bottleneck % + FPS 10 game
  [-] GET /api/products/category/{slug} — Không cần (dùng /Build/GetProducts)

  COMPATIBILITY GUARD
  [x] Socket CPU vs Mainboard (LGA1700, AM5...)
  [x] RAM type (DDR4/DDR5) vs Mainboard
  [x] PSU wattage vs Tổng TDP + 20% headroom (yêu cầu có CPU/GPU mới check)
  [x] Case form factor vs Mainboard size (ATX/mATX/ITX)

  BOTTLENECK ANALYZER
  [x] Công thức tính bottleneck từ BenchmarkScore thật trong DB
  [x] Animated progress bar CPU↔GPU (CSS transition 0.6s)
  [x] Gợi ý nâng cấp tự động theo mức chênh lệch

  FPS PERFORMANCE ESTIMATOR
  [x] Seed 41 sản phẩm: 8 CPU + 8 GPU + RAM + MB + PSU + Case + Storage + Cooling
  [x] Bảng FPS 10 game (Valorant, CS2, GTA V, Cyberpunk, Elden Ring...)
  [x] Badge phân loại build (BUDGET / MID-RANGE / HIGH-END / ENTHUSIAST)

  JAVASCRIPT REAL-TIME
  [x] Event listener sau mỗi selectProduct / clearSlot
  [x] AJAX gọi /api/compatibility + /api/performance không reload
  [x] Animate tổng tiền (bump animation CSS)

Ghi chú tuần này:
  - Smart Build Intelligence hoàn chỉnh: Compatibility Guard + Bottleneck + FPS real-time.
  - Seed 41 sản phẩm vào DB với đầy đủ BenchmarkScore, Socket, RamType, FormFactor, TdpWatt.

================================================================
TUẦN 8 — PHASE 5: Admin Dashboard + Polish + Demo
================================================================
Status  : [/] ĐANG THỰC HIỆN

  ADMIN DASHBOARD
  [x] Dashboard: doanh thu, đơn theo trạng thái, sản phẩm sắp hết hàng
  [x] Quản lý đơn hàng: danh sách + filter + chi tiết + cập nhật trạng thái
  [x] Quản lý tồn kho: hiển thị sản phẩm ≤5 trên Dashboard
  [x] Danh sách user: tên, email, role, trạng thái khóa

  POLISH
  [x] SEO: ViewData["Title"] + ["Description"] đồng bộ tất cả trang
  [x] 404 page custom (Undertale style — SANS reference)
  [x] Flash notifications (Success / Warn / Error) đồng bộ user + admin
  [-] Loading skeleton animations (bỏ qua — không cần thiết cho demo)
  [-] Responsive mobile: Bootstrap grid đã xử lý phần lớn

  DEMO PREP
  [ ] Viết script demo (kịch bản bảo vệ)
  [ ] Kiểm tra luồng chính: Build → Cart → Order
  [ ] Kiểm tra tính năng độc đáo: Compatibility + FPS
  [x] Backup database: seed 41 sản phẩm tự động khi chạy app

Ghi chú tuần này:
  - Phase 5 hoàn thành phần Admin + Polish. Còn lại: Demo Script.
  - Admin Panel: Dashboard thống kê, Order CRUD (Pending→Confirmed→Shipping→Delivered), User list.

================================================================
BUGS & ISSUES TRACKING
================================================================

Date       | Bug Description                                      | Status
-----------|------------------------------------------------------|--------
2026-06-03 | Meta tag trùng + sai cú pháp Razor (_Layout)         | [x] Fixed
2026-06-03 | serverBuild PascalCase → undefined khi reload trang  | [x] Fixed
2026-06-03 | Cart Remove thiếu AJAX header → trả HTML thay JSON   | [x] Fixed
2026-06-03 | Detail.cshtml không tồn tại → crash khi click CHI TIẾT| [x] Fixed
2026-06-03 | Admin Dashboard Razor ?? thiếu @()                   | [x] Fixed
2026-06-03 | PSU báo OK khi chưa chọn CPU/GPU                    | [x] Fixed
2026-06-03 | Recommend gaming không yêu cầu GPU                   | [x] Fixed
2026-06-03 | Ghost product checkout (sản phẩm bị xóa)            | [x] Fixed
2026-06-03 | Admin login sign-in customer rồi sign-out ngay       | [x] Fixed
2026-06-03 | Cancel đơn hàng thiếu CSRF protection                | [x] Fixed

================================================================
TỔNG KẾT TIẾN ĐỘ
================================================================

Phase 1 : [100%] ██████████
Phase 2 : [100%] ██████████
Phase 3 : [100%] ██████████
Phase 4 : [100%] ██████████
Phase 5 : [ 85%] █████████░  (Admin+Polish xong, còn Demo Script)
OVERALL : [ 93%] █████████░  (Gần hoàn thành — còn Demo Script)

Cập nhật lần cuối: 2026-06-03

Cập nhật lần cuối: 2026-06-02
