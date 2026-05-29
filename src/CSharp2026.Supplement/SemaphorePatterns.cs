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
// Supplement/SemaphorePatterns.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/SemaphorePatterns.cs
// SemaphoreSlim: limit concurrent external API calls
public class ThirdPartyApiClient
{
    private readonly SemaphoreSlim _throttle = new(20, 20); // max 20 concurrent
    private readonly HttpClient _http;

    public async Task<T> GetAsync<T>(string path, CancellationToken ct = default)
    {
        await _throttle.WaitAsync(ct);
        try
        {
            var response = await _http.GetAsync(path, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(ct) ?? default!;
        }
        finally
        {
            _throttle.Release();
        }
    }

    // Batch with bounded concurrency
    public async Task<TResult[]> BatchAsync<TItem, TResult>(
        IEnumerable<TItem> items,
        Func<TItem, Task<TResult>> process,
        int concurrency = 10)
    {
        var semaphore = new SemaphoreSlim(concurrency, concurrency);
        var tasks = items.Select(async item =>
        {
            await semaphore.WaitAsync();
            try { return await process(item); }
            finally { semaphore.Release(); }
        });
        return await Task.WhenAll(tasks);
    }
}

#pragma warning restore
