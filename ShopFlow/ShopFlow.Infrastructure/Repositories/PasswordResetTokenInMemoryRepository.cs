using Microsoft.Extensions.Caching.Memory;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of password reset token repository using IMemoryCache
/// This implementation stores tokens in memory without requiring database changes
/// </summary>
public class PasswordResetTokenInMemoryRepository : IPasswordResetTokenRepository
{
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "password_reset_token";
    private const string EmailIndexPrefix = "password_reset_email";

    public PasswordResetTokenInMemoryRepository(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Gets a password reset token by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The most recent active token for the email</returns>
    public Task<PasswordResetToken?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var emailKey = $"{EmailIndexPrefix}:{email.Value.ToLowerInvariant()}";

        if (_cache.TryGetValue(emailKey, out Guid tokenId))
        {
            var tokenKey = $"{CacheKeyPrefix}:{tokenId}";
            if (_cache.TryGetValue(tokenKey, out PasswordResetToken? token) &&
                token != null && !token.IsUsed && !token.IsExpired())
            {
                return Task.FromResult<PasswordResetToken?>(token);
            }
            else
            {
                // Clean up expired or used token references
                _cache.Remove(emailKey);
                if (token != null)
                {
                    _cache.Remove(tokenKey);
                }
            }
        }

        return Task.FromResult<PasswordResetToken?>(null);
    }

    /// <summary>
    /// Gets a password reset token by its ID
    /// </summary>
    /// <param name="id">Token ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The token if found and valid</returns>
    public Task<PasswordResetToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tokenKey = $"{CacheKeyPrefix}:{id}";

        if (_cache.TryGetValue(tokenKey, out PasswordResetToken? token) &&
            token != null && !token.IsUsed && !token.IsExpired())
        {
            return Task.FromResult<PasswordResetToken?>(token);
        }

        // Clean up expired or used token
        if (token != null)
        {
            _cache.Remove(tokenKey);
            var emailKey = $"{EmailIndexPrefix}:{token.Email.Value.ToLowerInvariant()}";
            _cache.Remove(emailKey);
        }

        return Task.FromResult<PasswordResetToken?>(null);
    }

    /// <summary>
    /// Adds a new password reset token to cache
    /// </summary>
    /// <param name="token">The token to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added token</returns>
    public Task<PasswordResetToken> AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        var tokenKey = $"{CacheKeyPrefix}:{token.Id}";
        var emailKey = $"{EmailIndexPrefix}:{token.Email.Value.ToLowerInvariant()}";

        // Remove any existing token for this email first
        if (_cache.TryGetValue(emailKey, out Guid existingTokenId))
        {
            var existingTokenKey = $"{CacheKeyPrefix}:{existingTokenId}";
            _cache.Remove(existingTokenKey);
        }

        // Store token with expiration time
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = token.OtpCode.ExpiresAt,
            Priority = CacheItemPriority.Normal,
            Size = 1 // Help with cache eviction
        };

        _cache.Set(tokenKey, token, cacheOptions);
        _cache.Set(emailKey, token.Id, cacheOptions);

        return Task.FromResult(token);
    }

    /// <summary>
    /// Updates an existing password reset token in cache
    /// </summary>
    /// <param name="token">The token to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated token</returns>
    public Task<PasswordResetToken> UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        var tokenKey = $"{CacheKeyPrefix}:{token.Id}";
        var emailKey = $"{EmailIndexPrefix}:{token.Email.Value.ToLowerInvariant()}";

        // Update token with same expiration
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = token.OtpCode.ExpiresAt,
            Priority = CacheItemPriority.Normal,
            Size = 1
        };

        _cache.Set(tokenKey, token, cacheOptions);
        _cache.Set(emailKey, token.Id, cacheOptions);

        return Task.FromResult(token);
    }

    /// <summary>
    /// Deletes expired tokens from cache
    /// Note: Memory cache automatically removes expired entries, so this is a no-op
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Completed task</returns>
    public Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        // Memory cache automatically removes expired entries
        // No action needed as expired entries are automatically cleaned up
        return Task.CompletedTask;
    }
}
