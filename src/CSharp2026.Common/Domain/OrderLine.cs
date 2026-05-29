// -----------------------------------------------------------------------
//   OrderLine.cs
//   Line item inside an Order — quantity * unit price = subtotal.
// -----------------------------------------------------------------------

using CSharp2026.Common.Events;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Common.Domain;

public enum OrderStatus
{
    Draft,
    Submitted,
    Paid,
    Shipped,
    Delivered,
    Cancelled,
}

/// <summary>Request to add a line to an order — the input shape before validation.</summary>
public sealed record OrderLineRequest(ProductId ProductId, int Quantity, Money UnitPrice);

/// <summary>
/// A line item on an order. Immutable after creation; an order mutates by
/// replacing its lines, not by mutating a line in place.
/// </summary>
public sealed record OrderLine
{
    public ProductId ProductId { get; }
    public int       Quantity  { get; }
    public Money     UnitPrice { get; }
    public Money     Subtotal => UnitPrice * Quantity;

    private OrderLine(ProductId productId, int quantity, Money unitPrice)
    {
        ProductId = productId;
        Quantity  = quantity;
        UnitPrice = unitPrice;
    }

    public static OrderLine Create(OrderLineRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Quantity <= 0)
        {
            throw new DomainException(
                $"Order line quantity must be positive (got {request.Quantity}).");
        }
        if (request.UnitPrice.Amount < 0)
        {
            throw new DomainException(
                $"Order line unit price cannot be negative (got {request.UnitPrice}).");
        }
        return new OrderLine(request.ProductId, request.Quantity, request.UnitPrice);
    }
}
