# ♥ HKTech PC Builder

> *"Nơi mọi linh kiện tìm thấy DETERMINATION của mình."*

Đồ án môn **Lập trình Web** — HUTECH | ASP.NET Core MVC (.NET 10)

---

## 🖥️ Giới thiệu

**HKTech PC Builder** là ứng dụng web giúp người dùng tự build cấu hình PC thông minh theo phong cách **Undertale-inspired** (dark mode, pixel art aesthetic).

### ✨ Tính năng nổi bật

| Tính năng | Mô tả |
|---|---|
| 🛠️ **Tự Build PC** | Chọn linh kiện từng slot (CPU, GPU, RAM, ...) |
| 🛡️ **Compatibility Guard** | Kiểm tra tương thích socket/RAM/PSU real-time |
| 📊 **Bottleneck Analyzer** | Phân tích % bottleneck CPU vs GPU trực quan |
| 🎮 **FPS Estimator** | Ước tính FPS ở 1080p/1440p/4K cho các tựa game phổ biến |
| ⭐ **Gợi ý cấu hình** | Tự động recommend combo linh kiện theo ngân sách & nhu cầu |
| 🛒 **Giỏ hàng** | Thêm toàn bộ build vào giỏ một click |

---

## 🎨 Theme Design

- **Undertale-inspired**: Dark mode, pixel art, DETERMINATION aesthetic
- **Dual-font system**:
  - `Press Start 2P` — cho nhãn ASCII (CPU, RAM, FILTER, SAVE...)
  - `VT323` + `Share Tech Mono` — cho nội dung tiếng Việt
- **Soul colors**: Gold `#FFD700` (SAVE point), Red `#FF4444` (❤ Soul), Green `#44CC44`
- **Pixel art PC** với trái tim Undertale animated trên hero section

---

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core MVC (.NET 10)
- **Database**: SQL Server + Entity Framework Core *(Phase 2)*
- **Frontend**: Vanilla HTML/CSS/JS + Bootstrap 5.3
- **Auth**: ASP.NET Core Identity *(Phase 2)*
- **Fonts**: Google Fonts (Press Start 2P, VT323, Share Tech Mono, Rajdhani)

---

## 📁 Cấu trúc Project

```
HKTech/
├── HKTech/                     # ASP.NET Core project
│   ├── Controllers/            # MVC Controllers
│   ├── Models/                 # Data models (Phase 2)
│   ├── Views/
│   │   ├── Shared/_Layout.cshtml   # Layout chung (Navbar, Footer)
│   │   ├── Home/Index.cshtml       # Trang chủ + Pixel Art PC hero
│   │   ├── Build/Index.cshtml      # Tự Build PC (Smart Build Intelligence)
│   │   ├── Product/Index.cshtml    # Danh sách linh kiện + Filter
│   │   └── Recommend/Index.cshtml  # Gợi ý cấu hình
│   └── wwwroot/css/site.css    # Design system (Undertale theme)
├── IMPLEMENTATION_PLAN.md      # Lộ trình triển khai chi tiết
├── PROGRESS_TRACKER.md         # Theo dõi tiến độ hàng tuần
├── BUG_NOTES.md                # Ghi chú bug & fix
└── README.md                   # File này
```

---

## 🚀 Chạy Project

### Yêu cầu
- .NET 10 SDK
- SQL Server (cho Phase 2)
- Visual Studio 2022 hoặc VS Code

### Chạy development server

```bash
# Clone repo
git clone <repo-url>
cd HKTech

# Chạy với hot-reload (khuyến nghị)
dotnet watch run --project ./HKTech/HKTech.csproj

# Hoặc chạy bình thường
dotnet run --project ./HKTech/HKTech.csproj
```

Mở trình duyệt: **http://localhost:5010**

---

## 📅 Lộ trình phát triển

| Phase | Nội dung | Trạng thái |
|---|---|---|
| **Phase 1** | Frontend UI, Layout, Smart Build Intelligence UI | ✅ Hoàn thành |
| **Phase 2** | Database, Models, EF Core Migration, Admin CRUD, Undertale Login | ✅ Hoàn thành |
| **Phase 3** | Build PC, Cart, Order, Recommend Engine | ✅ Hoàn thành |
| **Phase 4** | Smart Build Intelligence: API, Compatibility Guard, Bottleneck, FPS | ✅ Hoàn thành |
| **Phase 5** | Admin Dashboard, Polish, Demo Prep | 🔄 Đang làm |

---

## 👥 Thành viên

Đồ án môn Lập trình Web — **HUTECH** (2025-2026)

---

> *STAY DETERMINED* ♥
