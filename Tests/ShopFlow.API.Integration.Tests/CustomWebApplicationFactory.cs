using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ShopFlow.Infrastructure.Persistence;

namespace ShopFlow.API.Integration.Tests;

/// <summary>
/// Custom Web Application Factory for Vietnamese marketplace integration testing
/// Provides isolated test environment with in-memory database
/// </summary>
public class VietnameseMarketplaceWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<ShopFlowDbContext>));
            services.RemoveAll(typeof(ShopFlowDbContext));

            // Add in-memory database for testing
            services.AddDbContext<ShopFlowDbContext>(options =>
            {
                options.UseInMemoryDatabase("VietnameseMarketplaceTestDb");
                options.EnableSensitiveDataLogging();
            });

            // Override logging for testing
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));

            // Ensure database is created and seeded for Vietnamese marketplace testing
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ShopFlowDbContext>();

            dbContext.Database.EnsureCreated();
            SeedVietnameseMarketplaceTestData(dbContext);
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Seeds test data for Vietnamese marketplace scenarios
    /// </summary>
    private static void SeedVietnameseMarketplaceTestData(ShopFlowDbContext context)
    {
        // Skip seeding if data already exists
        if (context.Products.Any() || context.Categories.Any())
            return;

        // Seed Vietnamese marketplace test categories
        var vietnameseCategories = new[]
        {
            new Domain.Entities.CatCategory(
                Domain.ValueObjects.CategoryName.FromDisplayName("Thời trang Việt Nam"),
                Domain.ValueObjects.CategorySlug.FromName("thoi-trang-viet-nam"),
                Domain.Enums.CategoryStatus.Active,
                null)
            { Id = 1 },

            new Domain.Entities.CatCategory(
                Domain.ValueObjects.CategoryName.FromDisplayName("Ẩm thực truyền thống"),
                Domain.ValueObjects.CategorySlug.FromName("am-thuc-truyen-thong"),
                Domain.Enums.CategoryStatus.Active,
                null)
            { Id = 2 },

            new Domain.Entities.CatCategory(
                Domain.ValueObjects.CategoryName.FromDisplayName("Thủ công mỹ nghệ"),
                Domain.ValueObjects.CategorySlug.FromName("thu-cong-my-nghe"),
                Domain.Enums.CategoryStatus.Active,
                null)
            { Id = 3 }
        };

        context.Categories.AddRange(vietnameseCategories);

        // Seed Vietnamese marketplace test products
        var vietnameseProducts = new[]
        {
            new Domain.Entities.CatProduct
            {
                Id = 1,
                Name = Domain.ValueObjects.ProductName.FromDisplayName("Áo dài truyền thống"),
                Slug = Domain.ValueObjects.ProductSlug.FromString("ao-dai-truyen-thong"),
                Price = new Domain.ValueObjects.Money(850000, "VND"),
                Status = 1, // Approved
                CategoryId = 1,
                VendorId = 101,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new Domain.Entities.CatProduct
            {
                Id = 2,
                Name = Domain.ValueObjects.ProductName.FromDisplayName("Phở bò Hà Nội"),
                Slug = Domain.ValueObjects.ProductSlug.FromString("pho-bo-ha-noi"),
                Price = new Domain.ValueObjects.Money(65000, "VND"),
                Status = 1, // Approved
                CategoryId = 2,
                VendorId = 102,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new Domain.Entities.CatProduct
            {
                Id = 3,
                Name = Domain.ValueObjects.ProductName.FromDisplayName("Vietnamese Traditional Dress"),
                Slug = Domain.ValueObjects.ProductSlug.FromString("vietnamese-traditional-dress"),
                Price = new Domain.ValueObjects.Money(35.50m, "USD"),
                Status = 0, // Pending approval
                CategoryId = 1,
                VendorId = 103,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new Domain.Entities.CatProduct
            {
                Id = 4,
                Name = Domain.ValueObjects.ProductName.FromDisplayName("Nón lá Việt Nam"),
                Slug = Domain.ValueObjects.ProductSlug.FromString("non-la-viet-nam"),
                Price = new Domain.ValueObjects.Money(120000, "VND"),
                Status = 2, // Rejected
                CategoryId = 3,
                VendorId = 101,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(vietnameseProducts);

        // Seed test users for Vietnamese marketplace
        var testUsers = new[]
        {
            new Domain.Entities.CoreUser
            {
                Id = 101,
                FirstName = "Nguyen",
                LastName = "Van A",
                Email = Domain.ValueObjects.Email.FromEmailAddress("vendor1@shopflow.vn"),
                PhoneNumber = Domain.ValueObjects.PhoneNumber.FromPhoneString("+84901234567"),
                Status = 1, // Active
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new Domain.Entities.CoreUser
            {
                Id = 102,
                FirstName = "Tran",
                LastName = "Thi B",
                Email = Domain.ValueObjects.Email.FromEmailAddress("vendor2@shopflow.vn"),
                PhoneNumber = Domain.ValueObjects.PhoneNumber.FromPhoneString("+84907654321"),
                Status = 1, // Active
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new Domain.Entities.CoreUser
            {
                Id = 103,
                FirstName = "Le",
                LastName = "Van C",
                Email = Domain.ValueObjects.Email.FromEmailAddress("vendor3@shopflow.vn"),
                PhoneNumber = Domain.ValueObjects.PhoneNumber.FromPhoneString("+84912345678"),
                Status = 1, // Active
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new Domain.Entities.CoreUser
            {
                Id = 999,
                FirstName = "Admin",
                LastName = "User",
                Email = Domain.ValueObjects.Email.FromEmailAddress("admin@shopflow.vn"),
                PhoneNumber = Domain.ValueObjects.PhoneNumber.FromPhoneString("+84900000000"),
                Status = 1, // Active
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(testUsers);

        context.SaveChanges();
    }
}
