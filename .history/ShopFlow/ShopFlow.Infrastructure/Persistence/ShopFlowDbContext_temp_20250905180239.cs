using Microsoft.EntityFrameworkCore;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Infrastructure.Persistence;

public class ShopFlowDbContext : DbContext
{
    public ShopFlowDbContext(DbContextOptions<ShopFlowDbContext> options) : base(options)
    {
    }

    // Domain entities only
    public DbSet<CoreUser> Users { get; set; }
    public DbSet<CatProduct> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure domain entities
        modelBuilder.Entity<CoreUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(e => e.Email);
            entity.OwnsOne(e => e.Phone);
        });
        
        modelBuilder.Entity<CatProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}
