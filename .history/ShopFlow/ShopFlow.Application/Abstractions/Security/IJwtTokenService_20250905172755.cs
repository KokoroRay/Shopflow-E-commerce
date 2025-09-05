namespace ShopFlow.Application.Abstractions.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(long userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    (bool IsValid, long UserId, string Email) ValidateToken(string token);
    DateTime GetTokenExpiration(string token);
}
