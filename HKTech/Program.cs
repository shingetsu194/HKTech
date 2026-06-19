// ================================================================
// HKTECH — Program.cs
// Phase 2: EF Core + Identity + Session + Admin Area
// ================================================================

using HKTech.Data;
using HKTech.Models;
using HKTech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- MVC ---
builder.Services.AddControllersWithViews();

// --- CartService (Session-based) ---
builder.Services.AddScoped<CartService>();

// --- EF Core + SQL Server ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- ASP.NET Core Identity ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password policy
    options.Password.RequireDigit           = true;
    options.Password.RequiredLength         = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase       = false;
    options.Password.RequireLowercase       = false;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// --- Cookie (Identity redirect paths) ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath         = "/Account/Login";
    options.LogoutPath        = "/Account/Logout";
    options.AccessDeniedPath  = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan    = TimeSpan.FromDays(7);
});

// --- Session (dùng cho giỏ hàng Phase 3) ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name        = ".HKTech.Session";
});

// --- HttpContext (đọc Session cart count trong _Layout) ---
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// --- Error handling ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/Error404");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// --- Session PHẢI trước Authentication ---
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// --- Route: API (attribute routing cho ApiController) ---
app.MapControllers();

// --- Route: Admin Area ---
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// --- Route: Secret Admin Login (hidden URL, not linked on public site) ---
app.MapControllerRoute(
    name: "adminSecret",
    pattern: "hktech-secure-admin",
    defaults: new { controller = "Account", action = "AdminLogin" });

// --- Route: Default ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- Seed dữ liệu khi app khởi động ---
await DbSeeder.SeedAsync(app.Services);

app.Run();
