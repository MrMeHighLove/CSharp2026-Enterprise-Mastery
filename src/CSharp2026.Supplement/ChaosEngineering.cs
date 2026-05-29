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
// Supplement/ChaosEngineering.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ChaosEngineering.cs
using Polly.Simmy;

// Register chaos policies in DI (enable only in non-production environments)
builder.Services.AddResiliencePipeline("payment-service", (pipeline, context) =>
{
    pipeline
        .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 })
        .AddCircuitBreaker(new CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30)
        });

    // Chaos — enable via config
    if (context.ServiceProvider.GetRequiredService<IConfiguration>()
            .GetValue<bool>("Chaos:Enabled"))
    {
        pipeline
            // Inject latency: 30% of calls delayed by 2-5 seconds
            .AddChaosLatency(new ChaosLatencyStrategyOptions
            {
                EnabledGenerator = _ => ValueTask.FromResult(true),
                InjectionRateGenerator = _ => ValueTask.FromResult(0.3),
                LatencyGenerator = _ => ValueTask.FromResult(
                    TimeSpan.FromSeconds(Random.Shared.Next(2, 5)))
            })
            // Inject faults: 10% of calls throw
            .AddChaosFault(new ChaosFaultStrategyOptions
            {
                InjectionRateGenerator = _ => ValueTask.FromResult(0.1),
                FaultGenerator = _ => ValueTask.FromResult<Exception?>(
                    new HttpRequestException("Chaos fault injected"))
            });
    }
});

// Use in tests to verify resilience
public class PaymentServiceChaosTests
{
    [Fact]
    public async Task CircuitBreaker_Opens_After_Failures()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 5
            })
            .AddChaosFault(1.0, () => new Exception("Always fails"))
            .Build();

        // After enough failures, circuit should open
        for (int i = 0; i < 5; i++)
        {
            try { await pipeline.ExecuteAsync(_ => CallPaymentApiAsync()); }
            catch { /* expected */ }
        }

        await Assert.ThrowsAsync<BrokenCircuitException>(
            () => pipeline.ExecuteAsync(_ => CallPaymentApiAsync()));
    }
}

#pragma warning restore
