using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Application.Specifications.Users;

public class UserByEmailSpecification : BaseSpecification<CoreUser>
{
    public UserByEmailSpecification(Email email) 
        : base(user => user.Email == email)
    {
        AddInclude(user => user.UserRoles);
        AddInclude(user => user.CustomerProfile);
    }
}

public class UserByPhoneSpecification : BaseSpecification<CoreUser>
{
    public UserByPhoneSpecification(PhoneNumber phone) 
        : base(user => user.Phone == phone)
    {
    }
}

public class ActiveUsersSpecification : BaseSpecification<CoreUser>
{
    public ActiveUsersSpecification() 
        : base(user => user.Status == Domain.Enums.UserStatus.Active)
    {
        ApplyOrderByDescending(user => user.CreatedAt);
    }
}

public class UsersPaginatedSpecification : BaseSpecification<CoreUser>
{
    public UsersPaginatedSpecification(int pageIndex, int pageSize)
        : base(null)
    {
        ApplyPaging(pageIndex * pageSize, pageSize);
        ApplyOrderByDescending(user => user.CreatedAt);
    }
}
