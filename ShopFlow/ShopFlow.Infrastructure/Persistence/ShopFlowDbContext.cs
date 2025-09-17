using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PersistenceEntities = ShopFlow.Infrastructure.Persistence.Entities; // Added alias

namespace ShopFlow.Infrastructure.Persistence;

public partial class ShopFlowDbContext : DbContext
{
    public ShopFlowDbContext()
    {
    }

    public ShopFlowDbContext(DbContextOptions<ShopFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PersistenceEntities.Cart> Carts { get; set; }

    public virtual DbSet<PersistenceEntities.CartAppliedPromo> CartAppliedPromos { get; set; }

    public virtual DbSet<PersistenceEntities.CartItem> CartItems { get; set; }

    public virtual DbSet<PersistenceEntities.CatAttribute> CatAttributes { get; set; }

    public virtual DbSet<PersistenceEntities.CatAttributeOption> CatAttributeOptions { get; set; }

    public virtual DbSet<PersistenceEntities.CatAttributeOptionI18n> CatAttributeOptionI18ns { get; set; }

    public virtual DbSet<PersistenceEntities.CatCategory> CatCategories { get; set; }

    public virtual DbSet<PersistenceEntities.CatCategoryI18n> CatCategoryI18ns { get; set; }

    public virtual DbSet<PersistenceEntities.CatProduct> CatProducts { get; set; }

    public virtual DbSet<PersistenceEntities.CatProductI18n> CatProductI18ns { get; set; }

    public virtual DbSet<PersistenceEntities.CatSku> CatSkus { get; set; }

    public virtual DbSet<PersistenceEntities.CatSkuMedium> CatSkuMedia { get; set; }

    public virtual DbSet<PersistenceEntities.CatSkuOptionValue> CatSkuOptionValues { get; set; }

    public virtual DbSet<PersistenceEntities.CatSkuPricing> CatSkuPricings { get; set; }

    public virtual DbSet<PersistenceEntities.CatVariantGroup> CatVariantGroups { get; set; }

    public virtual DbSet<PersistenceEntities.CeReview> CeReviews { get; set; }

    public virtual DbSet<PersistenceEntities.CoreAddress> CoreAddresses { get; set; }

    public virtual DbSet<PersistenceEntities.CorePermission> CorePermissions { get; set; }

    public virtual DbSet<PersistenceEntities.CoreRole> CoreRoles { get; set; }

    public virtual DbSet<PersistenceEntities.CoreUser> CoreUsers { get; set; }

    public virtual DbSet<PersistenceEntities.CoreUserRole> CoreUserRoles { get; set; }

    public virtual DbSet<PersistenceEntities.InvAdjustment> InvAdjustments { get; set; }

    public virtual DbSet<PersistenceEntities.InvAdjustmentLine> InvAdjustmentLines { get; set; }

    public virtual DbSet<PersistenceEntities.InvReservation> InvReservations { get; set; }

    public virtual DbSet<PersistenceEntities.InvStock> InvStocks { get; set; }

    public virtual DbSet<PersistenceEntities.InvStockChangeLog> InvStockChangeLogs { get; set; }

    public virtual DbSet<PersistenceEntities.InvWarehouse> InvWarehouses { get; set; }

    public virtual DbSet<PersistenceEntities.MktOffer> MktOffers { get; set; }

    public virtual DbSet<PersistenceEntities.MktVendor> MktVendors { get; set; }

    public virtual DbSet<PersistenceEntities.OrdOrder> OrdOrders { get; set; }

    public virtual DbSet<PersistenceEntities.OrdOrderAddress> OrdOrderAddresses { get; set; }

    public virtual DbSet<PersistenceEntities.OrdOrderItem> OrdOrderItems { get; set; }

    public virtual DbSet<PersistenceEntities.OrdShipment> OrdShipments { get; set; }

    public virtual DbSet<PersistenceEntities.PayTransaction> PayTransactions { get; set; }

    public virtual DbSet<PersistenceEntities.PromoApplicationLog> PromoApplicationLogs { get; set; }

    public virtual DbSet<PersistenceEntities.PromoCampaign> PromoCampaigns { get; set; }

    public virtual DbSet<PersistenceEntities.PromoCoupon> PromoCoupons { get; set; }

    public virtual DbSet<PersistenceEntities.PromoRule> PromoRules { get; set; }

    public virtual DbSet<PersistenceEntities.RoleAdminProfile> RoleAdminProfiles { get; set; }

    public virtual DbSet<PersistenceEntities.RoleCustomerProfile> RoleCustomerProfiles { get; set; }

    public virtual DbSet<PersistenceEntities.RoleModeratorProfile> RoleModeratorProfiles { get; set; }

    public virtual DbSet<PersistenceEntities.RoleVendorStaff> RoleVendorStaffs { get; set; }

    public virtual DbSet<PersistenceEntities.RoleWarehouseStaff> RoleWarehouseStaffs { get; set; }

    public virtual DbSet<PersistenceEntities.ShipConfig> ShipConfigs { get; set; }

    public virtual DbSet<PersistenceEntities.ShipRate> ShipRates { get; set; }

    public virtual DbSet<PersistenceEntities.ShipZone> ShipZones { get; set; }

    public virtual DbSet<PersistenceEntities.SysExtMapping> SysExtMappings { get; set; }

    public virtual DbSet<PersistenceEntities.SysIntegration> SysIntegrations { get; set; }

    public virtual DbSet<PersistenceEntities.SysStaging> SysStagings { get; set; }

    public virtual DbSet<PersistenceEntities.SysWebhookEvent> SysWebhookEvents { get; set; }

    public virtual DbSet<PersistenceEntities.TaxClass> TaxClasses { get; set; }

    public virtual DbSet<PersistenceEntities.TaxRate> TaxRates { get; set; }

    public virtual DbSet<PersistenceEntities.TaxRateRule> TaxRateRules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ShopFlow;User Id=sa;Password=Ray013569dsdp~;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersistenceEntities.Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cart__3213E83F956C75A9");

            entity.HasIndex(e => e.GuestToken, "IX_cart_guest").HasFilter("([guest_token] IS NOT NULL)");

            entity.Property(e => e.Currency).IsFixedLength();
            entity.Property(e => e.LastActivityAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.Carts).HasConstraintName("FK_cart_user");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Carts).HasConstraintName("FK_cart_wh");
        });

        modelBuilder.Entity<PersistenceEntities.CartAppliedPromo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cart_app__3213E83FE7D97F3A");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartAppliedPromos).HasConstraintName("FK_cart_applied_cart");

            entity.HasOne(d => d.Rule).WithMany(p => p.CartAppliedPromos)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_cart_applied_rule");
        });

        modelBuilder.Entity<PersistenceEntities.CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cart_ite__3213E83FB61C73E2");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK_citem_cart");

            entity.HasOne(d => d.Offer).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citem_offer");

            entity.HasOne(d => d.Sku).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citem_sku");

            entity.HasOne(d => d.Vendor).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citem_vendor");
        });

        modelBuilder.Entity<PersistenceEntities.CatAttribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_attr__3213E83FC11209B2");
        });

        modelBuilder.Entity<PersistenceEntities.CatAttributeOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_attr__3213E83F91C09407");

            entity.HasOne(d => d.Attribute).WithMany(p => p.CatAttributeOptions).HasConstraintName("FK_attr_opt_attr");
        });

        modelBuilder.Entity<PersistenceEntities.CatAttributeOptionI18n>(entity =>
        {
            entity.Property(e => e.Lang).IsFixedLength();

            entity.HasOne(d => d.Option).WithMany().HasConstraintName("FK_attr_opt_i18n_opt");
        });

        modelBuilder.Entity<PersistenceEntities.CatCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_cate__3213E83F8BF3B064");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_cat_parent");
        });

        modelBuilder.Entity<PersistenceEntities.CatCategoryI18n>(entity =>
        {
            entity.Property(e => e.Language).IsFixedLength();

            entity.HasOne(d => d.Category).WithMany().HasConstraintName("FK_cat_i18n_cat");
        });

        modelBuilder.Entity<PersistenceEntities.CatProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_prod__3213E83F56E7CF86");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasMany(d => d.Categories).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "CatProductCategory",
                    r => r.HasOne<PersistenceEntities.CatCategory>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_prodcat_cat"),
                    l => l.HasOne<PersistenceEntities.CatProduct>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_prodcat_prod"),
                    j =>
                    {
                        j.HasKey("ProductId", "CategoryId").HasName("PK__cat_prod__1A56936E4F2CD660");
                        j.ToTable("cat_product_category");
                        j.IndexerProperty<long>("ProductId").HasColumnName("product_id");
                        j.IndexerProperty<long>("CategoryId").HasColumnName("category_id");
                    });
        });

        modelBuilder.Entity<PersistenceEntities.CatProductI18n>(entity =>
        {
            entity.Property(e => e.Lang).IsFixedLength();

            entity.HasOne(d => d.Product).WithMany().HasConstraintName("FK_prod_i18n_prod");
        });

        modelBuilder.Entity<PersistenceEntities.CatSku>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_sku__3213E83FF65F819C");

            entity.ToTable("cat_sku", tb => tb.HasTrigger("tr_CatSku_AutoGenerate"));

            entity.Property(e => e.AutoBarcodeGeneration).HasDefaultValue(true);
            entity.Property(e => e.AutoSkuGeneration).HasDefaultValue(true);
            entity.Property(e => e.BarcodeType).HasDefaultValue("EAN13");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CatSkuCreatedByNavigations).HasConstraintName("FK_cat_sku_created_by");

            entity.HasOne(d => d.Product).WithMany(p => p.Skus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sku_product");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CatSkuUpdatedByNavigations).HasConstraintName("FK_cat_sku_updated_by");

            entity.HasMany(d => d.TaxClasses).WithMany(p => p.Skus)
                .UsingEntity<Dictionary<string, object>>(
                    "TaxMapping",
                    r => r.HasOne<PersistenceEntities.TaxClass>().WithMany()
                        .HasForeignKey("TaxClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_taxmap_class"),
                    l => l.HasOne<PersistenceEntities.CatSku>().WithMany()
                        .HasForeignKey("SkuId")
                        .HasConstraintName("FK_taxmap_sku"),
                    j =>
                    {
                        j.HasKey("SkuId", "TaxClassId").HasName("PK__tax_mapp__1AF3CFA2863A6721");
                        j.ToTable("tax_mapping");
                        j.IndexerProperty<long>("SkuId").HasColumnName("sku_id");
                        j.IndexerProperty<int>("TaxClassId").HasColumnName("tax_class_id");
                    });
        });

        modelBuilder.Entity<PersistenceEntities.CatSkuMedium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_sku___3213E83F0F000CE6");

            entity.HasOne(d => d.Sku).WithMany(p => p.CatSkuMedia).HasConstraintName("FK_skumedia_sku");
        });

        modelBuilder.Entity<PersistenceEntities.CatSkuOptionValue>(entity =>
        {
            entity.HasKey(e => new { e.SkuId, e.AttributeId }).HasName("PK__cat_sku___43C05FEE046E459F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Attribute).WithMany(p => p.CatSkuOptionValues)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_skuopt_attr");

            entity.HasOne(d => d.Option).WithMany(p => p.CatSkuOptionValues).HasConstraintName("FK_skuopt_option");

            entity.HasOne(d => d.Sku).WithMany(p => p.CatSkuOptionValues).HasConstraintName("FK_skuopt_sku");
        });

        modelBuilder.Entity<PersistenceEntities.CatSkuPricing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_sku___3213E83F3E9BD49E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EffectiveFrom).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CatSkuPricings).HasConstraintName("FK__cat_sku_p__creat__79FD19BE");

            entity.HasOne(d => d.Sku).WithMany(p => p.CatSkuPricings).HasConstraintName("FK__cat_sku_p__sku_i__7908F585");
        });

        modelBuilder.Entity<PersistenceEntities.CatVariantGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cat_vari__3213E83F1E75BC8B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsRequired).HasDefaultValue(true);

            entity.HasOne(d => d.ParentGroup).WithMany(p => p.InverseParentGroup).HasConstraintName("FK__cat_varia__paren__74444068");

            entity.HasOne(d => d.Product).WithMany(p => p.CatVariantGroups).HasConstraintName("FK__cat_varia__produ__73501C2F");
        });

        modelBuilder.Entity<PersistenceEntities.CeReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ce_revie__3213E83FACA0E075");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ModeratedByNavigation).WithMany(p => p.CeReviewModeratedByNavigations).HasConstraintName("FK_review_moderator");

            entity.HasOne(d => d.Order).WithMany(p => p.CeReviews)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_review_order");

            entity.HasOne(d => d.Product).WithMany(p => p.CeReviews).HasConstraintName("FK_review_product");

            entity.HasOne(d => d.Sku).WithMany(p => p.CeReviews)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_review_sku");

            entity.HasOne(d => d.User).WithMany(p => p.CeReviewUsers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_review_user");

            entity.HasOne(d => d.Vendor).WithMany(p => p.CeReviews).HasConstraintName("FK_review_vendor");
        });

        modelBuilder.Entity<PersistenceEntities.CoreAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__core_add__3213E83F0C72B62C");

            entity.Property(e => e.Country)
                .HasDefaultValue("VN")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.CoreAddresses).HasConstraintName("FK_address_user");
        });

        modelBuilder.Entity<PersistenceEntities.CorePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__core_per__3213E83F8AFB48FF");
        });

        modelBuilder.Entity<PersistenceEntities.CoreRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__core_rol__3213E83F75355160");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "CoreRolePermission",
                    r => r.HasOne<PersistenceEntities.CorePermission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("FK_roleperm_perm"),
                    l => l.HasOne<PersistenceEntities.CoreRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_roleperm_role"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__core_rol__C85A5463EB37DB4F");
                        j.ToTable("core_role_permission");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<PersistenceEntities.CoreUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__core_use__3213E83F69E8B078");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<PersistenceEntities.CoreUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__core_use__6EDEA15378A5A4FA");

            entity.HasOne(d => d.Role).WithMany(p => p.CoreUserRoles).HasConstraintName("FK_userrole_role");

            entity.HasOne(d => d.User).WithMany(p => p.CoreUserRoles).HasConstraintName("FK_userrole_user");
        });

        modelBuilder.Entity<PersistenceEntities.InvAdjustment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inv_adju__3213E83FDB88AD09");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvAdjustments).HasConstraintName("FK_adj_user");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvAdjustments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adj_vendor");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvAdjustments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adj_wh");
        });

        modelBuilder.Entity<PersistenceEntities.InvAdjustmentLine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inv_adju__3213E83FE32B0D2B");

            entity.HasOne(d => d.Adjustment).WithMany(p => p.InvAdjustmentLines).HasConstraintName("FK_adjline_adj");

            entity.HasOne(d => d.Sku).WithMany(p => p.InvAdjustmentLines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adjline_sku");
        });

        modelBuilder.Entity<PersistenceEntities.InvReservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inv_rese__3213E83F14DC6098");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cart).WithMany(p => p.InvReservations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_resv_cart");

            entity.HasOne(d => d.Order).WithMany(p => p.InvReservations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_resv_order");

            entity.HasOne(d => d.Sku).WithMany(p => p.InvReservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resv_sku");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvReservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resv_vendor");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvReservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resv_wh");
        });

        modelBuilder.Entity<PersistenceEntities.InvStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inv_stoc__3213E83F38761265");

            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Sku).WithMany(p => p.InvStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_sku");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_vendor");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvStocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_wh");
        });

        modelBuilder.Entity<PersistenceEntities.InvStockChangeLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inv_stoc__3213E83FA462797F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvStockChangeLogs).HasConstraintName("FK__inv_stock__creat__6CA31EA0");

            entity.HasOne(d => d.Sku).WithMany(p => p.InvStockChangeLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inv_stock__sku_i__69C6B1F5");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvStockChangeLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inv_stock__vendo__6BAEFA67");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InvStockChangeLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inv_stock__wareh__6ABAD62E");
        });

        modelBuilder.Entity<PersistenceEntities.InvWarehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inv_ware__3213E83FD1082020");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<PersistenceEntities.MktOffer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__mkt_offe__3213E83FE1303B0F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Sku).WithMany(p => p.MktOffers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_offer_sku");

            entity.HasOne(d => d.Vendor).WithMany(p => p.MktOffers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_offer_vendor");
        });

        modelBuilder.Entity<PersistenceEntities.MktVendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__mkt_vend__3213E83FEA735B50");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<PersistenceEntities.OrdOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ord_orde__3213E83F8035F048");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Currency).IsFixedLength();
            entity.Property(e => e.PlacedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.OrdOrders).HasConstraintName("FK_order_user");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.OrdOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_wh");
        });

        modelBuilder.Entity<PersistenceEntities.OrdOrderAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ord_orde__3213E83FED20FEF9");

            entity.Property(e => e.Country).IsFixedLength();

            entity.HasOne(d => d.Order).WithMany(p => p.OrdOrderAddresses).HasConstraintName("FK_oaddr_order");
        });

        modelBuilder.Entity<PersistenceEntities.OrdOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ord_orde__3213E83FD6570F7E");

            entity.Property(e => e.LineTotal).HasComputedColumnSql("([qty]*([unit_price_gross]-[discount]))", true);

            entity.HasOne(d => d.Offer).WithMany(p => p.OrdOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oitm_offer");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdOrderItems).HasConstraintName("FK_oitm_order");

            entity.HasOne(d => d.Sku).WithMany(p => p.OrdOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oitm_sku");

            entity.HasOne(d => d.Vendor).WithMany(p => p.OrdOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oitm_vendor");
        });

        modelBuilder.Entity<PersistenceEntities.OrdShipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ord_ship__3213E83FB377F69D");

            entity.HasOne(d => d.Order).WithOne(p => p.OrdShipment).HasConstraintName("FK_ship_order");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.OrdShipments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ship_wh");
        });

        modelBuilder.Entity<PersistenceEntities.PayTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pay_tran__3213E83F4BA24808");

            entity.Property(e => e.Attempt).HasDefaultValue(1);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Order).WithMany(p => p.PayTransactions).HasConstraintName("FK_pay_order");
        });

        modelBuilder.Entity<PersistenceEntities.PromoApplicationLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__promo_ap__3213E83F5FC3E67C");

            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cart).WithMany(p => p.PromoApplicationLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_palog_cart");

            entity.HasOne(d => d.Order).WithMany(p => p.PromoApplicationLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_palog_order");

            entity.HasOne(d => d.Rule).WithMany(p => p.PromoApplicationLogs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_palog_rule");
        });

        modelBuilder.Entity<PersistenceEntities.PromoCampaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__promo_ca__3213E83F54F9CB3F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<PersistenceEntities.PromoCoupon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__promo_co__3213E83F423F95A3");

            entity.Property(e => e.Status).HasDefaultValue((byte)1);

            entity.HasOne(d => d.Campaign).WithMany(p => p.PromoCoupons).HasConstraintName("FK_coupon_campaign");
        });

        modelBuilder.Entity<PersistenceEntities.PromoRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__promo_ru__3213E83F323AC90F");

            entity.Property(e => e.Status).HasDefaultValue((byte)1);

            entity.HasOne(d => d.Campaign).WithMany(p => p.PromoRules).HasConstraintName("FK_rule_campaign");
        });

        modelBuilder.Entity<PersistenceEntities.RoleAdminProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__role_adm__B9BE370FFA559683");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Level).HasDefaultValue((byte)1);

            entity.HasOne(d => d.User).WithOne(p => p.RoleAdminProfile).HasConstraintName("FK_admin_user");

            entity.HasOne(d => d.CoreUserRole).WithMany(p => p.RoleAdminProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_admin_userrole");
        });

        modelBuilder.Entity<PersistenceEntities.RoleCustomerProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__role_cus__B9BE370FC237D2CA");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Gender).IsFixedLength();

            entity.HasOne(d => d.User).WithOne(p => p.RoleCustomerProfile).HasConstraintName("FK_customer_user");

            entity.HasOne(d => d.CoreUserRole).WithMany(p => p.RoleCustomerProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customer_userrole");
        });

        modelBuilder.Entity<PersistenceEntities.RoleModeratorProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__role_mod__B9BE370FD06DE732");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithOne(p => p.RoleModeratorProfile).HasConstraintName("FK_rmod_user");

            entity.HasOne(d => d.CoreUserRole).WithMany(p => p.RoleModeratorProfiles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rmod_userrole");
        });

        modelBuilder.Entity<PersistenceEntities.RoleVendorStaff>(entity =>
        {
            entity.HasKey(e => new { e.VendorId, e.UserId }).HasName("PK__role_ven__E4E6C80848B8C30B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithMany(p => p.RoleVendorStaffs).HasConstraintName("FK_rvs_user");

            entity.HasOne(d => d.Vendor).WithMany(p => p.RoleVendorStaffs).HasConstraintName("FK_rvs_vendor");

            entity.HasOne(d => d.CoreUserRole).WithMany(p => p.RoleVendorStaffs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rvs_userrole");
        });

        modelBuilder.Entity<PersistenceEntities.RoleWarehouseStaff>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseId, e.UserId }).HasName("PK__role_war__98D405CFF1721872");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.RoleWarehouseStaffs).HasConstraintName("FK_rwh_user");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.RoleWarehouseStaffs).HasConstraintName("FK_rwh_wh");

            entity.HasOne(d => d.CoreUserRole).WithMany(p => p.RoleWarehouseStaffs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rwh_userrole");
        });

        modelBuilder.Entity<PersistenceEntities.ShipConfig>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ship_con__3213E83FC069C866");

            entity.Property(e => e.Enabled).HasDefaultValue(true);
            entity.Property(e => e.LeadTimeDays).HasDefaultValue(2);

            entity.HasOne(d => d.Warehouse).WithOne(p => p.ShipConfig).HasConstraintName("FK_shipcfg_wh");
        });

        modelBuilder.Entity<PersistenceEntities.ShipRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ship_rat__3213E83F710EA2B6");

            entity.HasOne(d => d.Zone).WithMany(p => p.ShipRates).HasConstraintName("FK_shiprate_zone");
        });

        modelBuilder.Entity<PersistenceEntities.ShipZone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ship_zon__3213E83F0B9E46FD");

            entity.Property(e => e.Country)
                .HasDefaultValue("VN")
                .IsFixedLength();
        });

        modelBuilder.Entity<PersistenceEntities.SysExtMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sys_ext___3213E83F46BBC71F");

            entity.HasOne(d => d.Vendor).WithMany(p => p.SysExtMappings)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_extmap_vendor");
        });

        modelBuilder.Entity<PersistenceEntities.SysIntegration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sys_inte__3213E83F8C3CB34E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Vendor).WithMany(p => p.SysIntegrations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_sint_vendor");
        });

        modelBuilder.Entity<PersistenceEntities.SysStaging>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sys_stag__3213E83FDF3600D8");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Integration).WithMany(p => p.SysStagings).HasConstraintName("FK_stg_integration");
        });

        modelBuilder.Entity<PersistenceEntities.SysWebhookEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sys_webh__3213E83FAD51FF46");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Cart).WithMany(p => p.SysWebhookEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_cart");

            entity.HasOne(d => d.Integration).WithMany(p => p.SysWebhookEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_integration");

            entity.HasOne(d => d.Order).WithMany(p => p.SysWebhookEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_order");

            entity.HasOne(d => d.Product).WithMany(p => p.SysWebhookEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_product");

            entity.HasOne(d => d.Vendor).WithMany(p => p.SysWebhookEvents)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_vendor");
        });

        modelBuilder.Entity<PersistenceEntities.TaxClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tax_clas__3213E83F50BB2908");
        });

        modelBuilder.Entity<PersistenceEntities.TaxRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tax_rate__3213E83F1660E669");

            entity.Property(e => e.Country).IsFixedLength();
        });

        modelBuilder.Entity<PersistenceEntities.TaxRateRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tax_rate__3213E83F7FF7CAFC");

            entity.Property(e => e.EffectiveFrom).HasDefaultValue(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            entity.HasOne(d => d.TaxClass).WithMany(p => p.TaxRateRules).HasConstraintName("FK_trr_class");

            entity.HasOne(d => d.TaxRate).WithMany(p => p.TaxRateRules).HasConstraintName("FK_trr_rate");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}