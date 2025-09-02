using ShopFlow.Application.Abstractions.Security;

namespace ShopFlow.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string plain) => BCrypt.Net.BCrypt.HashPassword(plain);
    public bool VerifyPassword(string plain, string hash) => BCrypt.Net.BCrypt.Verify(plain, hash);
}