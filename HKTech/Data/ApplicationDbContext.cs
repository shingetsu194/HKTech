using HKTech.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HKTech.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── DbSets ────────────────────────────────────────────────────────────
    public DbSet<Category>    Categories  { get; set; }
    public DbSet<Product>     Products    { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order>       Orders      { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<PcBuild>     PcBuilds    { get; set; }
    public DbSet<PcBuildItem> PcBuildItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Category ──────────────────────────────────────────────────────
        builder.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
        });

        // ── Product ───────────────────────────────────────────────────────
        builder.Entity<Product>(e =>
        {
            e.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.CategoryId);
            e.HasIndex(p => p.IsActive);
        });

        // ── ProductImage ──────────────────────────────────────────────────
        builder.Entity<ProductImage>(e =>
        {
            e.HasOne(pi => pi.Product)
             .WithMany(p => p.Images)
             .HasForeignKey(pi => pi.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Order ─────────────────────────────────────────────────────────
        builder.Entity<Order>(e =>
        {
            e.HasOne(o => o.User)
             .WithMany(u => u.Orders)
             .HasForeignKey(o => o.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── OrderDetail ───────────────────────────────────────────────────
        builder.Entity<OrderDetail>(e =>
        {
            e.HasOne(od => od.Order)
             .WithMany(o => o.OrderDetails)
             .HasForeignKey(od => od.OrderId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(od => od.Product)
             .WithMany(p => p.OrderDetails)
             .HasForeignKey(od => od.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── PcBuild ───────────────────────────────────────────────────────
        builder.Entity<PcBuild>(e =>
        {
            e.HasOne(b => b.User)
             .WithMany(u => u.PcBuilds)
             .HasForeignKey(b => b.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── PcBuildItem ───────────────────────────────────────────────────
        builder.Entity<PcBuildItem>(e =>
        {
            e.HasOne(bi => bi.Build)
             .WithMany(b => b.Items)
             .HasForeignKey(bi => bi.BuildId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(bi => bi.Product)
             .WithMany(p => p.PcBuildItems)
             .HasForeignKey(bi => bi.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
