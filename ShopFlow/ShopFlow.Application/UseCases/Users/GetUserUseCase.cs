using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Mappings;

namespace ShopFlow.Application.UseCases.Users;

public class GetUserUseCase : IGetUserUseCase
{
    private readonly IRepository<core_user> _userRepository;
    private readonly IUserMapper _userMapper;

    public GetUserUseCase(
        IRepository<core_user> userRepository,
        IUserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<UserResponse?> ExecuteAsync(long id, CancellationToken cancellationToken = default)
    {
        // core_user được đảm bảo bởi GlobalUsings => ShopFlow.Domain.Entities.core_user
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return null;

        return await _userMapper.MapToUserResponseAsync(id, cancellationToken);
    }
}


