using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<CoreUser?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<CoreUser?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<CoreUser?> GetByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default);
    Task<IEnumerable<CoreUser>> FindAsync(ISpecification<CoreUser> specification, CancellationToken cancellationToken = default);
    Task<CoreUser> AddAsync(CoreUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(CoreUser user, CancellationToken cancellationToken = default);
    Task DeleteAsync(CoreUser user, CancellationToken cancellationToken = default);
}
