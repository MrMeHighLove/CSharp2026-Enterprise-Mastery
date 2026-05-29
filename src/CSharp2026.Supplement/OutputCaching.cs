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
// Supplement/OutputCaching.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/OutputCaching.cs
// Output caching: cache the entire HTTP response at the framework level
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(10)));

    options.AddPolicy("products", b => b
        .Expire(TimeSpan.FromMinutes(5))
        .SetVaryByQuery("category", "page")
        .Tag("products"));

    options.AddPolicy("user-specific", b => b
        .Expire(TimeSpan.FromSeconds(30))
        .SetVaryByHeader("Authorization")
        .SetVaryByClaim(ClaimTypes.NameIdentifier));
});

// Apply to endpoints
app.MapGet("/products", GetProducts)
    .CacheOutput("products");

app.MapGet("/me/orders", GetMyOrders)
    .CacheOutput("user-specific");

// Invalidate by tag after data changes
app.MapPost("/products", async (ProductDto dto, IOutputCacheStore cache, CancellationToken ct) =>
{
    await SaveProductAsync(dto);
    await cache.EvictByTagAsync("products", ct); // All product cache entries cleared
    return Results.Created();
});

// Vary by custom key — e.g. tenant in multi-tenant apps
public class TenantCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext ctx, CancellationToken ct)
    {
        var tenant = ctx.HttpContext.User.FindFirstValue("tenant_id") ?? "default";
        ctx.CacheVaryByValues.Add("tenant", tenant);
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext ctx, CancellationToken ct)
        => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext ctx, CancellationToken ct)
        => ValueTask.CompletedTask;
}

#pragma warning restore
