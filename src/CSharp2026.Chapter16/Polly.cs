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

namespace CSharp2026.Chapter16;

#pragma warning disable
// Chapter16/Polly.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Polly v8: composable resilience pipeline registered via DI
builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
{
    client.BaseAddress = new Uri("https://inventory-service/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddResilienceHandler("inventory-pipeline", pipeline =>
{
    // 1. Retry: exponential backoff, up to 3 attempts
    pipeline.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay    = TimeSpan.FromMilliseconds(200),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter   = true,   // prevent thundering herd
        ShouldHandle = args => ValueTask.FromResult(
            args.Outcome.Exception is not null ||
            (args.Outcome.Result?.StatusCode is HttpStatusCode.TooManyRequests
                or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout)),
    });

    // 2. Circuit Breaker: open after 5 failures in 30 seconds
    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        SamplingDuration       = TimeSpan.FromSeconds(30),
        MinimumThroughput      = 10,
        FailureRatio           = 0.5,  // 50% failure rate trips the circuit
        BreakDuration          = TimeSpan.FromSeconds(15),
    });

    // 3. Timeout per attempt
    pipeline.AddTimeout(TimeSpan.FromSeconds(5));
});

// Typed client — resilience is transparent to the caller
public class InventoryClient
{
    private readonly HttpClient _http;
    public InventoryClient(HttpClient http) => _http = http;

    public async Task<InventoryStatus?> GetStatusAsync(
        string sku, CancellationToken ct)
    {
        var response = await _http.GetAsync($"/inventory/{sku}", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InventoryStatus>(ct);
    }
}

#pragma warning restore
