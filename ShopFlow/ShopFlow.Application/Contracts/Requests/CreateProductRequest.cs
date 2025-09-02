using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Application.Contracts.Requests;

public class CreateProductRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? ShortDescription { get; set; }
    
    [MaxLength(500)]
    public string? MetaTitle { get; set; }
    
    [MaxLength(500)]
    public string? MetaDescription { get; set; }
    
    [Required]
    [MinLength(2)]
    [MaxLength(255)]
    public string Slug { get; set; } = string.Empty;
    
    [Required]
    public byte ProductType { get; set; }
    
    public int? ReturnDays { get; set; }
    
    [Required]
    public string Language { get; set; } = "vi";
    
    public List<long> CategoryIds { get; set; } = new();
}
