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
// Supplement/HealthChecks.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/HealthChecks.cs
builder.Services.AddHealthChecks()
    // Database: verifies the connection and can run a test query
    .AddNpgsql(
        builder.Configuration.GetConnectionString("Postgres")!,
        name: "postgres",
        tags: ["ready", "database"])

    // Redis: verifies connectivity
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        tags: ["ready", "cache"])

    // Custom health check: verifies business-critical data is available
    .AddCheck<OrderQueueHealthCheck>("order-queue",
        failureStatus: HealthStatus.Degraded,
        tags: ["ready"])

    // External dependency: HTTP-based check
    .AddUrlGroup(
        new Uri(builder.Configuration["PaymentService:HealthUrl"]!),
        name: "payment-service",
        failureStatus: HealthStatus.Degraded,
        tags: ["ready"]);

// Custom health check implementation
public class OrderQueueHealthCheck : IHealthCheck
{
    private readonly IOrderQueue _queue;

    public OrderQueueHealthCheck(IOrderQueue queue) => _queue = queue;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct)
    {
        try
        {
            var depth = await _queue.GetDepthAsync(ct);
            var data = new Dictionary<string, object> { ["queue_depth"] = depth };

            return depth switch
            {
                < 1000 => HealthCheckResult.Healthy("Queue is healthy", data),
                < 5000 => HealthCheckResult.Degraded("Queue is building up", data),
                _ => HealthCheckResult.Unhealthy("Queue is overloaded", data: data)
            };
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Queue check failed", ex);
        }
    }
}

#pragma warning restore
