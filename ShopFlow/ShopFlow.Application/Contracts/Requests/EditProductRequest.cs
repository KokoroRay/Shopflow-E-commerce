using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Application.Contracts.Requests;

/// <summary>
/// Request model for editing product details in Vietnamese marketplace
/// </summary>
public class EditProductRequest
{
    /// <summary>
    /// Product name - required for Vietnamese marketplace
    /// </summary>
    [Required]
    [MinLength(2)]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short description of the product
    /// </summary>
    [MaxLength(1000)]
    public string? ShortDescription { get; set; }

    /// <summary>
    /// Detailed description of the product  
    /// </summary>
    [MaxLength(5000)]
    public string? LongDescription { get; set; }

    /// <summary>
    /// Product type identifier
    /// </summary>
    public byte? ProductType { get; set; }

    /// <summary>
    /// Return policy in days
    /// </summary>
    [Range(0, 365)]
    public int? ReturnDays { get; set; }
}