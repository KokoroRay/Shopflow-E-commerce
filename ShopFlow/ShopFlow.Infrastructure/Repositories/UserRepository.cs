using Microsoft.EntityFrameworkCore;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Infrastructure.Persistence;
using ShopFlow.Application.Abstractions.Mappings;
using PersistenceEntities = ShopFlow.Infrastructure.Persistence.Entities;
using System.Linq;

namespace ShopFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ShopFlowDbContext _context;
    private readonly IDomainPersistenceMapper<CoreUser, PersistenceEntities.CoreUser> _userMapper; // Changed type

    public UserRepository(ShopFlowDbContext context, IDomainPersistenceMapper<CoreUser, PersistenceEntities.CoreUser> userMapper) // Changed type
    {
        _context = context;
        _userMapper = userMapper;
    }

    public async Task<CoreUser?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CoreUsers.FindAsync(new object[] { id }, cancellationToken);
        return _userMapper.ToDomain(dataEntity);
    }

    public async Task<CoreUser?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CoreUsers
            .FirstOrDefaultAsync(u => u.Email == email.Value, cancellationToken);
        return _userMapper.ToDomain(dataEntity);
    }

    public async Task<CoreUser?> GetByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CoreUsers
            .FirstOrDefaultAsync(u => u.Phone != null && u.Phone == phone.Value, cancellationToken);
        return _userMapper.ToDomain(dataEntity);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.CoreUsers.AnyAsync(u => u.Email == email.Value, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneAsync(PhoneNumber phone, CancellationToken cancellationToken = default)
    {
        return await _context.CoreUsers.AnyAsync(u => u.Phone != null && u.Phone == phone.Value, cancellationToken);
    }

    public async Task<IEnumerable<CoreUser>> FindAsync(ISpecification<CoreUser> specification, CancellationToken cancellationToken = default)
    {
        var dataEntities = await _context.CoreUsers.ToListAsync(cancellationToken);
        var domainEntities = dataEntities.Select(de => _userMapper.ToDomain(de)).ToList();

        var query = domainEntities.AsQueryable();
        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);

        if (specification.OrderBy != null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending != null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.IsPagingEnabled)
            query = query.Skip(specification.Skip).Take(specification.Take);

        return query.ToList();
    }

    public async Task<CoreUser> AddAsync(CoreUser user, CancellationToken cancellationToken = default)
    {
        var dataEntity = _userMapper.ToPersistence(user); // Changed method name
        await _context.CoreUsers.AddAsync(dataEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return _userMapper.ToDomain(dataEntity); // Return the mapped domain entity (with potentially updated ID)
    }

    public async Task UpdateAsync(CoreUser user, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CoreUsers.FindAsync(new object[] { user.Id }, cancellationToken);
        if (dataEntity == null)
        {
            throw new InvalidOperationException($"User with ID {user.Id} not found for update.");
        }
        _userMapper.UpdatePersistence(user, dataEntity);
        _context.CoreUsers.Update(dataEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CoreUser user, CancellationToken cancellationToken = default)
    {
        var dataEntity = await _context.CoreUsers.FindAsync(new object[] { user.Id }, cancellationToken);
        if (dataEntity == null)
        {
            return;
        }
        _context.CoreUsers.Remove(dataEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
