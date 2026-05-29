using CSharp2026.Common.Domain;
using CSharp2026.Common.Events;
using CSharp2026.Common.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CSharp2026.Common.Tests.Domain;

public sealed class OrderTests
{
    private static OrderLineRequest Line(decimal price = 9.99m, int qty = 2) =>
        new(ProductId.New(), qty, new Money(price, "USD"));

    [Fact]
    public void Create_Order_With_Lines_Records_Created_Event()
    {
        var order = Order.Create(CustomerId.New(), [Line()]);

        order.Status.Should().Be(OrderStatus.Draft);
        order.Lines.Should().HaveCount(1);
        order.DomainEvents.Should().ContainSingle()
             .Which.Should().BeOfType<OrderCreatedEvent>();
    }

    [Fact]
    public void Create_Order_Without_Lines_Throws()
    {
        var act = () => Order.Create(CustomerId.New(), Array.Empty<OrderLineRequest>());
        act.Should().Throw<DomainException>().WithMessage("*at least one line*");
    }

    [Fact]
    public void Total_Is_Sum_Of_Subtotals()
    {
        var lines = new[]
        {
            new OrderLineRequest(ProductId.New(), 2, new Money(10m, "USD")),
            new OrderLineRequest(ProductId.New(), 3, new Money(5m,  "USD")),
        };
        var order = Order.Create(CustomerId.New(), lines);

        order.Total.Should().Be(new Money(35m, "USD"));   // 2*10 + 3*5
    }

    [Fact]
    public void Submit_From_Draft_Records_Submitted_Event()
    {
        var order = Order.Create(CustomerId.New(), [Line()]);
        order.Submit();

        order.Status.Should().Be(OrderStatus.Submitted);
        order.DomainEvents.OfType<OrderSubmittedEvent>().Should().ContainSingle();
    }

    [Fact]
    public void Submit_From_Non_Draft_Throws()
    {
        var order = Order.Create(CustomerId.New(), [Line()]);
        order.Submit();
        var act = () => order.Submit();
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_Records_Reason_And_Event()
    {
        var order = Order.Create(CustomerId.New(), [Line()]);
        order.Cancel("customer changed mind");

        order.Status.Should().Be(OrderStatus.Cancelled);
        var ev = order.DomainEvents.OfType<OrderCancelledEvent>().Single();
        ev.Reason.Should().Be("customer changed mind");
    }

    [Fact]
    public void Cancel_With_Blank_Reason_Throws()
    {
        var order = Order.Create(CustomerId.New(), [Line()]);
        var act = () => order.Cancel("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ClearDomainEvents_Removes_All_Events()
    {
        var order = Order.Create(CustomerId.New(), [Line()]);
        order.Submit();
        order.DomainEvents.Should().HaveCount(2);

        order.ClearDomainEvents();
        order.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void OrderLine_Rejects_Zero_Quantity()
    {
        var act = () => OrderLine.Create(new OrderLineRequest(ProductId.New(), 0, new Money(1m, "USD")));
        act.Should().Throw<DomainException>().WithMessage("*quantity*");
    }

    [Fact]
    public void OrderLine_Rejects_Negative_Price()
    {
        var act = () => OrderLine.Create(new OrderLineRequest(ProductId.New(), 1, new Money(-5m, "USD")));
        act.Should().Throw<DomainException>().WithMessage("*unit price*");
    }
}
