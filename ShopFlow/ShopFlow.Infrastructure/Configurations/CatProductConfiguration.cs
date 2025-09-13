using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.Enums;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Infrastructure.Configurations;

/// <summary>
/// EF Core configuration for CatProduct entity with Vietnamese marketplace support
/// Includes value object mappings, indexes for performance, and Vietnamese text collation
/// </summary>
public class CatProductConfiguration : IEntityTypeConfiguration<CatProduct>
{
    public void Configure(EntityTypeBuilder<CatProduct> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);

        // Configure ProductName value object
        builder.Property(e => e.Name)
            .HasConversion(
                name => name.Value,
                value => ProductName.FromDisplayName(value))
            .HasMaxLength(ProductName.MAX_LENGTH)
            .IsRequired()
            .HasColumnName("Name")
            .UseCollation("Vietnamese_CI_AS"); // Case-insensitive Vietnamese collation

        // Configure ProductSlug value object  
        builder.Property(e => e.Slug)
            .HasConversion(
                slug => slug.Value,
                value => ProductSlug.FromString(value))
            .HasMaxLength(ProductSlug.MAX_LENGTH)
            .IsRequired()
            .HasColumnName("Slug")
            .UseCollation("SQL_Latin1_General_CP1_CI_AS"); // Standard ASCII for URLs

        // Configure ProductStatus enum
        builder.Property(e => e.Status)
            .HasConversion<byte>()
            .IsRequired()
            .HasColumnName("Status");

        // Configure other properties
        builder.Property(e => e.ProductType)
            .IsRequired()
            .HasColumnName("ProductType");

        builder.Property(e => e.ReturnDays)
            .HasColumnName("ReturnDays");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnName("UpdatedAt")
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes for Vietnamese marketplace performance
        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Products_Slug");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Products_Status");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Products_CreatedAt");

        builder.HasIndex(e => new { e.Status, e.CreatedAt })
            .HasDatabaseName("IX_Products_Status_CreatedAt");

        // Configure relationships
        builder.HasMany(e => e.Skus)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Categories)
            .WithMany()
            .UsingEntity(
                "ProductCategories",
                l => l.HasOne(typeof(CatCategory)).WithMany().HasForeignKey("CategoryId"),
                r => r.HasOne(typeof(CatProduct)).WithMany().HasForeignKey("ProductId"),
                j => j.HasKey("ProductId", "CategoryId"));

        builder.HasMany(e => e.Reviews)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("Products");
    }
}