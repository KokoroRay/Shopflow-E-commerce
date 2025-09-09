namespace ShopFlow.Application.Contracts.Response;

/// <summary>
/// Response for forgot password request
/// </summary>
public sealed class ForgotPasswordResponse
{
    /// <summary>
    /// Whether the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Status message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Time until OTP expires in minutes (only if successful)
    /// </summary>
    public int? ExpiresInMinutes { get; set; }
}

/// <summary>
/// Response for reset password request
/// </summary>
public sealed class ResetPasswordResponse
{
    /// <summary>
    /// Whether the password reset was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Status message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
