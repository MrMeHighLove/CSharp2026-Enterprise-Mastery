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

namespace CSharp2026.Chapter17;

#pragma warning disable
// Chapter17/ServiceLifetimes.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: CAPTIVE DEPENDENCY: Singleton capturing a Scoped service
// DbContext is registered as Scoped — but MyCache is Singleton
// Result: DbContext from request #1 is used for all subsequent requests
public class MyCache  // registered as Singleton
{
    private readonly AppDbContext _db;  // registered as Scoped — WRONG!
    public MyCache(AppDbContext db) => _db = db;
}

// GOOD: Correct: Singleton captures IServiceScopeFactory and creates scopes explicitly
public class MyCache : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    public MyCache(IServiceScopeFactory factory) => _scopeFactory = factory;

    public async Task<string?> GetValueAsync(string key, CancellationToken ct)
    {
        // Create a short-lived scope to safely resolve Scoped services
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.ConfigValues
            .Where(c => c.Key == key)
            .Select(c => c.Value)
            .FirstOrDefaultAsync(ct);
    }
    public void Dispose() { }
}

// GOOD: Enable scope validation in development to catch captive dependencies early
builder.Host.UseDefaultServiceProvider(opts =>
{
    opts.ValidateScopes  = true;
    opts.ValidateOnBuild = true;
});

#pragma warning restore
