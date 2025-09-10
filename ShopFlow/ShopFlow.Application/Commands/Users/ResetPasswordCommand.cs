using MediatR;

namespace ShopFlow.Application.Commands.Users;

/// <summary>
/// Command to reset user password using OTP verification
/// </summary>
/// <param name="Email">The email address of the user</param>
/// <param name="OtpCode">The OTP code received via email</param>
/// <param name="NewPassword">The new password to set</param>
public sealed record ResetPasswordCommand(
    string Email,
    string OtpCode,
    string NewPassword
) : IRequest<ResetPasswordResponse>;

/// <summary>
/// Response for the reset password command
/// </summary>
/// <param name="Success">Whether the password reset was successful</param>
/// <param name="Message">Status message</param>
public sealed record ResetPasswordResponse(
    bool Success,
    string Message
);
