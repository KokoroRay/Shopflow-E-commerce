using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopFlow.Infrastructure.Persistence.Entities;

[Keyless]
[Table("cat_attribute_option_i18n")]
[Index("OptionId", "Lang", Name = "UQ_attr_option_i18n", IsUnique = true)]
public partial class CatAttributeOptionI18n
{
    [Column("option_id")]
    public long OptionId { get; set; }

    [Column("lang")]
    [StringLength(2)]
    [Unicode(false)]
    public string Lang { get; set; } = null!;

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [ForeignKey("OptionId")]
    public virtual CatAttributeOption Option { get; set; } = null!;
}
