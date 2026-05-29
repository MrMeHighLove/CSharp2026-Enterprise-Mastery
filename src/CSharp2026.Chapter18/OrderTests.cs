// AUTO-WRAPPED for compilation. Original snippet content follows the
// namespace declaration. Snippets are illustrative and may reference
// types that need to be supplied by the chapter or by the reader.
// See ISSUES.md for the catalog of known gaps.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharp2026.Common.Domain;
using CSharp2026.Common.Events;
using CSharp2026.Common.Results;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Chapter18;

#pragma warning disable
// Chapter18/OrderTests.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// xUnit + FluentAssertions: clean, readable test assertions
public class OrderTests
{
    [Fact]
    public void Create_WithValidItems_ShouldSetStatusDraft()
    {
        // Arrange
        var customerId = CustomerId.New();
        var items = new List<OrderLineRequest>
        {
            new(ProductId.New(), quantity: 2, unitPrice: new Money(50m, "USD")),
        };

        // Act
        var order = Order.Create(customerId, items);

        // Assert — FluentAssertions for readable failures
        order.Status.Should().Be(OrderStatus.Draft);
        order.Lines.Should().HaveCount(1);
        order.Total.Amount.Should().Be(100m);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderCreatedEvent>();
    }

    [Fact]
    public void Submit_WhenDraft_ShouldTransitionToSubmitted()
    {
        var order = OrderFactory.CreateDraftOrder();

        order.Submit();

        order.Status.Should().Be(OrderStatus.Submitted);
        order.DomainEvents.Should().Contain(e => e is OrderSubmittedEvent);
    }

    [Fact]
    public void Submit_WhenAlreadySubmitted_ShouldThrowDomainException()
    {
        var order = OrderFactory.CreateSubmittedOrder();

        var act = () => order.Submit();

        act.Should().Throw<DomainException>()
            .WithMessage("*status*");
    }
}

#pragma warning restore
