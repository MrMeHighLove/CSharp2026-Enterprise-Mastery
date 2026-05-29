using CSharp2026.Chapter05.Decorator;
using CSharp2026.Chapter05.Strategy;
using CSharp2026.Common.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CSharp2026.Chapter05.Tests.Decorator;

public sealed class DecoratorPatternTests
{
    private static PaymentRequest Sample() =>
        new(OrderId.New(), new Money(10m, "USD"), "stripe");

    [Fact]
    public async Task LoggingDecorator_Delegates_To_Inner_And_Returns_Its_Result()
    {
        var inner = new CountingProcessor(success: true);
        var decorated = new LoggingPaymentProcessor(inner, NullLogger<LoggingPaymentProcessor>.Instance);

        var result = await decorated.ChargeAsync(Sample(), CancellationToken.None);

        result.Success.Should().BeTrue();
        inner.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task RetryDecorator_Returns_Success_On_First_Attempt_When_Possible()
    {
        var inner = new CountingProcessor(success: true);
        var decorated = new RetryingPaymentProcessor(inner, maxAttempts: 3);

        var result = await decorated.ChargeAsync(Sample(), CancellationToken.None);

        result.Success.Should().BeTrue();
        inner.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task RetryDecorator_Retries_Up_To_Max_Attempts_On_Failure()
    {
        var inner = new CountingProcessor(success: false);
        var decorated = new RetryingPaymentProcessor(inner, maxAttempts: 3);

        var result = await decorated.ChargeAsync(Sample(), CancellationToken.None);

        result.Success.Should().BeFalse();
        inner.CallCount.Should().Be(3);
    }

    [Fact]
    public async Task RetryDecorator_Stops_On_First_Success()
    {
        var inner = new FailNThenSucceedProcessor(failFor: 2);
        var decorated = new RetryingPaymentProcessor(inner, maxAttempts: 5);

        var result = await decorated.ChargeAsync(Sample(), CancellationToken.None);

        result.Success.Should().BeTrue();
        inner.CallCount.Should().Be(3);    // 2 failures + 1 success
    }

    [Fact]
    public void RetryDecorator_Rejects_Invalid_MaxAttempts()
    {
        var inner = new CountingProcessor(success: true);
        var act = async () =>
            await new RetryingPaymentProcessor(inner, maxAttempts: 0)
                .ChargeAsync(Sample(), CancellationToken.None);
        act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task Decorators_Compose_Correctly_When_Stacked()
    {
        var inner   = new FailNThenSucceedProcessor(failFor: 1);
        var logging = new LoggingPaymentProcessor(inner, NullLogger<LoggingPaymentProcessor>.Instance);
        var retry   = new RetryingPaymentProcessor(logging, maxAttempts: 3);

        var result = await retry.ChargeAsync(Sample(), CancellationToken.None);

        result.Success.Should().BeTrue();
        inner.CallCount.Should().Be(2);
    }

    private sealed class CountingProcessor(bool success) : IPaymentProcessor
    {
        public int CallCount { get; private set; }
        public Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(success ? PaymentResult.Ok("ok") : PaymentResult.Fail("nope"));
        }
    }

    private sealed class FailNThenSucceedProcessor(int failFor) : IPaymentProcessor
    {
        public int CallCount { get; private set; }
        public Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(
                CallCount <= failFor ? PaymentResult.Fail("transient") : PaymentResult.Ok("ok"));
        }
    }
}
