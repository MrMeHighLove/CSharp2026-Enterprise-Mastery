// -----------------------------------------------------------------------
//   Strategy.cs — Chapter 5 / Strategy Pattern
//
//   Strategy lets you swap an algorithm at runtime through a stable
//   interface. The classic example is payment processing: many
//   processors, one orchestrator that picks one by key.
//
//   The book's Service-Locator anti-pattern fix is captured here too:
//   the orchestrator takes an explicit IPaymentProcessorFactory rather
//   than IServiceProvider, so the dependency is visible at the call site.
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Chapter05.Strategy;

/// <summary>Input to a payment attempt.</summary>
public sealed record PaymentRequest(
    OrderId  OrderId,
    Money    Amount,
    string   Provider);   // "stripe", "paypal", ...

/// <summary>Outcome of a payment attempt.</summary>
public sealed record PaymentResult(
    bool   Success,
    string TransactionId,
    string? Error = null)
{
    public static PaymentResult Ok(string txn)        => new(true, txn);
    public static PaymentResult Fail(string reason)   => new(false, string.Empty, reason);
}

/// <summary>The strategy interface — one method, multiple implementations.</summary>
public interface IPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken);
}

public sealed class StripeProcessor : IPaymentProcessor
{
    public Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        // Real implementation would call Stripe's API.
        return Task.FromResult(PaymentResult.Ok($"stripe:{Guid.NewGuid():N}"));
    }
}

public sealed class PayPalProcessor : IPaymentProcessor
{
    public Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Task.FromResult(PaymentResult.Ok($"paypal:{Guid.NewGuid():N}"));
    }
}

/// <summary>
/// Single, intentional resolution point for keyed payment processors.
/// Hides IServiceProvider so business code doesn't take a dependency on
/// the entire container.
/// </summary>
public interface IPaymentProcessorFactory
{
    IPaymentProcessor Resolve(string providerKey);
}

public sealed class PaymentProcessorFactory(IServiceProvider serviceProvider)
    : IPaymentProcessorFactory
{
    public IPaymentProcessor Resolve(string providerKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        return serviceProvider.GetRequiredKeyedService<IPaymentProcessor>(providerKey);
    }
}

/// <summary>
/// Orchestrates payments by delegating to the right strategy. Depends on
/// the factory abstraction, not the container — easy to unit-test by
/// passing a fake factory.
/// </summary>
public sealed class PaymentOrchestrator(IPaymentProcessorFactory processors)
{
    public async Task<PaymentResult> ProcessAsync(
        PaymentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var processor = processors.Resolve(request.Provider);
        return await processor.ChargeAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Composition root extension — keep DI wiring in one place per chapter.
/// </summary>
public static class StrategyServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentStrategies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddKeyedScoped<IPaymentProcessor, StripeProcessor>("stripe");
        services.AddKeyedScoped<IPaymentProcessor, PayPalProcessor>("paypal");
        services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();
        services.AddScoped<PaymentOrchestrator>();
        return services;
    }
}
