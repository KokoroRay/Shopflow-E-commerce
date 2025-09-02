using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Application.Contracts.Requests;

public class CreateUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Phone]
    public string? Phone { get; set; }
    
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
}
