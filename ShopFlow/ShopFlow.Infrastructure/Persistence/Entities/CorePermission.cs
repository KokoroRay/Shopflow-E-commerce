using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("core_permission")]
[Index("Code", Name = "UQ__core_per__357D4CF942AF0224", IsUnique = true)]
public partial class CorePermission
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    [StringLength(100)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("Permissions")]
    public virtual ICollection<CoreRole> Roles { get; set; } = new List<CoreRole>();
}
