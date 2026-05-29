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

namespace CSharp2026.Chapter09;

#pragma warning disable
// Chapter09/CancellationToken.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// GOOD: CancellationToken threaded through every async call
public async Task<IReadOnlyList<Product>> GetRecommendationsAsync(
    CustomerId customerId,
    CancellationToken ct)   // <-- always the last parameter, by convention
{
    ct.ThrowIfCancellationRequested();

    var profile = await _profileService.GetAsync(customerId, ct);
    if (profile is null) return [];

    var candidates = await _catalogue.SearchAsync(
        new CatalogueQuery(profile.PreferredCategories), ct);

    var scores = await _scoringService.ScoreAsync(candidates, profile, ct);

    return scores
        .OrderByDescending(s => s.Score)
        .Take(10)
        .Select(s => s.Product)
        .ToList();
}

// Composing tokens: request token + operation timeout
public async Task<string> CallExternalApiAsync(string url, CancellationToken requestCt)
{
    // Combine: cancel if request cancelled OR if call exceeds 5 seconds
    using var cts = CancellationTokenSource.CreateLinkedTokenSource(requestCt);
    cts.CancelAfter(TimeSpan.FromSeconds(5));

    return await _httpClient.GetStringAsync(url, cts.Token);
}

// Graceful shutdown: ASP.NET Core provides the token via IHostApplicationLifetime
app.MapGet("/long-work", async (IHostApplicationLifetime lifetime, CancellationToken ct) =>
{
    using var cts = CancellationTokenSource.CreateLinkedTokenSource(
        ct, lifetime.ApplicationStopping);

    await DoLongWorkAsync(cts.Token);
    return Results.Ok();
});

#pragma warning restore
