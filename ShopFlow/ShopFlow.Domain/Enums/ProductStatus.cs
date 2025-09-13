namespace ShopFlow.Domain.Enums;

public enum ProductStatus : byte
{
    Draft = 0,
    Active = 1,
    Inactive = 2,
    Discontinued = 3,
    // Vietnamese Marketplace approval workflow statuses
    Pending = 4,        // Chờ duyệt - waiting for approval
    UnderReview = 5,    // Đang xem xét - under review by admin
    Rejected = 6        // Bị từ chối - rejected by admin
}
