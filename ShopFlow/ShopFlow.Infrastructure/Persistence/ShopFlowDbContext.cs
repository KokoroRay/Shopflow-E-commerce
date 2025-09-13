using Microsoft.EntityFrameworkCore;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Infrastructure.Configurations;

namespace ShopFlow.Infrastructure.Persistence;

public class ShopFlowDbContext : DbContext
{
    public ShopFlowDbContext(DbContextOptions<ShopFlowDbContext> options) : base(options)
    {
    }

    public DbSet<CoreUser> Users { get; set; }
    public DbSet<CatProduct> Products { get; set; }
    public DbSet<CatCategory> Categories { get; set; }
    public DbSet<Cart> Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new CatProductConfiguration());

        // Configure other entities
        modelBuilder.Entity<CoreUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Temporarily disable value object mapping for Email and Phone
            // TODO: Implement proper EF Core value object configuration
            entity.Ignore(e => e.Email);
            entity.Ignore(e => e.Phone);
        });

        // Configure Category entity - temporarily disabled to fix EF configuration issues

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}
