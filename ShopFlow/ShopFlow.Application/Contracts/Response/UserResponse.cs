namespace ShopFlow.Application.Contracts.Response;

public class UserResponse
{
    public long Id { get; set; }
    
    public string Email { get; set; } = string.Empty;
    
    public string? Phone { get; set; }
    
    public bool EmailVerified { get; set; }
    
    public byte Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public string? FullName { get; set; }
    
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public List<string> Roles { get; set; } = new();
}
