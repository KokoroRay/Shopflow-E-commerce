using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("sys_ext_mapping")]
[Index("Entity", "LocalId", Name = "IX_ext_mapping_lookup")]
[Index("VendorId", "Entity", "SystemId", "ExternalId", Name = "UQ_ext_mapping", IsUnique = true)]
public partial class SysExtMapping
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("vendor_id")]
    public long? VendorId { get; set; }

    [Column("entity")]
    [StringLength(30)]
    [Unicode(false)]
    public string Entity { get; set; } = null!;

    [Column("local_id")]
    public long LocalId { get; set; }

    [Column("external_id")]
    [StringLength(128)]
    public string ExternalId { get; set; } = null!;

    [Column("system_id")]
    [StringLength(50)]
    [Unicode(false)]
    public string SystemId { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("SysExtMappings")]
    public virtual MktVendor? Vendor { get; set; }
}
