using ShopFlow.Domain.Entities;
using ShopFlow.Domain.ValueObjects;

namespace ShopFlow.Domain.DomainServices;

public class PricingService
{
    public Money CalculatePrice(MktOffer offer, int quantity)
    {
        if (offer == null) throw new ArgumentNullException(nameof(offer));
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var basePrice = offer.PriceGrossVnd ?? 0m;
        var totalPrice = basePrice * quantity;

        // Apply quantity discounts if applicable
        if (offer.MinQty.HasValue && quantity >= offer.MinQty.Value)
        {
            // Could apply bulk discount logic here
        }

        return new Money(totalPrice, "VND");
    }

    public Money CalculateTax(Money amount, decimal taxRate)
    {
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        if (taxRate < 0 || taxRate > 1) throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        return amount * taxRate;
    }

    public Money CalculateShippingFee(Money orderValue, string shippingZone, decimal weight)
    {
        if (orderValue == null) throw new ArgumentNullException(nameof(orderValue));
        if (string.IsNullOrWhiteSpace(shippingZone)) throw new ArgumentException("Shipping zone is required", nameof(shippingZone));
        if (weight <= 0) throw new ArgumentException("Weight must be positive", nameof(weight));

        // This would typically look up shipping rates from a service
        // For now, return a simple calculation
        var baseFee = new Money(30000, "VND"); // Base shipping fee
        var weightFee = new Money((decimal)(weight * 5000), "VND"); // 5000 VND per kg

        return baseFee + weightFee;
    }

    public Money ApplyDiscount(Money amount, decimal discountPercentage)
    {
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        if (discountPercentage < 0 || discountPercentage > 1) 
            throw new ArgumentException("Discount percentage must be between 0 and 1", nameof(discountPercentage));

        var discountAmount = amount * discountPercentage;
        return amount - discountAmount;
    }

    public Money CalculateTotal(Money subtotal, Money tax, Money shipping, Money discount)
    {
        if (subtotal == null) throw new ArgumentNullException(nameof(subtotal));
        if (tax == null) throw new ArgumentNullException(nameof(tax));
        if (shipping == null) throw new ArgumentNullException(nameof(shipping));
        if (discount == null) throw new ArgumentNullException(nameof(discount));

        return subtotal + tax + shipping - discount;
    }

    public bool IsValidPrice(Money price)
    {
        return price != null && price.Amount >= 0;
    }
}
