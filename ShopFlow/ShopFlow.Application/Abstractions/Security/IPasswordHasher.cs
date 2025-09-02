namespace ShopFlow.Application.Abstractions.Security;

public interface IPasswordHasher
{
    string HashPassword(string plain);
    bool VerifyPassword(string plain, string hash);
}