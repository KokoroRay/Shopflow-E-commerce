using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Abstractions.Repositories;

/// <summary>
/// Repository interface for managing password reset tokens
/// </summary>
public interface IPasswordResetTokenRepository
{
    /// <summary>
    /// Gets a password reset token by its ID
    /// </summary>
    /// <param name="id">The token ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The password reset token if found, null otherwise</returns>
    Task<PasswordResetToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest active password reset token for an email
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The latest active token if found, null otherwise</returns>
    Task<PasswordResetToken?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new password reset token
    /// </summary>
    /// <param name="token">The token to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added token</returns>
    Task<PasswordResetToken> AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing password reset token
    /// </summary>
    /// <param name="token">The token to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated token</returns>
    Task<PasswordResetToken> UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the cleanup operation</returns>
    Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);
}
