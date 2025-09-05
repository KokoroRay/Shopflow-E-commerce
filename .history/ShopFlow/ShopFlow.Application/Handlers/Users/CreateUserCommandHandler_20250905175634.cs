using MediatR;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;
using ShopFlow.Domain.Enums;

namespace ShopFlow.Application.Handlers.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = CoreUser.Create(
            Email.Create(request.Email),
            PhoneNumber.Create(request.Phone),
            _passwordHasher.HashPassword(request.Password),
            UserStatus.Active
        );

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email.Value,
            Phone = user.Phone?.Value,
            Status = (byte)user.Status,
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreatedAt,
            FullName = request.FullName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Roles = new List<string>()
        };
    }
}
