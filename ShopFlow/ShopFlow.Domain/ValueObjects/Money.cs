namespace ShopFlow.Domain.ValueObjects;

public class Money : IEquatable<Money>, IComparable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "VND")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));
        
        Amount = Math.Round(amount, 4, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "VND") => new(0, currency);
    
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        
        return new Money(left.Amount + right.Amount, left.Currency);
    }
    
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        
        return new Money(left.Amount - right.Amount, left.Currency);
    }
    
    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }
    
    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();
        
        return new Money(money.Amount / divisor, money.Currency);
    }

    public static implicit operator decimal(Money money) => money.Amount;
    
    public static explicit operator Money(decimal amount) => new(amount);

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Money);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");
        
        return Amount.CompareTo(other.Amount);
    }

    public override string ToString()
    {
        return $"{Amount:F4} {Currency}";
    }

    public static bool operator ==(Money? left, Money? right)
    {
        return EqualityComparer<Money>.Default.Equals(left, right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !(left == right);
    }

    public static bool operator <(Money left, Money right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Money left, Money right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Money left, Money right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Money left, Money right)
    {
        return left.CompareTo(right) >= 0;
    }
}
