using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Application.Contracts.Requests;

public class UpdateUserRequest
{
    [Phone]
    public string? Phone { get; set; }
    
    [MinLength(2)]
    [MaxLength(100)]
    public string? FullName { get; set; }
    
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public bool? EmailVerified { get; set; }
    
    public byte? Status { get; set; }
}
