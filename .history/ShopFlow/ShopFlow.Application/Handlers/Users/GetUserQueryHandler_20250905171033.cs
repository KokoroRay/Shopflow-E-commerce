using MediatR;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Queries.Users;
using ShopFlow.Application.Contracts.Response;

namespace ShopFlow.Application.Handlers.Users;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMapper _userMapper;

    public GetUserQueryHandler(IUserRepository userRepository, IUserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<UserResponse?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return null;

        var dataEntity = _userMapper.ToData(user);
        return _userMapper.ToUserResponse(dataEntity);
    }
}
