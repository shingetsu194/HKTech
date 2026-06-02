# 🚀 HKTech — PC Builder Web App (ASP.NET Core MVC .NET 10)
# Implementation Plan — Đồ án môn Lập trình Web
# Tạo ngày: 2026-06-01 | Cập nhật: 2026-06-02

================================================================
TỔNG QUAN DỰ ÁN
================================================================
Tên dự án  : HKTech PC Builder
Công nghệ  : ASP.NET Core MVC .NET 10 + SQL Server + EF Core
Theme      : Undertale-inspired (pixel retro, dark, gold/red)
Thời gian  : 8 tuần | Tiến độ: 70% (Phase 3 hoàn tất)
GitHub     : https://github.com/shingetsu194/HKTech
Branch     : master (dev) → main (production)

================================================================
TÍNH NĂNG ĐỘC ĐÁO — "SMART BUILD INTELLIGENCE"
================================================================
Đây là điểm phân biệt HKTech với MỌI web PC Builder thông thường.

1. COMPATIBILITY GUARD (Real-time cảnh báo tương thích)
   - CPU socket vs Mainboard socket (LGA1851, AM5...)
   - RAM type DDR4/DDR5 vs Mainboard hỗ trợ
   - PSU Wattage vs Tổng TDP toàn bộ build
   - Case form factor vs Mainboard size (ATX/mATX/ITX)

2. BOTTLENECK ANALYZER (Phân tích Bottleneck)
   - Tính % bottleneck giữa CPU và GPU đang chọn
   - Hiển thị thanh progress bar: CPU ████░░ GPU
   - Gợi ý nâng cấp phù hợp

3. FPS PERFORMANCE ESTIMATOR (Ước tính FPS)
   - Dựa trên CPU + GPU + RAM, ước tính FPS ở 1080p/1440p/4K
   - Bảng game phổ biến: Valorant, CS2, GTA V, Cyberpunk 2077...
   - Badge: "Mượt Gaming" / "Workstation" / "Rendering Pro"

Tất cả cập nhật REAL-TIME bằng AJAX (không reload trang).

================================================================
TIMELINE 5 GIAI ĐOẠN
================================================================

PHASE 1 — Tuần 6 ✅ HOÀN THÀNH
  Project Setup + Layout Undertale + Giao diện tĩnh
  - Khởi tạo ASP.NET Core MVC project (.NET 10)
  - _Layout.cshtml: Header, Nav, Footer (Undertale theme)
  - CSS Design System (pixel font, gold/red/dark)
  - Views tĩnh: Home, Product List, Build PC, Recommend
  - Pixel SVG heart icons (8×6 Undertale grid)
  - Slogan: "YOU HAVE DETERMINATION. WE HAVE THE PARTS."

PHASE 2 — Tuần 6-7 ✅ HOÀN THÀNH
  Models + Database + CRUD + Identity
  - Models: Category, Product, ProductImage, Order, PcBuild, ApplicationUser
  - EF Core Code-First Migration + SQL Server
  - ASP.NET Core Identity (Đăng nhập/Đăng ký)
  - Roles: Admin, Customer
  - CRUD Admin: Sản phẩm + Danh mục + Upload ảnh
  - DB instance: LAPTOP-O833CQCQ\SQLEXPRESS

PHASE 3 — Tuần 7 ✅ HOÀN THÀNH (2026-06-02)
  Core Features: Build PC + Recommend + Giỏ hàng + Đặt hàng

  Controllers:
  - BuildController: Session-based slot builder, GetProducts (AJAX),
    SelectProduct, RemoveSlot, ClearAll, SaveBuild (auth required)
  - CartService (Scoped DI): Add, Remove, Update, Clear, AddBuild
  - CartController: Index, Add, Update, Remove, Clear, AddBuild
  - RecommendController: Budget allocation algo, 3 combo tiers,
    bottleneck calc, socket/RAM compatibility filter
  - OrderController: Checkout (GET+POST), Confirm, History, Cancel

  Views:
  - Build/Index.cshtml: AJAX product loading, session restore,
    toast notifications, SAVE BUILD panel
  - Cart/Index.cshtml: Item table, AJAX qty update/remove, summary
  - Order/Checkout.cshtml: Shipping form + cart sidebar
  - Order/Confirm.cshtml: Order confirmation + item list
  - Order/History.cshtml: Order history + cancel button

