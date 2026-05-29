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

namespace CSharp2026.Supplement;

#pragma warning disable
// Supplement/PollyResilience.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/PollyResilience.cs
// Build a reusable resilience pipeline
var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
    // 1. Total timeout for the entire operation
    .AddTimeout(TimeSpan.FromSeconds(10))

    // 2. Retry with exponential back-off and jitter
    .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
    {
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        Delay = TimeSpan.FromMilliseconds(200),
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(r => r.StatusCode == HttpStatusCode.ServiceUnavailable
                           || r.StatusCode == HttpStatusCode.TooManyRequests),
        OnRetry = args =>
        {
            _logger.LogWarning("Retry {Attempt} after {Delay}",
                args.AttemptNumber, args.RetryDelay);
            return ValueTask.CompletedTask;
        }
    })

    // 3. Circuit breaker — opens after 50% failure rate
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
    {
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 10,
        BreakDuration = TimeSpan.FromSeconds(30),
        OnOpened = args =>
        {
            _logger.LogError("Circuit opened: {Reason}", args.Outcome.Exception?.Message);
            return ValueTask.CompletedTask;
        }
    })

    // 4. Per-attempt timeout (prevents a single slow call from blocking retries)
    .AddTimeout(TimeSpan.FromSeconds(3))

    .Build();

// Register named pipelines in DI
builder.Services.AddResiliencePipeline<string, HttpResponseMessage>(
    "payment-service", (builder, _) =>
    {
        builder
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(500)
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = 0.3,
                SamplingDuration = TimeSpan.FromSeconds(60)
            });
    });

// Inject and use
public class PaymentGatewayClient
{
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    public PaymentGatewayClient(ResiliencePipelineProvider<string> provider)
    {
        _pipeline = provider.GetPipeline<HttpResponseMessage>("payment-service");
    }

    public async Task<PaymentResult> ChargeAsync(ChargeRequest request, CancellationToken ct)
    {
        var response = await _pipeline.ExecuteAsync(
            async token => await _http.PostAsJsonAsync("/charge", request, token),
            ct);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaymentResult>(ct) ?? default!;
    }
}

#pragma warning restore
