// -----------------------------------------------------------------------
//   Decorator.cs — Chapter 5 / Decorator Pattern
//
//   Decorators add cross-cutting behaviour (logging, retry, caching)
//   without modifying the wrapped class. Each decorator implements the
//   same interface as the thing it wraps, so the caller doesn't notice.
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using CSharp2026.Chapter05.Strategy;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Chapter05.Decorator;

/// <summary>
/// Logs each payment attempt and its outcome. Wraps another IPaymentProcessor.
/// </summary>
public sealed class LoggingPaymentProcessor(
    IPaymentProcessor inner,
    ILogger<LoggingPaymentProcessor> logger) : IPaymentProcessor
{
    private static readonly Action<ILogger, Money, OrderId, string, Exception?> ChargingPayment =
        LoggerMessage.Define<Money, OrderId, string>(
            LogLevel.Information,
            new EventId(1001, nameof(ChargingPayment)),
            "Charging {Amount} for order {OrderId} via {Provider}");

    private static readonly Action<ILogger, OrderId, string, Exception?> ChargeSucceeded =
        LoggerMessage.Define<OrderId, string>(
            LogLevel.Information,
            new EventId(1002, nameof(ChargeSucceeded)),
            "Charge succeeded for order {OrderId}: {TxnId}");

    private static readonly Action<ILogger, OrderId, string, Exception?> ChargeFailed =
        LoggerMessage.Define<OrderId, string>(
            LogLevel.Warning,
            new EventId(1003, nameof(ChargeFailed)),
            "Charge failed for order {OrderId}: {Error}");

    public async Task<PaymentResult> ChargeAsync(
        PaymentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ChargingPayment(logger, request.Amount, request.OrderId, request.Provider, null);

        PaymentResult result = await inner.ChargeAsync(request, cancellationToken).ConfigureAwait(false);

        if (result.Success)
        {
            ChargeSucceeded(logger, request.OrderId, result.TransactionId, null);
        }
        else
        {
            ChargeFailed(logger, request.OrderId, result.Error ?? "Unknown error", null);
        }
        return result;
    }
}

/// <summary>
/// Retries a transient failure a bounded number of times. Real production
/// retry should use Polly (see Chapter 16); this is the pattern in pure
/// form for teaching.
/// </summary>
public sealed class RetryingPaymentProcessor(
    IPaymentProcessor inner,
    int maxAttempts = 3) : IPaymentProcessor
{
    public async Task<PaymentResult> ChargeAsync(
        PaymentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxAttempts, 1);

        PaymentResult last = PaymentResult.Fail("never attempted");
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            last = await inner.ChargeAsync(request, cancellationToken).ConfigureAwait(false);
            if (last.Success)
            {
                return last;
            }
            // (Real code would back off here; intentionally simple for teaching.)
        }
        return last;
    }
}