PHASE 4 — Tuần 7-8 🔲 ĐANG LÊN KẾ HOẠCH
  Smart Build Intelligence (Tính năng độc đáo)
  - RESTful API: /api/compatibility, /api/performance
  - JavaScript real-time compatibility check (socket, RAM, PSU)
  - Bottleneck bar animation dựa trên BenchmarkScore (real API)
  - FPS estimation table với dữ liệu thật
  - Seed benchmark data cho 25+ CPU + 25+ GPU

PHASE 5 — Tuần 8 🔲 CHƯA BẮT ĐẦU
  Admin Area + Polish + Demo Prep
  - Admin Dashboard (thống kê đơn hàng, doanh thu)
  - Quản lý đơn hàng (cập nhật trạng thái)
  - Recommend/Index.cshtml View (hiển thị 3 combo)
  - SEO, responsive mobile hoàn chỉnh
  - 404 page custom (Undertale style)
  - Demo script chuẩn bị bảo vệ

================================================================
DATABASE SCHEMA
================================================================

Category       : Id, Name, Slug, Icon, Description
Product        : Id, Name, Price, Description, CategoryId,
                 TdpWatt, BenchmarkScore, Socket, RamType,
                 FormFactor, StockQuantity, IsActive
ProductImage   : Id, ProductId, ImageUrl, IsPrimary
ApplicationUser: (IdentityUser) + FullName, Address, PhoneNumber
Order          : Id, UserId, TotalPrice, Status, CreatedAt, ShippingAddress
OrderDetail    : Id, OrderId, ProductId, Quantity, UnitPrice
PcBuild        : Id, UserId, Name, TotalPrice, CreatedAt
PcBuildItem    : Id, BuildId, ProductId, Slot

================================================================
SERVICES & DI REGISTRATION (Program.cs)
================================================================

builder.Services.AddScoped<CartService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

================================================================
KEY API ENDPOINTS (Phase 3)
================================================================

GET  /Build                        → Build page
GET  /Build/GetProducts?slot=cpu   → JSON: { success, products[] }
POST /Build/SelectProduct          → JSON: { success, message }
POST /Build/RemoveSlot             → JSON: { success }
POST /Build/ClearAll               → 200 OK
POST /Build/SaveBuild              → JSON: { success, message }

GET  /Cart                         → Cart page (List<CartItem>)
POST /Cart/Add                     → JSON: { success, cartCount }
POST /Cart/Remove                  → JSON: { success, cartTotal, cartCount }
POST /Cart/Update                  → JSON: { success, lineTotal, cartTotal }
POST /Cart/Clear                   → Redirect
POST /Cart/AddBuild                → JSON: { success, message, cartCount }

GET  /Order/Checkout               → Checkout form
POST /Order/Checkout               → Create Order, clear cart, redirect Confirm
GET  /Order/Confirm/{id}           → Confirmation page
GET  /Order/History                → User's order history
POST /Order/Cancel/{id}            → JSON: { success, message }

POST /Recommend                    → 3 combo suggestions (ViewBag.Combos)

================================================================
NUGET PACKAGES
================================================================
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Identity.UI
SixLabors.ImageSharp

================================================================
UNDERTALE THEME DESIGN TOKENS
================================================================
Background  : #0D0D0D (The Underground)
Surface      : #1A1A1A (Dark cave walls)
Primary      : #FFD700 (SAVE point gold)
Accent Red   : #FF4444 (DETERMINATION / Frisk's soul)
Accent Blue  : #4488FF (Sans / Patience)
Text Primary : #FFFFFF
Text Muted   : #888888
Border       : 2px solid #FFD700 (pixel style)
Font Headers : "Press Start 2P" (Google Fonts - pixel)
Font Body    : "Courier New" hoặc monospace
Heart Shape  : SVG 8×6 pixel grid (Undertale SOUL style)
