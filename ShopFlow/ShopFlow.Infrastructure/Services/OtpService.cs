using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Services;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Infrastructure.Services;

/// <summary>
/// OTP service implementation for generating and validating OTP codes
/// </summary>
public sealed class OtpService : IOtpService
{
    private readonly ILogger<OtpService> _logger;

    /// <summary>
    /// Initializes a new instance of the OtpService
    /// </summary>
    public OtpService(ILogger<OtpService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates a new OTP code with specified expiration
    /// </summary>
    public OtpCode GenerateOtp(int expirationMinutes = 15)
    {
        try
        {
            var otpCode = OtpCode.Generate(expirationMinutes);
            _logger.LogInformation("OTP code generated with expiration in {ExpirationMinutes} minutes", expirationMinutes);
            return otpCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate OTP code");
            throw;
        }
    }

    /// <summary>
    /// Validates if the provided OTP code matches the expected format
    /// </summary>
    public bool IsValidOtpFormat(string otpCode)
    {
        return OtpCode.IsValidFormat(otpCode);
    }

    /// <summary>
    /// Generates a secure random OTP code as string
    /// </summary>
    public string GenerateOtpString()
    {
        try
        {
            var otpCode = OtpCode.Generate();
            return otpCode.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate OTP string");
            throw;
        }
    }

    /// <summary>
    /// Validates if the OTP code is not expired
    /// </summary>
    public bool IsOtpValid(OtpCode otpCode)
    {
        if (otpCode is null)
            return false;

        return otpCode.IsValid();
    }
}
