using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Infrastructure.Persistence;
using ShopFlow.Application.Specifications.Users;

namespace ShopFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ShopFlowDbContext _context;
    private readonly IUserMapper _mapper;

    public UserRepository(ShopFlowDbContext context, IUserMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CoreUser?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.core_users.FindAsync(new object[] { id }, cancellationToken);
        return dataEntity != null ? _mapper.ToDomain(dataEntity) : null;
    }

    public async Task<CoreUser?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.core_users
            .FirstOrDefaultAsync(u => u.email == email.Value, cancellationToken);
        return dataEntity != null ? _mapper.ToDomain(dataEntity) : null;
    }

    public async Task<CoreUser?> GetByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.core_users
            .FirstOrDefaultAsync(u => u.phone == phone.Value, cancellationToken);
        return dataEntity != null ? _mapper.ToDomain(dataEntity) : null;
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.core_users
            .AnyAsync(u => u.email == email.Value, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default)
    {
        return await _context.core_users
            .AnyAsync(u => u.phone == phone.Value, cancellationToken);
    }

    public async Task<IEnumerable<CoreUser>> FindAsync(ISpecification<CoreUser> specification, CancellationToken cancellationToken = default)
    {
        // This would require converting domain specification to EF specification
        // For now, return empty collection
        await Task.CompletedTask;
        return new List<CoreUser>();
    }

    public async Task<CoreUser> AddAsync(CoreUser user, CancellationToken cancellationToken = default)
    {
        var dataEntity = _mapper.ToData(user);
        var entry = await _context.core_users.AddAsync(dataEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Update domain entity with generated ID
        var updatedDomainEntity = _mapper.ToDomain(entry.Entity);
        return updatedDomainEntity;
    }

    public async Task UpdateAsync(CoreUser user, CancellationToken cancellationToken = default)
    {
        var existingDataEntity = await _context.core_users.FindAsync(new object[] { user.Id }, cancellationToken);
        if (existingDataEntity != null)
        {
            _mapper.UpdateData(user, existingDataEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(CoreUser user, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.core_users.FindAsync(new object[] { user.Id }, cancellationToken);
        if (dataEntity != null)
        {
            _context.core_users.Remove(dataEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
