using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data;

public class ApplicationDbContext : IdentityDbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    public DbSet<ContactUsMessage> Messages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductCategory>()
            .HasKey(productCategory => new { productCategory.CategoryId, productCategory.ProductId });
        // modelBuilder.Entity<ProductCategory>()
        //     .HasOne(productCategory => productCategory.Category)
        //     .WithMany(category => category.ProductCategories)
        //     .HasForeignKey(productCategory => productCategory.CategoryId);
        // modelBuilder.Entity<ProductCategory>()
        //     .HasOne(productCategory => productCategory.Product)
        //     .WithMany(product => product.Category)
        //     .HasForeignKey(productCategory => productCategory.ProductId);
    }
}