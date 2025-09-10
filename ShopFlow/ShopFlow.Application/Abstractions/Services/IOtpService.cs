using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Abstractions.Services;

/// <summary>
/// Service interface for OTP generation and validation
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Generates a new OTP code with specified expiration
    /// </summary>
    /// <param name="expirationMinutes">The expiration time in minutes (default: 15)</param>
    /// <returns>A new OtpCode instance</returns>
    OtpCode GenerateOtp(int expirationMinutes = 15);

    /// <summary>
    /// Validates if the provided OTP code matches the expected format
    /// </summary>
    /// <param name="otpCode">The OTP code to validate</param>
    /// <returns>True if valid format, false otherwise</returns>
    bool IsValidOtpFormat(string otpCode);

    /// <summary>
    /// Generates a secure random OTP code as string
    /// </summary>
    /// <returns>6-digit OTP string</returns>
    string GenerateOtpString();

    /// <summary>
    /// Validates if the OTP code is not expired
    /// </summary>
    /// <param name="otpCode">The OTP code to check</param>
    /// <returns>True if not expired, false otherwise</returns>
    bool IsOtpValid(OtpCode otpCode);
}
