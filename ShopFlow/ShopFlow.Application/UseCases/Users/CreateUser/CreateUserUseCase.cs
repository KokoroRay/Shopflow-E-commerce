using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Application.Abstractions.Security;
using ShopFlow.Application.Contracts.Requests;
using ShopFlow.Application.Contracts.Response;
using ShopFlow.Domain.Entities;
using ShopFlow.Application.Abstractions.Mappings;

namespace ShopFlow.Application.UseCases.Users;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IRepository<core_user> _userRepository;
    private readonly IRepository<role_customer_profile> _customerProfileRepository;
    private readonly IRepository<core_role> _roleRepository;
    private readonly IRepository<core_user_role> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserMapper _userMapper;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserUseCase(
        IRepository<core_user> userRepository,
        IRepository<role_customer_profile> customerProfileRepository,
        IRepository<core_role> roleRepository,
        IRepository<core_user_role> userRoleRepository,
        IUnitOfWork unitOfWork,
        IUserMapper userMapper,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _customerProfileRepository = customerProfileRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _userMapper = userMapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
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

            var user = new core_user
            {
                email = request.Email,
                phone = request.Phone,
                password_hash = _passwordHasher.HashPassword(request.Password),
                status = 1,
                email_verified = false,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);

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

