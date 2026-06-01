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
TUẦN 5-6 — PHASE 1: Project Setup + Giao diện (ĐANG LÀM)
================================================================
Status  : [/] ĐANG THỰC HIỆN
Bắt đầu : 2026-06-01

  SETUP
  [/] Khởi tạo ASP.NET Core MVC project (.NET 10)
  [ ] Cài NuGet packages (EF Core, Identity, ImageSharp)
  [ ] Cấu hình appsettings.json (connection string SQL Server)

  LAYOUT & DESIGN (Undertale Theme)
  [ ] _Layout.cshtml: Header + Navigation + Footer
  [ ] CSS Design System (Press Start 2P font, gold/dark/red)
  [ ] Pixel-style border và animation effects
  [ ] Responsive (mobile menu)

  VIEWS TĨNH
  [ ] Home/Index.cshtml — Hero banner + PC nổi bật + Linh kiện mới
  [ ] Product/Index.cshtml — Danh sách linh kiện (có filter)
  [ ] Build/Index.cshtml — Slot-based PC builder UI
  [ ] Recommend/Index.cshtml — Form chọn nhu cầu/budget

  SEED DATA PLACEHOLDER
  [ ] Seed 6 danh mục (CPU, GPU, RAM, PSU, Case, Mainboard)
  [ ] Seed ~30 sản phẩm mẫu với benchmark data

Ghi chú tuần này:
  - ___________________________________________
  - ___________________________________________

================================================================
TUẦN 6-7 — PHASE 2: Models + Database + CRUD + Auth
================================================================
Status  : [ ] CHƯA BẮT ĐẦU

  DATABASE & EF CORE
  [ ] Tạo ApplicationDbContext
  [ ] Models: Category, Product, ProductImage
  [ ] Models: Order, OrderDetail, PcBuild, PcBuildItem
  [ ] Code-First Migration + Update Database
  [ ] Seed data thật vào DB

  IDENTITY & AUTH
  [ ] ApplicationUser (extend IdentityUser)
  [ ] Đăng ký / Đăng nhập / Đăng xuất
  [ ] Phân quyền: Admin vs Customer
  [ ] Trang Profile user

  CRUD ADMIN
  [ ] Area Admin setup
  [ ] ProductController CRUD + upload nhiều ảnh
  [ ] CategoryController CRUD
  [ ] Validation (server-side + client-side)

Ghi chú tuần này:
  - ___________________________________________
  - ___________________________________________

================================================================
TUẦN 7 — PHASE 3: Core Features (Build PC + Recommend + Cart)
================================================================
Status  : [ ] CHƯA BẮT ĐẦU

  BUILD PC
  [ ] BuildController — Hiển thị slot builder
  [ ] Chọn linh kiện từng slot (CPU, GPU, RAM, MB, PSU, Case)
  [ ] Lưu cấu hình tạm thời (Session)
  [ ] Nút "Thêm toàn bộ vào giỏ hàng"
  [ ] Lưu build của user đã đăng nhập

  RECOMMEND PC
  [ ] RecommendController
  [ ] Form: Budget + Mục đích sử dụng
  [ ] Thuật toán query combo linh kiện tối ưu từ DB
  [ ] Hiển thị top 3 gợi ý cấu hình

  GIỎ HÀNG & ĐẶT HÀNG
  [ ] CartService (Session-based)
  [ ] CartController: Xem, Thêm, Xóa, Cập nhật
  [ ] OrderController: Thanh toán + Tạo Order/OrderDetail
  [ ] Trang xác nhận đơn hàng
  [ ] Lịch sử đơn hàng của user

Ghi chú tuần này:
  - ___________________________________________
  - ___________________________________________

================================================================
TUẦN 7-8 — PHASE 4: Smart Build Intelligence ⭐
================================================================
Status  : [ ] CHƯA BẮT ĐẦU

  API ENDPOINTS
  [ ] GET /api/compatibility — Kiểm tra tương thích (JSON)
  [ ] GET /api/performance — Bottleneck + FPS estimate (JSON)
  [ ] GET /api/products/category/{slug} — Filter sản phẩm

  COMPATIBILITY GUARD
  [ ] Socket CPU vs Mainboard
  [ ] RAM type (DDR4/DDR5) vs Mainboard
  [ ] PSU wattage vs Tổng TDP
  [ ] Case form factor vs Mainboard size

  BOTTLENECK ANALYZER
  [ ] Công thức tính bottleneck từ BenchmarkScore
  [ ] Animated progress bar CPU↔GPU
  [ ] Gợi ý nâng cấp tự động

  FPS PERFORMANCE ESTIMATOR
  [ ] Seed benchmark data cho ~25 CPU + ~25 GPU
  [ ] Bảng FPS cho 10 game phổ biến
  [ ] Badge phân loại build

  JAVASCRIPT REAL-TIME
  [ ] Event listener khi thay đổi linh kiện
  [ ] AJAX gọi API + cập nhật UI không reload
  [ ] Animate số tiền tổng

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
  [ ] Toast notifications (thành công/lỗi)
  [ ] 404 page custom (Undertale style)

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

Date       | Bug Description                  | Status
-----------|----------------------------------|--------
           |                                  |
           |                                  |
           |                                  |

================================================================
TỔNG KẾT TIẾN ĐỘ
================================================================

Phase 1 : [  0%] ░░░░░░░░░░
Phase 2 : [  0%] ░░░░░░░░░░
Phase 3 : [  0%] ░░░░░░░░░░
Phase 4 : [  0%] ░░░░░░░░░░
Phase 5 : [  0%] ░░░░░░░░░░
OVERALL : [  5%] ░░░░░░░░░░  (Lên kế hoạch xong)

Cập nhật lần cuối: 2026-06-01
