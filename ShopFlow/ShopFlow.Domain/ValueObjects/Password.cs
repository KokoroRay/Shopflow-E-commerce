namespace ShopFlow.Domain.ValueObjects;

public record Password
{
    public string Value { get; }

    public Password(string value)
    {
        // Add validation logic here if needed
        Value = value;
    }
}
