using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Infrastructure.Persistence;

public partial class ShopFlowDbContext : DbContext
{
    public ShopFlowDbContext(DbContextOptions<ShopFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<cart> carts { get; set; }

    public virtual DbSet<cart_applied_promo> cart_applied_promos { get; set; }

    public virtual DbSet<cart_item> cart_items { get; set; }

    public virtual DbSet<cat_attribute> cat_attributes { get; set; }

    public virtual DbSet<cat_attribute_option> cat_attribute_options { get; set; }

    public virtual DbSet<cat_attribute_option_i18n> cat_attribute_option_i18ns { get; set; }

    public virtual DbSet<cat_category> cat_categories { get; set; }

    public virtual DbSet<cat_category_i18n> cat_category_i18ns { get; set; }

    public virtual DbSet<cat_product> cat_products { get; set; }

    public virtual DbSet<cat_product_i18n> cat_product_i18ns { get; set; }

    public virtual DbSet<cat_sku> cat_skus { get; set; }

    public virtual DbSet<cat_sku_medium> cat_sku_media { get; set; }

    public virtual DbSet<cat_sku_option_value> cat_sku_option_values { get; set; }

    public virtual DbSet<ce_review> ce_reviews { get; set; }

    public virtual DbSet<core_address> core_addresses { get; set; }

    public virtual DbSet<core_permission> core_permissions { get; set; }

    public virtual DbSet<core_role> core_roles { get; set; }

    public virtual DbSet<core_user> core_users { get; set; }

    public virtual DbSet<core_user_role> core_user_roles { get; set; }

    public virtual DbSet<inv_adjustment> inv_adjustments { get; set; }

    public virtual DbSet<inv_adjustment_line> inv_adjustment_lines { get; set; }

    public virtual DbSet<inv_reservation> inv_reservations { get; set; }

    public virtual DbSet<inv_stock> inv_stocks { get; set; }

    public virtual DbSet<inv_warehouse> inv_warehouses { get; set; }

    public virtual DbSet<mkt_offer> mkt_offers { get; set; }

    public virtual DbSet<mkt_vendor> mkt_vendors { get; set; }

    public virtual DbSet<ord_order> ord_orders { get; set; }

    public virtual DbSet<ord_order_address> ord_order_addresses { get; set; }

    public virtual DbSet<ord_order_item> ord_order_items { get; set; }

    public virtual DbSet<ord_shipment> ord_shipments { get; set; }

    public virtual DbSet<pay_transaction> pay_transactions { get; set; }

    public virtual DbSet<promo_application_log> promo_application_logs { get; set; }

    public virtual DbSet<promo_campaign> promo_campaigns { get; set; }

    public virtual DbSet<promo_coupon> promo_coupons { get; set; }

    public virtual DbSet<promo_rule> promo_rules { get; set; }

    public virtual DbSet<role_admin_profile> role_admin_profiles { get; set; }

    public virtual DbSet<role_customer_profile> role_customer_profiles { get; set; }

    public virtual DbSet<role_moderator_profile> role_moderator_profiles { get; set; }

    public virtual DbSet<role_vendor_staff> role_vendor_staffs { get; set; }

    public virtual DbSet<role_warehouse_staff> role_warehouse_staffs { get; set; }

    public virtual DbSet<ship_config> ship_configs { get; set; }

    public virtual DbSet<ship_rate> ship_rates { get; set; }

    public virtual DbSet<ship_zone> ship_zones { get; set; }

    public virtual DbSet<sys_ext_mapping> sys_ext_mappings { get; set; }

    public virtual DbSet<sys_integration> sys_integrations { get; set; }

    public virtual DbSet<sys_staging> sys_stagings { get; set; }

    public virtual DbSet<sys_webhook_event> sys_webhook_events { get; set; }

    public virtual DbSet<tax_class> tax_classes { get; set; }

    public virtual DbSet<tax_rate> tax_rates { get; set; }

    public virtual DbSet<tax_rate_rule> tax_rate_rules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<cart>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cart__3213E83FE2B69EFD");

            entity.ToTable("cart");

            entity.HasIndex(e => e.guest_token, "IX_cart_guest").HasFilter("([guest_token] IS NOT NULL)");

            entity.HasIndex(e => e.user_id, "IX_cart_user");

            entity.Property(e => e.currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.guest_token)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.last_activity_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.user).WithMany(p => p.carts)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_cart_user");

