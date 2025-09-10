using Microsoft.Extensions.Logging;
using ShopFlow.Application.Abstractions.Services;

namespace ShopFlow.Infrastructure.Services;

/// <summary>
/// Email service implementation for sending password reset emails
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the EmailService
    /// </summary>
    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sends a password reset OTP email to the specified recipient
    /// </summary>
    public async Task SendPasswordResetOtpAsync(string toEmail, string otpCode, int expirationMinutes, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = "Password Reset OTP - ShopFlow";
            var body = GetPasswordResetOtpTemplate(otpCode, expirationMinutes);

            await SendEmailAsync(toEmail, subject, body, isHtml: true, cancellationToken);

            _logger.LogInformation("Password reset OTP email sent to: {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset OTP email to: {Email}", toEmail);
            throw;
        }
    }

    /// <summary>
    /// Sends a password reset confirmation email
    /// </summary>
    public async Task SendPasswordResetConfirmationAsync(string toEmail, DateTime resetDateTime, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = "Password Reset Confirmation - ShopFlow";
            var body = GetPasswordResetConfirmationTemplate(resetDateTime);

            await SendEmailAsync(toEmail, subject, body, isHtml: true, cancellationToken);

            _logger.LogInformation("Password reset confirmation email sent to: {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset confirmation email to: {Email}", toEmail);
            throw;
        }
    }

    /// <summary>
    /// Sends a generic email with subject and body
    /// </summary>
    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement actual email sending using SMTP, SendGrid, or other email service
            // For now, we'll just log the email content
            _logger.LogInformation("Sending email to: {Email}", toEmail);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {Body}", body);

            // Simulate async email sending
            await Task.Delay(100, cancellationToken);

            _logger.LogInformation("Email sent successfully to: {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to: {Email}", toEmail);
            throw;
        }
    }

    /// <summary>
    /// Gets the HTML template for password reset OTP email
    /// </summary>
    private static string GetPasswordResetOtpTemplate(string otpCode, int expirationMinutes)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Password Reset OTP</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .logo {{ font-size: 24px; font-weight: bold; color: #2c3e50; }}
        .otp-code {{ font-size: 32px; font-weight: bold; color: #3498db; text-align: center; background-color: #ecf0f1; padding: 20px; border-radius: 5px; margin: 20px 0; letter-spacing: 5px; }}
        .warning {{ color: #e74c3c; font-weight: bold; text-align: center; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #7f8c8d; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>ShopFlow</div>
            <h2>Password Reset Request</h2>
        </div>
        
        <p>Hello,</p>
        
        <p>We received a request to reset your password for your ShopFlow account. Please use the following OTP code to complete the password reset process:</p>
        
        <div class='otp-code'>{otpCode}</div>
        
        <div class='warning'>
            ⚠️ This OTP will expire in {expirationMinutes} minutes
        </div>
        
        <p><strong>Important:</strong></p>
        <ul>
            <li>This OTP is valid for only {expirationMinutes} minutes</li>
            <li>Never share this code with anyone</li>
            <li>If you didn't request this password reset, please ignore this email</li>
            <li>For security reasons, you can only use this code once</li>
        </ul>
        
        <p>If you continue to have problems, please contact our support team.</p>
        
        <div class='footer'>
            <p>This is an automated message from ShopFlow. Please do not reply to this email.</p>
            <p>&copy; 2025 ShopFlow. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Gets the HTML template for password reset confirmation email
    /// </summary>
    private static string GetPasswordResetConfirmationTemplate(DateTime resetDateTime)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Password Reset Confirmation</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .logo {{ font-size: 24px; font-weight: bold; color: #2c3e50; }}
        .success {{ color: #27ae60; font-weight: bold; text-align: center; margin: 20px 0; font-size: 18px; }}
        .info-box {{ background-color: #e8f5e8; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #7f8c8d; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>ShopFlow</div>
            <h2>Password Reset Successful</h2>
        </div>
        
        <div class='success'>
            ✅ Your password has been successfully reset!
        </div>
        
        <p>Hello,</p>
        
        <p>This is a confirmation that your ShopFlow account password was successfully reset on <strong>{resetDateTime:yyyy-MM-dd HH:mm:ss} UTC</strong>.</p>
        
        <div class='info-box'>
            <p><strong>What happened:</strong></p>
            <ul>
                <li>You requested a password reset</li>
                <li>You verified your identity with the OTP code</li>
                <li>Your password has been updated</li>
                <li>All existing sessions have been invalidated for security</li>
            </ul>
        </div>
        
        <p><strong>Security Notes:</strong></p>
        <ul>
            <li>If this wasn't you, please contact our support team immediately</li>
            <li>You can now log in with your new password</li>
            <li>We recommend using a strong, unique password</li>
            <li>Consider enabling two-factor authentication for additional security</li>
        </ul>
        
        <p>Thank you for using ShopFlow!</p>
        
        <div class='footer'>
            <p>This is an automated message from ShopFlow. Please do not reply to this email.</p>
            <p>&copy; 2025 ShopFlow. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }
}
