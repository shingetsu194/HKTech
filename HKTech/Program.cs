// ================================================================
// HKTECH — Program.cs
// Phase 1: Session, StaticFiles, MVC routing + Admin Area
// Phase 2 sẽ bổ sung: EF Core, Identity, DbContext
// ================================================================

var builder = WebApplication.CreateBuilder(args);

// --- MVC ---
builder.Services.AddControllersWithViews();

// --- Session (dùng cho giỏ hàng Phase 3) ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name        = ".HKTech.Session";
});

// --- HttpContext (dùng trong _Layout để đọc Session cart count) ---
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// --- Error handling ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// --- Session PHẢI đặt trước Authorization ---
app.UseSession();

app.UseAuthentication(); // Phase 2: Identity
app.UseAuthorization();

// --- Route: Admin Area (Phase 5) ---
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// --- Route: Default ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
