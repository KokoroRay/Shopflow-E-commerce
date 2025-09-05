using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Abstractions.Mappings;

public interface IEntityMapper<TDomain, TData>
{
    TDomain ToDomain(TData dataEntity);
    TData ToData(TDomain domainEntity);
    void UpdateData(TDomain domainEntity, TData dataEntity);
}

public interface IUserMapper : IEntityMapper<CoreUser, core_user>
{
    // Additional mapping methods if needed
}

public interface IProductMapper : IEntityMapper<CatProduct, cat_product>
{
    // Additional mapping methods if needed
}
