using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Services;

namespace ShopFlow.Infrastructure.Services;

public class BaseService<T> : IBaseService<T>
    where T : ShopFlow.Domain.Entities.BaseEntity
{
    private readonly IRepository<T> _repository;

    public BaseService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);

    public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);

    public Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        => _repository.AddAsync(entity, cancellationToken);

    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(entity, cancellationToken);

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);

    public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
        => _repository.ExistsAsync(id, cancellationToken);
}