            entity.HasOne<inv_warehouse>()
                .WithMany()
                .HasForeignKey(d => d.warehouse_id)
                .HasConstraintName("FK_cart_wh");
        });

        modelBuilder.Entity<cart_applied_promo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cart_app__3213E83FBEB33213");

            entity.ToTable("cart_applied_promo");

            entity.HasIndex(e => e.campaign_id, "IX_cart_applied_campaign");

            entity.HasIndex(e => e.coupon_id, "IX_cart_applied_coupon");

            entity.HasIndex(e => e.rule_id, "IX_cart_applied_rule");

            entity.Property(e => e.discount_amount).HasColumnType("decimal(19, 4)");

            entity.HasOne(d => d.cart).WithMany()
                .HasForeignKey(d => d.cart_id)
                .HasConstraintName("FK_cart_applied_cart");

            entity.HasOne(d => d.rule).WithMany()
                .HasForeignKey(d => d.rule_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_cart_applied_rule");
        });

        modelBuilder.Entity<cart_item>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cart_ite__3213E83F0FAEA501");

            entity.ToTable("cart_item");

            entity.HasIndex(e => e.cart_id, "IX_cart_item_cart");

            entity.Property(e => e.attributes_snapshot).HasMaxLength(1000);
            entity.Property(e => e.price_gross_snapshot).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.qty).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.tax_rate_snapshot).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.cart).WithMany()
                .HasForeignKey(d => d.cart_id)
                .HasConstraintName("FK_citem_cart");

            entity.HasOne(d => d.offer).WithMany()
                .HasForeignKey(d => d.offer_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citem_offer");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citem_sku");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_citem_vendor");
        });

        modelBuilder.Entity<cat_attribute>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cat_attr__3213E83FB5C1C08A");

            entity.ToTable("cat_attribute");

            entity.HasIndex(e => e.code, "UQ__cat_attr__357D4CF98DDDF4C5").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.data_type)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<cat_attribute_option>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cat_attr__3213E83F9587BAE4");

            entity.ToTable("cat_attribute_option");

            entity.HasIndex(e => new { e.attribute_id, e.code }, "UQ_attr_option").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.attribute).WithMany()
                .HasForeignKey(d => d.attribute_id)
                .HasConstraintName("FK_attr_opt_attr");
        });

        modelBuilder.Entity<cat_attribute_option_i18n>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("cat_attribute_option_i18n");

            entity.HasIndex(e => new { e.option_id, e.lang }, "UQ_attr_option_i18n").IsUnique();

            entity.Property(e => e.lang)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.name).HasMaxLength(100);

            entity.HasOne(d => d.option).WithMany()
                .HasForeignKey(d => d.option_id)
                .HasConstraintName("FK_attr_opt_i18n_opt");
        });

        modelBuilder.Entity<cat_category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cat_cate__3213E83F9B78ABD5");

            entity.ToTable("cat_category");

            entity.HasIndex(e => e.parent_id, "IX_cat_category_parent");

            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.parent).WithMany(p => p.Inverseparent)
                .HasForeignKey(d => d.parent_id)
                .HasConstraintName("FK_cat_parent");
        });

        modelBuilder.Entity<cat_category_i18n>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("cat_category_i18n");

            entity.HasIndex(e => new { e.category_id, e.lang }, "UQ_cat_category_i18n").IsUnique();

            entity.HasIndex(e => new { e.lang, e.slug }, "UQ_cat_category_slug").IsUnique();

            entity.Property(e => e.lang)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.meta_desc).HasMaxLength(500);
            entity.Property(e => e.meta_title).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.slug).HasMaxLength(255);

            entity.HasOne(d => d.category).WithMany()
                .HasForeignKey(d => d.category_id)
                .HasConstraintName("FK_cat_i18n_cat");
        });

        modelBuilder.Entity<cat_product>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cat_prod__3213E83F93656E59");

            entity.ToTable("cat_product");

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.status).HasDefaultValue((byte)1);
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            // Configure many-to-many without relying on navigation properties on either side
            entity
                .HasMany<cat_category>()
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "cat_product_category",
                    r => r.HasOne<cat_category>().WithMany()
                        .HasForeignKey("category_id")
                        .HasConstraintName("FK_prodcat_cat"),
                    l => l.HasOne<cat_product>().WithMany()
                        .HasForeignKey("product_id")
                        .HasConstraintName("FK_prodcat_prod"),
                    j =>
                    {
                        j.HasKey("product_id", "category_id").HasName("PK__cat_prod__1A56936E3E784F72");
                        j.ToTable("cat_product_category");
                    });
        });

        modelBuilder.Entity<cat_product_i18n>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("cat_product_i18n");

            entity.HasIndex(e => new { e.product_id, e.lang }, "UQ_cat_product_i18n").IsUnique();

            entity.HasIndex(e => new { e.lang, e.slug }, "UQ_cat_product_slug").IsUnique();

            entity.Property(e => e.lang)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.short_desc).HasMaxLength(1000);
            entity.Property(e => e.slug).HasMaxLength(255);

            entity.HasOne(d => d.product).WithMany()
                .HasForeignKey(d => d.product_id)
                .HasConstraintName("FK_prod_i18n_prod");
        });

        modelBuilder.Entity<cat_sku>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cat_sku__3213E83F57D44029");

            entity.ToTable("cat_sku");

            entity.HasIndex(e => e.product_id, "IX_cat_sku_product");

            entity.HasIndex(e => e.sku_code, "UQ__cat_sku__843F428F1400B56E").IsUnique();

            entity.Property(e => e.barcode)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.options_json).HasMaxLength(1000);
            entity.Property(e => e.sku_code)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.product).WithMany()
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sku_product");

            entity.HasMany(d => d.tax_classes).WithMany(p => p.skus)
                .UsingEntity<Dictionary<string, object>>(
                    "tax_mapping",
                    r => r.HasOne<tax_class>().WithMany()
                        .HasForeignKey("tax_class_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_taxmap_class"),
                    l => l.HasOne<cat_sku>().WithMany()
                        .HasForeignKey("sku_id")
                        .HasConstraintName("FK_taxmap_sku"),
                    j =>
                    {
                        j.HasKey("sku_id", "tax_class_id").HasName("PK__tax_mapp__1AF3CFA203E1CF36");
                        j.ToTable("tax_mapping");
                    });
        });

        modelBuilder.Entity<cat_sku_medium>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cat_sku___3213E83FA780699C");

            entity.HasIndex(e => new { e.sku_id, e.is_primary, e.sort }, "IX_cat_sku_media").IsDescending(false, true, false);

            entity.Property(e => e.url).HasMaxLength(1000);

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .HasConstraintName("FK_skumedia_sku");
        });

        modelBuilder.Entity<cat_sku_option_value>(entity =>
        {
            entity.HasKey(e => new { e.sku_id, e.attribute_id }).HasName("PK__cat_sku___43C05FEE185516A3");

            entity.ToTable("cat_sku_option_value");

            entity.HasIndex(e => e.attribute_id, "IX_skuopt_attr");

            entity.HasIndex(e => new { e.option_id, e.sku_id }, "UQ_sku_option").IsUnique();

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.attribute).WithMany()
                .HasForeignKey(d => d.attribute_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_skuopt_attr");

            entity.HasOne(d => d.option).WithMany()
                .HasForeignKey(d => d.option_id)
                .HasConstraintName("FK_skuopt_option");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .HasConstraintName("FK_skuopt_sku");
        });

        modelBuilder.Entity<ce_review>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ce_revie__3213E83FEB653455");

            entity.ToTable("ce_review");

            entity.HasIndex(e => new { e.product_id, e.vendor_id, e.status, e.created_at }, "IX_review_target").IsDescending(false, false, false, true);

            entity.HasIndex(e => new { e.user_id, e.created_at }, "IX_review_user").IsDescending(false, true);

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.moderated_at).HasPrecision(0);

            entity.HasOne(d => d.moderated_byNavigation).WithMany()
                .HasForeignKey(d => d.moderated_by)
                .HasConstraintName("FK_review_moderator");

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_review_order");

            entity.HasOne(d => d.product).WithMany()
                .HasForeignKey(d => d.product_id)
                .HasConstraintName("FK_review_product");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_review_sku");

            entity.HasOne(d => d.user).WithMany()
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_review_user");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .HasConstraintName("FK_review_vendor");
        });

        modelBuilder.Entity<core_address>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__core_add__3213E83FE98E91E5");

            entity.ToTable("core_address");

            entity.HasIndex(e => new { e.user_id, e.addr_type, e.is_default }, "IX_core_address_user").IsDescending(false, false, true);

            entity.Property(e => e.country)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("VN")
                .IsFixedLength();
            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.district).HasMaxLength(120);
            entity.Property(e => e.full_name).HasMaxLength(120);
            entity.Property(e => e.line1).HasMaxLength(255);
            entity.Property(e => e.line2).HasMaxLength(255);
            entity.Property(e => e.phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.postal_code).HasMaxLength(20);
            entity.Property(e => e.province).HasMaxLength(120);
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ward).HasMaxLength(120);

            entity.HasOne(d => d.user).WithMany()
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_address_user");
        });

        modelBuilder.Entity<core_permission>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__core_per__3213E83F58AC5275");

            entity.ToTable("core_permission");

            entity.HasIndex(e => e.code, "UQ__core_per__357D4CF9CE052DC3").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.description).HasMaxLength(255);
        });

        modelBuilder.Entity<core_role>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__core_rol__3213E83F1D90F3C6");

            entity.ToTable("core_role");

            entity.HasIndex(e => e.code, "UQ__core_rol__357D4CF96EE0621B").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.name).HasMaxLength(100);

            entity.Ignore(e => e.permissions);
        });

        modelBuilder.Entity<core_user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__core_use__3213E83F2D66A16D");

            entity.ToTable("core_user");

            entity.HasIndex(e => e.email, "UQ__core_use__AB6E61648F49A5D6").IsUnique();

            entity.HasIndex(e => e.phone, "UQ__core_use__B43B145FF8E7F06B").IsUnique();

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.password_hash).HasMaxLength(255);
            entity.Property(e => e.phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.status).HasDefaultValue((byte)1);
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<core_user_role>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.role_id }).HasName("PK__core_use__6EDEA1537B7F6708");

            entity.ToTable("core_user_role");

            entity.HasOne(d => d.role).WithMany(p => p.core_user_roles)
                .HasForeignKey(d => d.role_id)
                .HasConstraintName("FK_userrole_role");

            entity.HasOne(d => d.user).WithMany(p => p.core_user_roles)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_userrole_user");
        });

        modelBuilder.Entity<inv_adjustment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__inv_adju__3213E83F9C61D466");

            entity.ToTable("inv_adjustment");

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.reason).HasMaxLength(255);

            entity.HasOne(d => d.created_byNavigation).WithMany()
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("FK_adj_user");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adj_vendor");

            entity.HasOne(d => d.warehouse).WithMany()
                .HasForeignKey(d => d.warehouse_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adj_wh");
        });

        modelBuilder.Entity<inv_adjustment_line>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__inv_adju__3213E83FBAA29FFA");

            entity.ToTable("inv_adjustment_line");

            entity.Property(e => e.delta_qty).HasColumnType("decimal(18, 3)");

            entity.HasOne(d => d.adjustment).WithMany()
                .HasForeignKey(d => d.adjustment_id)
                .HasConstraintName("FK_adjline_adj");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_adjline_sku");
        });

        modelBuilder.Entity<inv_reservation>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__inv_rese__3213E83F99242ACF");

            entity.ToTable("inv_reservation");

            entity.HasIndex(e => new { e.warehouse_id, e.vendor_id, e.sku_id, e.status }, "IX_reservation_lookup");

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.expires_at).HasPrecision(0);
            entity.Property(e => e.qty).HasColumnType("decimal(18, 3)");

            entity.HasOne(d => d.cart).WithMany()
                .HasForeignKey(d => d.cart_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_resv_cart");

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_resv_order");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resv_sku");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resv_vendor");

            entity.HasOne(d => d.warehouse).WithMany()
                .HasForeignKey(d => d.warehouse_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resv_wh");
        });

        modelBuilder.Entity<inv_stock>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__inv_stoc__3213E83F664A6920");

            entity.ToTable("inv_stock");

            entity.HasIndex(e => new { e.sku_id, e.warehouse_id }, "IX_stock_sku_wh");

            entity.HasIndex(e => new { e.warehouse_id, e.vendor_id, e.sku_id }, "UQ_stock_wh_vendor_sku").IsUnique();

            entity.Property(e => e.on_hand).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.reserved).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.row_ver)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.safety_stock).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_sku");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_vendor");

            entity.HasOne(d => d.warehouse).WithMany()
                .HasForeignKey(d => d.warehouse_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_wh");
        });

        modelBuilder.Entity<inv_warehouse>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__inv_ware__3213E83FE68AF02E");

            entity.ToTable("inv_warehouse");

            entity.HasIndex(e => e.code, "UQ__inv_ware__357D4CF923EDBA83").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.name).HasMaxLength(255);
        });

        modelBuilder.Entity<mkt_offer>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__mkt_offe__3213E83F9CE3EAF4");

            entity.ToTable("mkt_offer");

            entity.HasIndex(e => e.sku_id, "IX_offer_sku");

            entity.HasIndex(e => e.vendor_id, "IX_offer_vendor");

            entity.HasIndex(e => new { e.vendor_id, e.sku_id }, "UQ_offer_vendor_sku").IsUnique();

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.max_qty).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.min_qty).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.price_gross_usd).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.price_gross_vnd).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.status).HasDefaultValue((byte)1);
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_offer_sku");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_offer_vendor");
        });

        modelBuilder.Entity<mkt_vendor>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__mkt_vend__3213E83FB605B1E6");

            entity.ToTable("mkt_vendor");

            entity.HasIndex(e => e.code, "UQ__mkt_vend__357D4CF9B4A60CD0").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.status).HasDefaultValue((byte)1);
            entity.Property(e => e.tax_code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<ord_order>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ord_orde__3213E83F81D1958B");

            entity.ToTable("ord_order");

            entity.HasIndex(e => e.placed_at, "IX_order_placed").IsDescending();

            entity.HasIndex(e => new { e.user_id, e.placed_at }, "IX_order_user").IsDescending(false, true);

            entity.HasIndex(e => e.order_code, "UQ__ord_orde__99D12D3FD4B4E915").IsUnique();

            entity.Property(e => e.cancel_reason).HasMaxLength(255);
            entity.Property(e => e.completed_at).HasPrecision(0);
            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.discount_total).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.fx_rate).HasColumnType("decimal(18, 8)");
            entity.Property(e => e.grand_total).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.order_code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.placed_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.shipping_fee).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.subtotal).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.tax_total).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.user).WithMany()
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_order_user");

            entity.HasOne(d => d.warehouse).WithMany()
                .HasForeignKey(d => d.warehouse_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_wh");
        });

        modelBuilder.Entity<ord_order_address>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ord_orde__3213E83F23E72B67");

            entity.ToTable("ord_order_address");

            entity.HasIndex(e => new { e.order_id, e.addr_type }, "UQ_order_addr_type").IsUnique();

            entity.Property(e => e.country)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.district).HasMaxLength(120);
            entity.Property(e => e.full_name).HasMaxLength(120);
            entity.Property(e => e.line1).HasMaxLength(255);
            entity.Property(e => e.line2).HasMaxLength(255);
            entity.Property(e => e.phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.postal_code).HasMaxLength(20);
            entity.Property(e => e.province).HasMaxLength(120);
            entity.Property(e => e.ward).HasMaxLength(120);

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("FK_oaddr_order");
        });

        modelBuilder.Entity<ord_order_item>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ord_orde__3213E83FC176D2AE");

            entity.ToTable("ord_order_item");

            entity.HasIndex(e => e.order_id, "IX_order_item_order");

            entity.HasIndex(e => e.vendor_id, "IX_order_item_vendor");

            entity.Property(e => e.attributes_snapshot).HasMaxLength(1000);
            entity.Property(e => e.discount).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.line_total)
                .HasComputedColumnSql("([qty]*([unit_price_gross]-[discount]))", true)
                .HasColumnType("decimal(38, 6)");
            entity.Property(e => e.name_snapshot).HasMaxLength(255);
            entity.Property(e => e.qty).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.tax_amount).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.unit_price_gross).HasColumnType("decimal(19, 4)");

            entity.HasOne(d => d.offer).WithMany()
                .HasForeignKey(d => d.offer_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oitm_offer");

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("FK_oitm_order");

            entity.HasOne(d => d.sku).WithMany()
                .HasForeignKey(d => d.sku_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oitm_sku");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_oitm_vendor");
        });

        modelBuilder.Entity<ord_shipment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ord_ship__3213E83FB395BF91");

            entity.ToTable("ord_shipment");

            entity.HasIndex(e => e.order_id, "UQ_ship_order").IsUnique();

            entity.Property(e => e.cost).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.delivered_at).HasPrecision(0);
            entity.Property(e => e.shipped_at).HasPrecision(0);
            entity.Property(e => e.shipper_code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.tracking_no)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.order).WithOne(p => p.ord_shipment)
                .HasForeignKey<ord_shipment>(d => d.order_id)
                .HasConstraintName("FK_ship_order");

            entity.HasOne(d => d.warehouse).WithMany()
                .HasForeignKey(d => d.warehouse_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ship_wh");
        });

        modelBuilder.Entity<pay_transaction>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__pay_tran__3213E83F1B3C9C90");

            entity.ToTable("pay_transaction");

            entity.HasIndex(e => e.idempotency_key, "UQ__pay_tran__A7BA59F4943B55D5").IsUnique();

            entity.Property(e => e.amount).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.attempt).HasDefaultValue(1);
            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.idempotency_key)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.secure_hash)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.vnp_bank_code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.vnp_card_type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.vnp_pay_date)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.vnp_transaction_no)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.vnp_txn_ref)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("FK_pay_order");
        });

        modelBuilder.Entity<promo_application_log>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__promo_ap__3213E83F82A388CB");

            entity.ToTable("promo_application_log");

            entity.HasIndex(e => e.campaign_id, "IX_promo_log_campaign");

            entity.HasIndex(e => new { e.cart_id, e.applied_at }, "IX_promo_log_cart");

            entity.HasIndex(e => e.coupon_id, "IX_promo_log_coupon");

            entity.HasIndex(e => new { e.order_id, e.applied_at }, "IX_promo_log_order");

            entity.Property(e => e.amount).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.applied_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.cart).WithMany()
                .HasForeignKey(d => d.cart_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_palog_cart");

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_palog_order");

            entity.HasOne(d => d.rule).WithMany(p => p.promo_application_logs)
                .HasForeignKey(d => d.rule_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_palog_rule");
        });

        modelBuilder.Entity<promo_campaign>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__promo_ca__3213E83F6DA3431A");

            entity.ToTable("promo_campaign", tb => tb.HasTrigger("trg_promo_campaign_cleanup"));

            entity.HasIndex(e => new { e.period_start, e.period_end, e.status }, "IX_promo_campaign_period");

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.period_end).HasPrecision(0);
            entity.Property(e => e.period_start).HasPrecision(0);
            entity.Property(e => e.status).HasDefaultValue((byte)1);
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<promo_coupon>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__promo_co__3213E83F931D3A8C");

            entity.ToTable("promo_coupon", tb => tb.HasTrigger("trg_promo_coupon_cleanup"));

            entity.HasIndex(e => e.code, "UQ__promo_co__357D4CF98839FDFC").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.status).HasDefaultValue((byte)1);

            entity.HasOne(d => d.campaign).WithMany(p => p.promo_coupons)
                .HasForeignKey(d => d.campaign_id)
                .HasConstraintName("FK_coupon_campaign");
        });

        modelBuilder.Entity<promo_rule>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__promo_ru__3213E83FDD31DDC7");

            entity.ToTable("promo_rule");

            entity.HasIndex(e => e.campaign_id, "IX_promo_rule_campaign");

            entity.Property(e => e.status).HasDefaultValue((byte)1);

            entity.HasOne(d => d.campaign).WithMany(p => p.promo_rules)
                .HasForeignKey(d => d.campaign_id)
                .HasConstraintName("FK_rule_campaign");
        });

        modelBuilder.Entity<role_admin_profile>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PK__role_adm__B9BE370F53CD92B1");

            entity.ToTable("role_admin_profile", tb => tb.HasTrigger("trg_role_admin_profile_validate"));

            entity.Property(e => e.user_id).ValueGeneratedNever();
            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.level).HasDefaultValue((byte)1);
            entity.Property(e => e.note).HasMaxLength(500);

            entity.HasOne(d => d.user).WithOne()
                .HasForeignKey<role_admin_profile>(d => d.user_id)
                .HasConstraintName("FK_admin_user");

            entity.HasOne(d => d.core_user_role).WithMany()
                .HasForeignKey(d => new { d.user_id, d.role_id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_admin_userrole");
        });

        modelBuilder.Entity<role_customer_profile>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PK__role_cus__B9BE370FD8EC194B");

            entity.ToTable("role_customer_profile", tb => tb.HasTrigger("trg_role_customer_profile_validate"));

            entity.Property(e => e.user_id).ValueGeneratedNever();
            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.full_name).HasMaxLength(200);
            entity.Property(e => e.gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne<core_user>()
                .WithOne()
                .HasForeignKey<role_customer_profile>(d => d.user_id)
                .HasConstraintName("FK_customer_user");

            // No FK to core_user_role because Domain model lacks role_id
        });

        modelBuilder.Entity<role_moderator_profile>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PK__role_mod__B9BE370FE8C6F404");

            entity.ToTable("role_moderator_profile", tb => tb.HasTrigger("trg_role_moderator_profile_validate"));

            entity.Property(e => e.user_id).ValueGeneratedNever();
            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.team).HasMaxLength(100);

            entity.HasOne(d => d.user).WithOne()
                .HasForeignKey<role_moderator_profile>(d => d.user_id)
                .HasConstraintName("FK_rmod_user");

            entity.HasOne(d => d.core_user_role).WithMany()
                .HasForeignKey(d => new { d.user_id, d.role_id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rmod_userrole");
        });

        modelBuilder.Entity<role_vendor_staff>(entity =>
        {
            entity.HasKey(e => new { e.vendor_id, e.user_id }).HasName("PK__role_ven__E4E6C80842F0EB10");

            entity.ToTable("role_vendor_staff", tb => tb.HasTrigger("trg_role_vendor_staff_validate"));

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.title).HasMaxLength(100);

            entity.HasOne(d => d.user).WithMany()
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_rvs_user");

            entity.HasOne(d => d.vendor).WithMany(p => p.role_vendor_staffs)
                .HasForeignKey(d => d.vendor_id)
                .HasConstraintName("FK_rvs_vendor");

            entity.HasOne(d => d.core_user_role).WithMany()
                .HasForeignKey(d => new { d.user_id, d.role_id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rvs_userrole");
        });

        modelBuilder.Entity<role_warehouse_staff>(entity =>
        {
            entity.HasKey(e => new { e.warehouse_id, e.user_id }).HasName("PK__role_war__98D405CFDF78C920");

            entity.ToTable("role_warehouse_staff", tb => tb.HasTrigger("trg_role_warehouse_staff_validate"));

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.shift).HasMaxLength(50);

            entity.HasOne(d => d.user).WithMany()
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_rwh_user");

            entity.HasOne(d => d.warehouse).WithMany(p => p.role_warehouse_staffs)
                .HasForeignKey(d => d.warehouse_id)
                .HasConstraintName("FK_rwh_wh");

            entity.HasOne(d => d.core_user_role).WithMany()
                .HasForeignKey(d => new { d.user_id, d.role_id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rwh_userrole");
        });

        modelBuilder.Entity<ship_config>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ship_con__3213E83F4DAFF8F6");

            entity.ToTable("ship_config");

            entity.HasIndex(e => e.warehouse_id, "UQ__ship_con__734FE6BE4A791E15").IsUnique();

            entity.Property(e => e.enabled).HasDefaultValue(true);
            entity.Property(e => e.lead_time_days).HasDefaultValue(2);

            entity.HasOne(d => d.warehouse).WithOne(p => p.ship_config)
                .HasForeignKey<ship_config>(d => d.warehouse_id)
                .HasConstraintName("FK_shipcfg_wh");
        });

        modelBuilder.Entity<ship_rate>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ship_rat__3213E83F10BB855A");

            entity.ToTable("ship_rate");

            entity.HasIndex(e => new { e.zone_id, e.weight_from_g, e.weight_to_g, e.effective_from }, "IX_ship_rate_zone").IsDescending(false, false, false, true);

            entity.Property(e => e.base_fee).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.cod_surcharge).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.effective_from).HasPrecision(0);
            entity.Property(e => e.effective_to).HasPrecision(0);
            entity.Property(e => e.fuel_surcharge_pct).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.step_fee_per_500g).HasColumnType("decimal(19, 4)");

            entity.HasOne(d => d.zone).WithMany()
                .HasForeignKey(d => d.zone_id)
                .HasConstraintName("FK_shiprate_zone");
        });

        modelBuilder.Entity<ship_zone>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ship_zon__3213E83F13A748B9");

            entity.ToTable("ship_zone");

            entity.HasIndex(e => e.country, "IX_ship_zone_country");

            entity.HasIndex(e => e.code, "UQ__ship_zon__357D4CF9768757B0").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.country)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("VN")
                .IsFixedLength();
            entity.Property(e => e.district_pattern).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.region_pattern).HasMaxLength(255);
        });

        modelBuilder.Entity<sys_ext_mapping>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__sys_ext___3213E83FCDB25722");

            entity.ToTable("sys_ext_mapping");

            entity.HasIndex(e => new { e.entity, e.local_id }, "IX_ext_mapping_lookup");

            entity.HasIndex(e => new { e.vendor_id, e.entity, e.system_id, e.external_id }, "UQ_ext_mapping").IsUnique();

            entity.Property(e => e.entity)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.external_id).HasMaxLength(128);
            entity.Property(e => e.system_id)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_extmap_vendor");
        });

        modelBuilder.Entity<sys_integration>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__sys_inte__3213E83F10E1101E");

            entity.ToTable("sys_integration");

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.status).HasDefaultValue((byte)1);
            entity.Property(e => e.type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.updated_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_sint_vendor");
        });

        modelBuilder.Entity<sys_staging>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__sys_stag__3213E83F3F033DD5");

            entity.ToTable("sys_staging");

            entity.HasIndex(e => new { e.integration_id, e.status, e.created_at }, "IX_staging_status");

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.entity)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.error_msg).HasMaxLength(1000);
            entity.Property(e => e.processed_at).HasPrecision(0);

            entity.HasOne(d => d.integration).WithMany()
                .HasForeignKey(d => d.integration_id)
                .HasConstraintName("FK_stg_integration");
        });

        modelBuilder.Entity<sys_webhook_event>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__sys_webh__3213E83FF7766A24");

            entity.ToTable("sys_webhook_event");

            entity.HasIndex(e => new { e.order_id, e.cart_id, e.product_id }, "IX_webhook_entity");

            entity.HasIndex(e => new { e.integration_id, e.status, e.created_at }, "IX_webhook_integration");

            entity.HasIndex(e => new { e.status, e.created_at }, "IX_webhook_status");

            entity.HasIndex(e => new { e.vendor_id, e.status, e.created_at }, "IX_webhook_vendor");

            entity.Property(e => e.created_at)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.last_attempt_at).HasPrecision(0);
            entity.Property(e => e.topic)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.cart).WithMany()
                .HasForeignKey(d => d.cart_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_cart");

            entity.HasOne(d => d.integration).WithMany()
                .HasForeignKey(d => d.integration_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_integration");

            entity.HasOne(d => d.order).WithMany()
                .HasForeignKey(d => d.order_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_order");

            entity.HasOne(d => d.product).WithMany()
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_product");

            entity.HasOne(d => d.vendor).WithMany()
                .HasForeignKey(d => d.vendor_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_webhook_vendor");
        });

        modelBuilder.Entity<tax_class>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tax_clas__3213E83FF5B507A4");

            entity.ToTable("tax_class");

            entity.HasIndex(e => e.code, "UQ__tax_clas__357D4CF9B2C7A80F").IsUnique();

            entity.Property(e => e.code)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tax_rate>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tax_rate__3213E83F167D1E25");

            entity.ToTable("tax_rate");

            entity.Property(e => e.country)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.rate_percent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.region)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tax_rate_rule>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tax_rate__3213E83F9F5C36EF");

            entity.ToTable("tax_rate_rule");

            entity.HasIndex(e => new { e.tax_class_id, e.effective_from }, "IX_trr_class").IsDescending(false, true);

            entity.Property(e => e.effective_from)
                .HasPrecision(0)
                .HasDefaultValue(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.effective_to).HasPrecision(0);

            entity.HasOne(d => d.tax_class).WithMany()
                .HasForeignKey(d => d.tax_class_id)
                .HasConstraintName("FK_trr_class");

            entity.HasOne(d => d.tax_rate).WithMany()
                .HasForeignKey(d => d.tax_rate_id)
                .HasConstraintName("FK_trr_rate");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
