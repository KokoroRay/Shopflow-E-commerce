using MediatR;

namespace ShopFlow.Application.Commands.Users;

/// <summary>
/// Command to initiate a password reset process by sending OTP to user's email
/// </summary>
/// <param name="Email">The email address to send the reset OTP to</param>
public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;

/// <summary>
/// Response for the forgot password command
/// </summary>
/// <param name="Success">Whether the request was successful</param>
/// <param name="Message">Status message</param>
/// <param name="ExpiresInMinutes">Time until OTP expires (only if successful)</param>
public sealed record ForgotPasswordResponse(
    bool Success,
    string Message,
    int? ExpiresInMinutes = null
);
