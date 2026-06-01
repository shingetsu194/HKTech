# 🚀 HKTech — PC Builder Web App (ASP.NET Core MVC .NET 10)
# Implementation Plan — Đồ án môn Lập trình Web
# Tạo ngày: 2026-06-01

================================================================
TỔNG QUAN DỰ ÁN
================================================================
Tên dự án  : HKTech PC Builder
Công nghệ  : ASP.NET Core MVC .NET 10 + SQL Server + EF Core
Theme      : Undertale-inspired (pixel retro, dark, gold/red)
Thời gian  : 8 tuần (hiện tuần 5-6, còn ~2-3 tuần)

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

PHASE 1 — Tuần 6 (ĐANG LÀM)
  Project Setup + Layout Undertale + Giao diện tĩnh
  - Khởi tạo ASP.NET Core MVC project
  - _Layout.cshtml: Header, Nav, Footer (Undertale theme)
  - CSS Design System (pixel font, gold/red/dark)
  - Views tĩnh: Home, Product List, Build PC, Recommend
  - Seed data placeholder

PHASE 2 — Tuần 6-7
  Models + Database + CRUD + Identity
  - Models: Category, Product, ProductImage, Order, PcBuild, ApplicationUser
  - EF Core Code-First Migration + SQL Server
  - ASP.NET Core Identity (Đăng nhập/Đăng ký)
  - Roles: Admin, Customer
  - CRUD Admin: Sản phẩm + Danh mục + Upload ảnh

PHASE 3 — Tuần 7
  Core Features: Build PC + Recommend + Giỏ hàng
  - Build PC: Chọn linh kiện theo slot
  - Recommend: Lọc combo theo budget/nhu cầu
  - Giỏ hàng (Session) + Đặt hàng (Order/OrderDetail)

PHASE 4 — Tuần 7-8
  Smart Build Intelligence (Tính năng độc đáo)
  - RESTful API endpoints (compatibility, performance)
  - JavaScript real-time update
  - Bottleneck bar + FPS table animation

PHASE 5 — Tuần 8
  Admin Area + Polish + Demo Prep
  - Admin Dashboard (thống kê, quản lý đơn)
  - SEO, responsive mobile
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
Order          : Id, UserId, TotalPrice, Status, CreatedAt
OrderDetail    : Id, OrderId, ProductId, Quantity, UnitPrice
PcBuild        : Id, UserId, Name, TotalPrice, CreatedAt
PcBuildItem    : Id, BuildId, ProductId, Slot

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
