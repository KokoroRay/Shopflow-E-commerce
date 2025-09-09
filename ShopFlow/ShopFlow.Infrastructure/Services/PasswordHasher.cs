using ShopFlow.Application.Abstractions.Security;

namespace ShopFlow.Infrastructure.Services;

/// <summary>
/// BCrypt-based password hasher implementation
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12; // BCrypt work factor for security

    /// <summary>
    /// Hashes a plain text password using BCrypt
    /// </summary>
    /// <param name="plain">Plain text password</param>
    /// <returns>BCrypt hashed password</returns>
    /// <exception cref="ArgumentException">When password is null or empty</exception>
    public string HashPassword(string plain)
    {
        if (string.IsNullOrWhiteSpace(plain))
            throw new ArgumentException("Password cannot be null or empty", nameof(plain));

        return BCrypt.Net.BCrypt.HashPassword(plain, WorkFactor);
    }

    /// <summary>
    /// Verifies a plain text password against a BCrypt hash
    /// </summary>
    /// <param name="plain">Plain text password</param>
    /// <param name="hash">BCrypt hash to verify against</param>
    /// <returns>True if password matches hash, false otherwise</returns>
    public bool VerifyPassword(string plain, string hash)
    {
        if (string.IsNullOrWhiteSpace(plain) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(plain, hash);
        }
        catch (Exception)
        {
            return false; // Invalid hash format or other BCrypt errors
        }
    }
}