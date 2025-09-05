namespace ShopFlow.Application.Abstractions.Services;

public interface ICurrentUserService
{
    long? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    List<string> Roles { get; }
}
