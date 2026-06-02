using HKTech.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Data;

/// <summary>
/// Seed thiết yếu khi app khởi động lần đầu:
///   - Roles: Admin, Customer
///   - Tài khoản Admin mặc định
/// Dữ liệu Categories và Products do nhóm tự thêm qua Admin Panel.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Apply pending migrations tự động
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
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── 3. Categories & Products ──────────────────────────────────────
        // Nhóm tự thêm dữ liệu thật qua Admin Panel:
        //   → Danh mục : /Admin/CategoryAdmin/Create
        //   → Sản phẩm : /Admin/ProductAdmin/Create
    }
}
