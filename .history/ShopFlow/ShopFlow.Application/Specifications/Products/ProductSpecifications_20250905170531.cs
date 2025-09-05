using ShopFlow.Application.Abstractions.Repositories;
using ShopFlow.Domain.Entities;

namespace ShopFlow.Application.Specifications.Products;

public class ProductByCategorySpecification : BaseSpecification<CatProduct>
{
    public ProductByCategorySpecification(long categoryId) 
        : base(product => product.CategoryId == categoryId)
    {
        AddInclude(product => product.Category);
        AddInclude(product => product.Skus);
    }
}

public class ProductByNameSpecification : BaseSpecification<CatProduct>
{
    public ProductByNameSpecification(string productName) 
        : base(product => product.Name.Contains(productName))
    {
        ApplyOrderBy(product => product.Name);
    }
}

public class ActiveProductsSpecification : BaseSpecification<CatProduct>
{
    public ActiveProductsSpecification() 
        : base(product => product.Status == Domain.Enums.ProductStatus.Active)
    {
        ApplyOrderByDescending(product => product.CreatedAt);
    }
}

public class ProductsPaginatedSpecification : BaseSpecification<CatProduct>
{
    public ProductsPaginatedSpecification(int pageIndex, int pageSize, long? categoryId = null)
        : base(categoryId.HasValue ? product => product.CategoryId == categoryId.Value : null)
    {
        ApplyPaging(pageIndex * pageSize, pageSize);
        ApplyOrderByDescending(product => product.CreatedAt);
        AddInclude(product => product.Category);
    }
}
