================================================================
   HKTECH PC BUILDER — WEEKLY PROGRESS TRACKER
   Môn: Lập trình Web | Thời gian: 8 tuần
   Tạo ngày: 2026-06-01 | Cập nhật: 2026-06-02
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
  [x] Pixel SVG hearts (navbar, footer brand, footer bottom)
  [x] Navbar: LOG IN gold button, bỏ ĐĂNG KÝ

  VIEWS TĨNH
  [x] Home/Index.cshtml — Hero banner + PC nổi bật + Linh kiện mới
  [x] Product/Index.cshtml — Danh sách linh kiện (có filter)
  [x] Build/Index.cshtml — Slot-based PC builder UI
  [x] Recommend/Index.cshtml — Form chọn nhu cầu/budget

  SEED DATA PLACEHOLDER
  [x] Seed 6 danh mục (CPU, GPU, RAM, PSU, Case, Mainboard)
  [x] Seed ~30 sản phẩm mẫu với benchmark data

Ghi chú tuần này:
  - Slogan mới: "YOU HAVE DETERMINATION. WE HAVE THE PARTS."
  - Pixel art SVG heart thay thế ♥ text toàn bộ giao diện

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
  - Phase 2 hoàn tất bởi teammate, merge vào master
  - DB instance: LAPTOP-O833CQCQ\SQLEXPRESS (máy teammate)

================================================================
TUẦN 7 — PHASE 3: Core Features (Build PC + Recommend + Cart)
================================================================
Status  : [x] HOÀN THÀNH (2026-06-02)

  BUILD PC
  [x] BuildController — Session-based slot management
  [x] GET /Build/GetProducts?slot=xxx — load sản phẩm từ DB
  [x] POST /Build/SelectProduct — lưu slot vào Session
  [x] POST /Build/RemoveSlot — xóa slot khỏi Session
  [x] POST /Build/ClearAll — xóa toàn bộ build
  [x] POST /Build/SaveBuild — lưu build cho user đã đăng nhập
  [x] Build/Index.cshtml — AJAX kết nối API thật, restore từ Session
  [x] SAVE BUILD panel inline + toast notifications

  RECOMMEND PC
  [x] RecommendController — thuật toán query combo từ DB
  [x] Phân bổ budget theo useCase (gaming/esports/workstation/streaming/office)
  [x] Tạo 3 combo: BUDGET (×0.75) / BALANCED (×1.00) / PERFORMANCE (×1.25)
  [x] Tính bottleneck % CPU↔GPU từ BenchmarkScore
  [x] Socket compatibility filter (CPU ↔ Mainboard)
  [x] RecommendCombo ViewModel với BadgeColor

  GIỎ HÀNG & ĐẶT HÀNG
  [x] CartService — Session-based CRUD
  [x] CartController: Index, Add, Remove, Update, Clear, AddBuild
  [x] CartService đăng ký Scoped DI trong Program.cs
  [x] Cart/Index.cshtml — bảng items, AJAX qty update, remove, summary
  [x] OrderController — Checkout (GET+POST), Confirm, History, Cancel
  [x] Order/Checkout.cshtml — form giao hàng + cart summary sidebar
  [x] Order/Confirm.cshtml — xác nhận đơn hàng + danh sách item
  [x] Order/History.cshtml — lịch sử + cancel đơn Pending

Ghi chú tuần này:
  - CartService.AddBuildToCartAsync() cho phép add cả build vào cart 1 click
  - OrderController lấy giá sản phẩm mới nhất từ DB lúc tạo Order (tránh price drift)
  - Order.Cancel() chỉ cho phép hủy khi Status == Pending

================================================================
TUẦN 7-8 — PHASE 4: Smart Build Intelligence ⭐
================================================================
Status  : [ ] CHƯA BẮT ĐẦU

  API ENDPOINTS
  [ ] GET /api/compatibility — Kiểm tra tương thích (JSON)
  [ ] GET /api/performance — Bottleneck + FPS estimate (JSON)
  [x] GET /Build/GetProducts?slot={slug} — Filter sản phẩm theo danh mục (DONE Phase 3)

  COMPATIBILITY GUARD
  [ ] Socket CPU vs Mainboard (real-time AJAX)
  [ ] RAM type (DDR4/DDR5) vs Mainboard
  [ ] PSU wattage vs Tổng TDP
  [ ] Case form factor vs Mainboard size

  BOTTLENECK ANALYZER
  [ ] Công thức tính bottleneck từ BenchmarkScore (real API)
  [x] Animated progress bar CPU↔GPU (client-side, dùng BenchmarkScore)
  [ ] Gợi ý nâng cấp tự động

  FPS PERFORMANCE ESTIMATOR
  [ ] Seed benchmark data cho ~25 CPU + ~25 GPU
  [ ] Bảng FPS cho 10 game phổ biến (real data)
  [x] Badge phân loại build (client-side placeholder)

  JAVASCRIPT REAL-TIME
  [x] Event listener khi thay đổi linh kiện
  [x] AJAX gọi API + cập nhật UI không reload
  [x] Animate số tiền tổng (bump effect)

Ghi chú tuần này:
  - ___________________________________________
  - ___________________________________________

================================================================
TUẦN 8 — PHASE 5: Admin Dashboard + Polish + Demo
================================================================
Status  : [ ] CHƯA BẮT ĐẦU

  ADMIN DASHBOARD
  [ ] Trang tổng quan (thống kê đơn hàng, doanh thu)
  [ ] Quản lý đơn hàng (xem, cập nhật trạng thái)
  [ ] Quản lý tồn kho
  [ ] Danh sách user

  POLISH
  [ ] SEO: title tags, meta description tất cả trang
  [ ] Responsive mobile hoàn chỉnh
  [ ] Loading skeleton animations
  [x] Toast notifications (thành công/lỗi) — Build & Cart
  [ ] 404 page custom (Undertale style)

  RECOMMEND VIEW
  [ ] Recommend/Index.cshtml — hiển thị 3 combo từ controller

  DEMO PREP
  [ ] Viết script demo (kịch bản bảo vệ)
  [ ] Kiểm tra luồng chính: Build → Cart → Order
  [ ] Kiểm tra tính năng độc đáo: Compatibility + FPS
  [ ] Backup database với seed data đủ

Ghi chú tuần này:
  - ___________________________________________
  - ___________________________________________

================================================================
BUGS & ISSUES TRACKING
================================================================

Date       | Bug Description                              | Status
-----------|----------------------------------------------|----------
2026-06-02 | Build .exe locked khi app đang chạy          | Expected (dotnet watch)
           |                                              |
           |                                              |

================================================================
TỔNG KẾT TIẾN ĐỘ
================================================================

Phase 1 : [100%] ██████████  (Setup + Giao diện Undertale theme)
Phase 2 : [100%] ██████████  (DB + Auth + Admin CRUD)
Phase 3 : [100%] ██████████  (Build + Recommend + Cart + Order)
Phase 4 : [ 20%] ██░░░░░░░░  (Client-side done, API endpoints còn)
Phase 5 : [  0%] ░░░░░░░░░░  (Chưa bắt đầu)
OVERALL : [ 70%] ███████░░░  (Phase 3 hoàn tất!)

Cập nhật lần cuối: 2026-06-02
