using Microsoft.EntityFrameworkCore;
using ShopFlow.Domain.Interfaces;
using System.Reflection;

namespace ShopFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tự động apply tất cả IEntityTypeConfiguration<T> trong assembly này
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Đăng ký tất cả entity triển khai IEntity từ assembly Domain
        modelBuilder.RegisterAllEntities<IEntity>(typeof(IEntity).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
