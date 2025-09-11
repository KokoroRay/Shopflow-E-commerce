namespace ShopFlow.Domain.Enums;

/// <summary>
/// Represents the status of a category in the system
/// </summary>
public enum CategoryStatus : byte
{
    /// <summary>
    /// Category is active and visible to users
    /// </summary>
    Active = 1,

    /// <summary>
    /// Category is inactive and hidden from users
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Category is soft-deleted
    /// </summary>
    Deleted = 3
}