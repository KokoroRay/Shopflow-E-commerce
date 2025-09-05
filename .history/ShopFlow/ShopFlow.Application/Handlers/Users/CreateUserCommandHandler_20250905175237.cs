using MediatR;
using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Abstractions.Mappings;
using ShopFlow.Application.Abstractions.Messaging;
using ShopFlow.Application.Commands.Users;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Application.Specifications.Users;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Handlers.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserMapper _userMapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserMapper userMapper,
        IPasswordHasher passwordHasher,
        IDomainEventPublisher domainEventPublisher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userMapper = userMapper;
        _passwordHasher = passwordHasher;
        _domainEventPublisher = domainEventPublisher;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        
        // Check if user already exists using domain-specific repository
        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists");
        }

        if (!string.IsNullOrEmpty(request.Phone))
        {
            var phone = new PhoneNumber(request.Phone);
            if (await _userRepository.ExistsByPhoneAsync(phone, cancellationToken))
            {
                throw new InvalidOperationException($"User with phone {request.Phone} already exists");
            }
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Create domain user
            var phone = !string.IsNullOrEmpty(request.Phone) ? new PhoneNumber(request.Phone) : null;
            var user = new CoreUser(email, _passwordHasher.HashPassword(request.Password), phone);

            // Add user using domain repository
            var createdUser = await _userRepository.AddAsync(user, cancellationToken);

            // Publish domain events
            foreach (var domainEvent in user.DomainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }
            
            user.ClearDomainEvents();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Map to response using updated mapper
            var dataEntity = _userMapper.ToData(createdUser);
            return _userMapper.ToUserResponse(dataEntity);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userRepository.FindAsync(u => u.email == request.Email, cancellationToken);
        if (existingUser.Any())
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists");
        }

        if (!string.IsNullOrEmpty(request.Phone))
        {
            var existingPhoneUser = await _userRepository.FindAsync(u => u.phone == request.Phone, cancellationToken);
            if (existingPhoneUser.Any())
            {
                throw new InvalidOperationException($"User with phone {request.Phone} already exists");
            }
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Create domain user
            var email = new Email(request.Email);
            var phone = !string.IsNullOrEmpty(request.Phone) ? new PhoneNumber(request.Phone) : null;
            var user = new CoreUser(email, _passwordHasher.HashPassword(request.Password), phone);

            // For now, we'll still use the old entities for persistence
            // This should be refactored to use the new domain entities
            var coreUser = new core_user
            {
                email = request.Email,
                phone = request.Phone,
                password_hash = _passwordHasher.HashPassword(request.Password),
                status = 1,
                email_verified = false,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(coreUser, cancellationToken);

            var customerProfile = new role_customer_profile
            {
                user_id = createdUser.id,
                full_name = request.FullName,
                gender = request.Gender,
                created_at = DateTime.UtcNow
            };

            await _customerProfileRepository.AddAsync(customerProfile, cancellationToken);

            var customerRole = await _roleRepository.FindAsync(r => r.code == "CUSTOMER", cancellationToken);
            if (!customerRole.Any())
            {
                throw new InvalidOperationException("Customer role not found");
            }

            var userRole = new core_user_role
            {
                user_id = createdUser.id,
                role_id = customerRole.First().id
            };

            await _userRoleRepository.AddAsync(userRole, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return await _userMapper.MapToUserResponseAsync(createdUser.id, cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
