using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Table("core_role")]
[Index("Code", Name = "UQ__core_rol__357D4CF9B2B1791A", IsUnique = true)]
public partial class CoreRole
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<CoreUserRole> CoreUserRoles { get; set; } = new List<CoreUserRole>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<CorePermission> Permissions { get; set; } = new List<CorePermission>();
}
