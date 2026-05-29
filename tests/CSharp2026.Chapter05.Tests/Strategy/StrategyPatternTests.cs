using CSharp2026.Chapter05.Strategy;
using CSharp2026.Common.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CSharp2026.Chapter05.Tests.Strategy;

public sealed class StrategyPatternTests
{
    private static PaymentRequest Sample(string provider) =>
        new(OrderId.New(), new Money(10m, "USD"), provider);

    [Fact]
    public async Task StripeProcessor_Returns_Stripe_Prefixed_Transaction()
    {
        StripeProcessor processor = new();
        var result = await processor.ChargeAsync(Sample("stripe"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.TransactionId.Should().StartWith("stripe:");
    }

    [Fact]
    public async Task PayPalProcessor_Returns_PayPal_Prefixed_Transaction()
    {
        PayPalProcessor processor = new();
        var result = await processor.ChargeAsync(Sample("paypal"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.TransactionId.Should().StartWith("paypal:");
    }

    [Fact]
    public async Task Orchestrator_Routes_To_The_Correct_Processor()
    {
        var services = new ServiceCollection()
            .AddPaymentStrategies()
            .BuildServiceProvider();
        var orchestrator = services.GetRequiredService<PaymentOrchestrator>();

        var stripe = await orchestrator.ProcessAsync(Sample("stripe"), CancellationToken.None);
        var paypal = await orchestrator.ProcessAsync(Sample("paypal"), CancellationToken.None);

        stripe.TransactionId.Should().StartWith("stripe:");
        paypal.TransactionId.Should().StartWith("paypal:");
    }

    [Fact]
    public void Factory_Rejects_Unknown_Provider_Key()
    {
        var services = new ServiceCollection()
            .AddPaymentStrategies()
            .BuildServiceProvider();
        var factory = services.GetRequiredService<IPaymentProcessorFactory>();

        var act = () => factory.Resolve("nonexistent");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task Orchestrator_With_Fake_Factory_Is_Easy_To_Unit_Test()
    {
        // Demonstrates the senior-architect benefit of the factory abstraction:
        // we can swap a fake without spinning up the DI container.
        var fakeProcessor = new FakeProcessor();
        var fakeFactory = new FakeFactory(fakeProcessor);
        var orchestrator = new PaymentOrchestrator(fakeFactory);

        var result = await orchestrator.ProcessAsync(Sample("anything"), CancellationToken.None);

        result.TransactionId.Should().Be("fake-txn");
        fakeProcessor.CallCount.Should().Be(1);
    }

    private sealed class FakeProcessor : IPaymentProcessor
    {
        public int CallCount { get; private set; }
        public Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(PaymentResult.Ok("fake-txn"));
        }
    }

    private sealed class FakeFactory(IPaymentProcessor singleton) : IPaymentProcessorFactory
    {
        public IPaymentProcessor Resolve(string providerKey) => singleton;
    }
}
