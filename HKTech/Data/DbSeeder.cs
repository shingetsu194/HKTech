using HKTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HKTech.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        // ── 1. Roles ──────────────────────────────────────────────────────
        foreach (var role in new[] { "Admin", "Customer" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ── 2. Admin account ──────────────────────────────────────────────
        const string adminEmail    = "admin@hktech.vn";
        const string adminPassword = "Admin@123456";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                FullName       = "HKTech Admin",
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── 3. Categories ─────────────────────────────────────────────────
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "CPU",         Slug = "cpu",       Icon = "🧠", Description = "Bộ vi xử lý Intel và AMD" },
                new Category { Name = "GPU / VGA",   Slug = "gpu",       Icon = "🎮", Description = "Card đồ họa rời" },
                new Category { Name = "RAM",         Slug = "ram",       Icon = "💾", Description = "Bộ nhớ DDR4 / DDR5" },
                new Category { Name = "Mainboard",   Slug = "mainboard", Icon = "🧩", Description = "Bo mạch chủ Intel và AMD" },
                new Category { Name = "PSU / Nguồn", Slug = "psu",       Icon = "⚡", Description = "Nguồn máy tính 80+ Gold/Bronze" },
                new Category { Name = "Case / Vỏ",  Slug = "case",      Icon = "🗄️", Description = "Vỏ case ATX, mATX, ITX" },
                new Category { Name = "Ổ cứng",     Slug = "storage",   Icon = "💿", Description = "SSD NVMe, SATA và HDD" },
                new Category { Name = "Tản nhiệt",  Slug = "cooling",   Icon = "❄️", Description = "Tản nhiệt khí và tản nhiệt nước AIO" }
            );
            await db.SaveChangesAsync();
        }

        // ── 4. Products ───────────────────────────────────────────────────
        // Lấy CategoryId từ DB
        var cat = await db.Categories.ToDictionaryAsync(c => c.Slug);

        // Nếu có scraped_products.json → luôn chạy (chỉ thêm sản phẩm mới, bỏ qua trùng)
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "scraped_products.json");
        if (File.Exists(jsonPath))
        {
            Console.WriteLine("[Seeder] Tìm thấy scraped_products.json — đồng bộ sản phẩm từ phongvu.vn");
            await SeedFromScrapedJson(db, cat, jsonPath);
            return;
        }

        // Fallback: seed mẫu cứng — chỉ khi chưa có sản phẩm nào
        if (await db.Products.AnyAsync()) return;

        // ── BenchmarkScore scale: 0–3000 ──────────────────────────────────
        // Budget CPU ~300-500 | Mid ~700-1000 | High ~1100-1600 | Flagship ~1800-2500
        // Budget GPU ~200-400 | Mid ~700-1100 | High ~1300-1800 | Flagship ~2200-3000

        var products = new List<Product>
        {
            // ══════════════════════ CPU ══════════════════════
            new()
            {
                Name = "Intel Core i3-14100F", CategoryId = cat["cpu"].Id,
                Price = 2_490_000, Description = "CPU Intel 4 nhân 8 luồng, tốt cho văn phòng và gaming nhẹ.",
                BenchmarkScore = 420, TdpWatt = 58, Socket = "LGA1700",
                StockQuantity = 20, IsActive = true,
            },
            new()
            {
                Name = "Intel Core i5-13600KF", CategoryId = cat["cpu"].Id,
                Price = 5_490_000, Description = "CPU Intel 14 nhân 20 luồng, cân bằng hiệu năng/giá tốt nhất.",
                BenchmarkScore = 820, TdpWatt = 125, Socket = "LGA1700",
                StockQuantity = 15, IsActive = true,
            },
            new()
            {
                Name = "Intel Core i7-14700KF", CategoryId = cat["cpu"].Id,
                Price = 8_990_000, Description = "CPU Intel 20 nhân 28 luồng, hiệu năng mạnh cho gaming và render.",
                BenchmarkScore = 1250, TdpWatt = 181, Socket = "LGA1700",
                StockQuantity = 10, IsActive = true,
            },
            new()
            {
                Name = "Intel Core i9-14900KF", CategoryId = cat["cpu"].Id,
                Price = 14_990_000, Description = "CPU Intel flagship 24 nhân, đỉnh cao hiệu năng single/multi-thread.",
                BenchmarkScore = 1950, TdpWatt = 253, Socket = "LGA1700",
                StockQuantity = 5, IsActive = true,
            },
            new()
            {
                Name = "AMD Ryzen 5 7600X", CategoryId = cat["cpu"].Id,
                Price = 4_990_000, Description = "CPU AMD 6 nhân 12 luồng AM5, IPC cao xuất sắc cho gaming.",
                BenchmarkScore = 810, TdpWatt = 105, Socket = "AM5",
                StockQuantity = 18, IsActive = true,
            },
            new()
            {
                Name = "AMD Ryzen 7 7700X", CategoryId = cat["cpu"].Id,
                Price = 8_490_000, Description = "CPU AMD 8 nhân 16 luồng AM5, hiệu năng gaming và đa nhiệm xuất sắc.",
                BenchmarkScore = 1180, TdpWatt = 105, Socket = "AM5",
                StockQuantity = 12, IsActive = true,
            },
            new()
            {
                Name = "AMD Ryzen 9 7900X", CategoryId = cat["cpu"].Id,
                Price = 13_990_000, Description = "CPU AMD 12 nhân 24 luồng, lý tưởng cho workstation và streaming.",
                BenchmarkScore = 1750, TdpWatt = 170, Socket = "AM5",
                StockQuantity = 6, IsActive = true,
            },
            new()
            {
                Name = "AMD Ryzen 9 7950X", CategoryId = cat["cpu"].Id,
                Price = 22_990_000, Description = "CPU AMD 16 nhân 32 luồng, flagship AM5 cho chuyên gia đồ họa và 3D.",
                BenchmarkScore = 2400, TdpWatt = 170, Socket = "AM5",
                StockQuantity = 3, IsActive = true,
            },

            // ══════════════════════ GPU ══════════════════════
            new()
            {
                Name = "NVIDIA GeForce GTX 1650 4GB", CategoryId = cat["gpu"].Id,
                Price = 3_290_000, Description = "Card đồ họa entry-level, chơi game esports và văn phòng.",
                BenchmarkScore = 320, TdpWatt = 75,
                StockQuantity = 25, IsActive = true,
            },
            new()
            {
                Name = "NVIDIA GeForce RTX 3060 12GB", CategoryId = cat["gpu"].Id,
                Price = 7_990_000, Description = "Card tầm trung tốt nhất, chơi 1080p Ultra mượt và hỗ trợ Ray Tracing.",
                BenchmarkScore = 880, TdpWatt = 170,
                StockQuantity = 15, IsActive = true,
            },
            new()
            {
                Name = "NVIDIA GeForce RTX 3070 8GB", CategoryId = cat["gpu"].Id,
                Price = 11_490_000, Description = "Card tầm cao chinh phục 1440p Ultra, DLSS 2.0.",
                BenchmarkScore = 1150, TdpWatt = 220,
                StockQuantity = 10, IsActive = true,
            },
            new()
            {
                Name = "NVIDIA GeForce RTX 4070 12GB", CategoryId = cat["gpu"].Id,
                Price = 15_990_000, Description = "Ada Lovelace thế hệ mới, DLSS 3 và hiệu năng 1440p đỉnh cao.",
                BenchmarkScore = 1550, TdpWatt = 200,
                StockQuantity = 8, IsActive = true,
            },
            new()
            {
                Name = "NVIDIA GeForce RTX 4080 Super 16GB", CategoryId = cat["gpu"].Id,
                Price = 28_990_000, Description = "Flagship tầm cao, 4K gaming mượt mà và AI acceleration.",
                BenchmarkScore = 2250, TdpWatt = 320,
                StockQuantity = 4, IsActive = true,
            },
            new()
            {
                Name = "NVIDIA GeForce RTX 4090 24GB", CategoryId = cat["gpu"].Id,
                Price = 45_990_000, Description = "Card đồ họa mạnh nhất thế giới, 4K 144fps và render chuyên nghiệp.",
                BenchmarkScore = 2950, TdpWatt = 450,
                StockQuantity = 2, IsActive = true,
            },
            new()
            {
                Name = "AMD Radeon RX 6700 XT 12GB", CategoryId = cat["gpu"].Id,
                Price = 7_490_000, Description = "GPU AMD tầm trung, 1080p/1440p xuất sắc với giá cả hợp lý.",
                BenchmarkScore = 860, TdpWatt = 230,
                StockQuantity = 12, IsActive = true,
            },
            new()
            {
                Name = "AMD Radeon RX 7800 XT 16GB", CategoryId = cat["gpu"].Id,
                Price = 13_990_000, Description = "GPU AMD RDNA3, 1440p Ultra hoàn hảo, VRAM 16GB cho AI.",
                BenchmarkScore = 1480, TdpWatt = 263,
                StockQuantity = 7, IsActive = true,
            },

            // ══════════════════════ RAM ══════════════════════
            new()
            {
                Name = "Kingston Fury Beast DDR4 16GB 3200MHz", CategoryId = cat["ram"].Id,
                Price = 1_090_000, Description = "RAM DDR4 16GB (2×8GB), tần số 3200MHz CL16, tản nhiệt nhôm.",
                BenchmarkScore = 0, TdpWatt = 4, RamType = "DDR4",
                StockQuantity = 50, IsActive = true,
            },
            new()
            {
                Name = "Corsair Vengeance DDR4 32GB 3600MHz", CategoryId = cat["ram"].Id,
                Price = 2_290_000, Description = "RAM DDR4 32GB (2×16GB), 3600MHz CL18, hiệu năng gaming cao.",
                BenchmarkScore = 0, TdpWatt = 5, RamType = "DDR4",
                StockQuantity = 30, IsActive = true,
            },
            new()
            {
                Name = "G.Skill Trident Z5 DDR5 16GB 6000MHz", CategoryId = cat["ram"].Id,
                Price = 2_490_000, Description = "RAM DDR5 16GB (2×8GB), 6000MHz CL36, thế hệ mới hiệu năng cao.",
                BenchmarkScore = 0, TdpWatt = 6, RamType = "DDR5",
                StockQuantity = 25, IsActive = true,
            },
            new()
            {
                Name = "Kingston Fury Beast DDR5 32GB 5200MHz", CategoryId = cat["ram"].Id,
                Price = 4_290_000, Description = "RAM DDR5 32GB (2×16GB), 5200MHz, lý tưởng cho workstation.",
                BenchmarkScore = 0, TdpWatt = 7, RamType = "DDR5",
                StockQuantity = 20, IsActive = true,
            },

            // ══════════════════════ MAINBOARD ══════════════════════
            new()
            {
                Name = "ASUS Prime B760M-A DDR4", CategoryId = cat["mainboard"].Id,
                Price = 2_690_000, Description = "Mainboard Intel B760, mATX, DDR4, phù hợp build tầm trung tiết kiệm.",
                BenchmarkScore = 0, TdpWatt = 15, Socket = "LGA1700", RamType = "DDR4", FormFactor = "mATX",
                StockQuantity = 15, IsActive = true,
            },
            new()
            {
                Name = "MSI PRO Z790-P DDR5", CategoryId = cat["mainboard"].Id,
                Price = 5_990_000, Description = "Mainboard Intel Z790 ATX DDR5, hỗ trợ OC và PCIe 5.0 NVMe.",
                BenchmarkScore = 0, TdpWatt = 20, Socket = "LGA1700", RamType = "DDR5", FormFactor = "ATX",
                StockQuantity = 10, IsActive = true,
            },
            new()
            {
                Name = "ASUS ROG Strix Z790-E DDR5", CategoryId = cat["mainboard"].Id,
                Price = 10_990_000, Description = "Mainboard Intel Z790 flagship, 20+1 VRM, DDR5 8000MHz+.",
                BenchmarkScore = 0, TdpWatt = 25, Socket = "LGA1700", RamType = "DDR5", FormFactor = "ATX",
                StockQuantity = 5, IsActive = true,
            },
            new()
            {
                Name = "MSI MAG B650 Tomahawk WiFi", CategoryId = cat["mainboard"].Id,
                Price = 3_990_000, Description = "Mainboard AMD B650 ATX DDR5, WiFi 6E, lý tưởng cho Ryzen 7000.",
                BenchmarkScore = 0, TdpWatt = 18, Socket = "AM5", RamType = "DDR5", FormFactor = "ATX",
                StockQuantity = 12, IsActive = true,
            },
            new()
            {
                Name = "ASUS ROG Crosshair X670E Hero", CategoryId = cat["mainboard"].Id,
                Price = 12_990_000, Description = "Mainboard AMD X670E flagship, PCIe 5.0 full, cho Ryzen 9 7950X.",
                BenchmarkScore = 0, TdpWatt = 30, Socket = "AM5", RamType = "DDR5", FormFactor = "ATX",
                StockQuantity = 3, IsActive = true,
            },

            // ══════════════════════ PSU ══════════════════════
            // Với PSU, TdpWatt = công suất đầu ra (W)
            new()
            {
                Name = "Cooler Master MWE 550W Bronze", CategoryId = cat["psu"].Id,
                Price = 1_190_000, Description = "Nguồn 550W 80+ Bronze, phù hợp build budget không card rời mạnh.",
                BenchmarkScore = 0, TdpWatt = 550,
                StockQuantity = 30, IsActive = true,
            },
            new()
            {
                Name = "Seasonic Focus GX-750W Gold", CategoryId = cat["psu"].Id,
                Price = 2_490_000, Description = "Nguồn 750W 80+ Gold, full modular, hỗ trợ hầu hết cấu hình gaming.",
                BenchmarkScore = 0, TdpWatt = 750,
                StockQuantity = 20, IsActive = true,
            },
            new()
            {
                Name = "Corsair RM850e 850W Gold", CategoryId = cat["psu"].Id,
                Price = 3_290_000, Description = "Nguồn 850W 80+ Gold ATX 3.0, semi-modular, cho RTX 4070/4080.",
                BenchmarkScore = 0, TdpWatt = 850,
                StockQuantity = 15, IsActive = true,
            },
            new()
            {
                Name = "be quiet! Dark Power 13 1000W Platinum", CategoryId = cat["psu"].Id,
                Price = 5_990_000, Description = "Nguồn 1000W 80+ Platinum, cực êm, cho build flagship RTX 4090.",
                BenchmarkScore = 0, TdpWatt = 1000,
                StockQuantity = 8, IsActive = true,
            },

            // ══════════════════════ CASE ══════════════════════
            // FormFactor = kích thước mainboard lớn nhất mà case chứa được
            new()
            {
                Name = "Xigmatek Gemini Plus mATX", CategoryId = cat["case"].Id,
                Price = 890_000, Description = "Vỏ case mATX nhỏ gọn, 2 quạt ARGB, kính cường lực, giá rẻ.",
                BenchmarkScore = 0, TdpWatt = 0, FormFactor = "mATX",
                StockQuantity = 30, IsActive = true,
            },
            new()
            {
                Name = "NZXT H510 ATX Mid Tower", CategoryId = cat["case"].Id,
                Price = 1_990_000, Description = "Vỏ case ATX minimalist, kính cường lực, cable management tốt.",
                BenchmarkScore = 0, TdpWatt = 0, FormFactor = "ATX",
                StockQuantity = 20, IsActive = true,
            },
            new()
            {
                Name = "Lian Li PC-O11 Dynamic ATX", CategoryId = cat["case"].Id,
                Price = 3_290_000, Description = "Vỏ case ATX full kính, dual chamber, hỗ trợ 360mm AIO, RGB showcase.",
                BenchmarkScore = 0, TdpWatt = 0, FormFactor = "ATX",
                StockQuantity = 12, IsActive = true,
            },
            new()
            {
                Name = "Fractal Design Terra ITX", CategoryId = cat["case"].Id,
                Price = 2_990_000, Description = "Vỏ case ITX nhỏ gọn cao cấp, aluminum, hỗ trợ card đồ họa dài.",
                BenchmarkScore = 0, TdpWatt = 0, FormFactor = "ITX",
                StockQuantity = 8, IsActive = true,
            },

            // ══════════════════════ STORAGE ══════════════════════
            new()
            {
                Name = "Samsung 870 EVO SATA SSD 500GB", CategoryId = cat["storage"].Id,
                Price = 1_090_000, Description = "SSD SATA 2.5\" 500GB, đọc 560MB/s, bền bỉ và ổn định.",
                BenchmarkScore = 0, TdpWatt = 2,
                StockQuantity = 40, IsActive = true,
            },
            new()
            {
                Name = "WD Black SN850X NVMe 1TB", CategoryId = cat["storage"].Id,
                Price = 2_490_000, Description = "SSD NVMe PCIe 4.0 1TB, đọc 7300MB/s, tốc độ đỉnh cho gaming.",
                BenchmarkScore = 0, TdpWatt = 5,
                StockQuantity = 25, IsActive = true,
            },
            new()
            {
                Name = "Samsung 990 Pro NVMe 2TB", CategoryId = cat["storage"].Id,
                Price = 4_290_000, Description = "SSD NVMe PCIe 4.0 2TB, đọc 7450MB/s, bảo vệ nhiệt tốt nhất.",
                BenchmarkScore = 0, TdpWatt = 6,
                StockQuantity = 15, IsActive = true,
            },
            new()
            {
                Name = "Seagate Barracuda HDD 2TB", CategoryId = cat["storage"].Id,
                Price = 1_290_000, Description = "HDD 3.5\" SATA 2TB 7200RPM, lưu trữ dung lượng lớn giá rẻ.",
                BenchmarkScore = 0, TdpWatt = 8,
                StockQuantity = 35, IsActive = true,
            },

            // ══════════════════════ COOLING ══════════════════════
            new()
            {
                Name = "Cooler Master Hyper 212 Black", CategoryId = cat["cooling"].Id,
                Price = 490_000, Description = "Tản nhiệt khí 120mm, hỗ trợ Intel LGA1700 và AMD AM5, giá tốt.",
                BenchmarkScore = 0, TdpWatt = 5,
                StockQuantity = 40, IsActive = true,
            },
            new()
            {
                Name = "Noctua NH-D15 Chromax Black", CategoryId = cat["cooling"].Id,
                Price = 2_590_000, Description = "Tản nhiệt khí dual-tower tốt nhất thị trường, hỗ trợ TDP 250W+.",
                BenchmarkScore = 0, TdpWatt = 8,
                StockQuantity = 15, IsActive = true,
            },
            new()
            {
                Name = "Corsair iCUE H100i AIO 240mm", CategoryId = cat["cooling"].Id,
                Price = 2_990_000, Description = "Tản nước AIO 240mm, RGB, pump êm, tốt cho i7/Ryzen 7.",
                BenchmarkScore = 0, TdpWatt = 18,
                StockQuantity = 20, IsActive = true,
            },
            new()
            {
                Name = "DeepCool LT720 AIO 360mm", CategoryId = cat["cooling"].Id,
                Price = 3_490_000, Description = "Tản nước AIO 360mm, 3×120mm fan, hỗ trợ TDP 300W, cho i9/Ryzen 9.",
                BenchmarkScore = 0, TdpWatt = 25,
                StockQuantity = 12, IsActive = true,
            },
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();
    }

    // ── Seed từ scraped_products.json (dữ liệu thật từ phongvu.vn) ───────────
    private static async Task SeedFromScrapedJson(
        ApplicationDbContext db,
        Dictionary<string, Category> catMap,
        string jsonPath)
    {
        var json    = await File.ReadAllTextAsync(jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var items   = JsonSerializer.Deserialize<List<ScrapedProductDto>>(json, options);
        if (items == null || items.Count == 0) return;

        // Lấy danh sách tên sản phẩm đã có để tránh trùng lặp
        var existingNames = (await db.Products.Select(p => p.Name).ToListAsync()).ToHashSet();

        var newProducts = new List<Product>();

        foreach (var item in items)
        {
            if (!catMap.TryGetValue(item.CategorySlug ?? "", out var category)) continue;
            if (string.IsNullOrWhiteSpace(item.Name)) continue;
            if (existingNames.Contains(item.Name)) continue; // bỏ qua nếu đã có

            var product = new Product
            {
                Name           = item.Name,
                Description    = item.Name,
                Price          = item.Price,
                CategoryId     = category.Id,
                Socket         = item.Socket,
                RamType        = item.RamType,
                FormFactor     = item.FormFactor,
                TdpWatt        = item.TdpWatt,
                BenchmarkScore = item.BenchmarkScore,
                StockQuantity  = item.StockQuantity > 0 ? item.StockQuantity : 15,
                IsActive       = item.IsActive,
            };

            if (!string.IsNullOrEmpty(item.ImageFile))
            {
                product.Images = new List<ProductImage>
                {
                    new() { ImageUrl = $"/images/products/{item.ImageFile}", IsPrimary = true }
                };
            }

            newProducts.Add(product);
        }

        if (newProducts.Count == 0)
        {
            Console.WriteLine("[Seeder] Không có sản phẩm mới để thêm.");
            return;
        }

        db.Products.AddRange(newProducts);
        await db.SaveChangesAsync();

        Console.WriteLine($"[Seeder] Đã thêm {newProducts.Count} sản phẩm mới (bỏ qua {existingNames.Count} đã có).");
    }

    // DTO khớp với JSON output của scraper.py
    private class ScrapedProductDto
    {
        public string?  Name           { get; set; }
        public decimal  Price          { get; set; }
        public string?  CategorySlug   { get; set; }
        public string?  ImageFile      { get; set; }
        public string?  Socket         { get; set; }
        public string?  RamType        { get; set; }
        public string?  FormFactor     { get; set; }
        public int      TdpWatt        { get; set; }
        public int      BenchmarkScore { get; set; }
        public int      StockQuantity  { get; set; }
        public bool     IsActive       { get; set; }
    }
}
