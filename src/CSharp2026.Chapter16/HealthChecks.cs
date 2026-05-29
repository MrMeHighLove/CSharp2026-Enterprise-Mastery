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
// Chapter16/HealthChecks.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Health checks: essential for Kubernetes liveness/readiness probes
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "sql-server", tags: ["ready"])
    .AddRedis(redisConnectionString, name: "redis", tags: ["ready"])
    .AddRabbitMQ(rabbitConnectionString, name: "rabbitmq", tags: ["ready"])
    .AddCheck<CustomBusinessHealthCheck>("business-rules", tags: ["ready"]);

// Separate liveness (is the process alive?) from readiness (can it serve traffic?)
app.MapHealthChecks("/health/live",  new HealthCheckOptions
{
    Predicate = _ => false,  // liveness: just return 200 — process is alive
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("ready"),  // readiness: check dependencies
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

// Custom health check: business-level diagnostic
public class CustomBusinessHealthCheck : IHealthCheck
{
    private readonly IOrderReader _orders;
    public CustomBusinessHealthCheck(IOrderReader orders) => _orders = orders;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext ctx, CancellationToken ct)
    {
        try
        {
            var count = await _orders.GetPendingCountAsync(ct);
            if (count > 10_000)
                return HealthCheckResult.Degraded(
                    $"High pending order backlog: {count}");
            return HealthCheckResult.Healthy($"Pending orders: {count}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cannot reach order store", ex);
        }
    }
}

#pragma warning restore
