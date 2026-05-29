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

namespace CSharp2026.Chapter25;

#pragma warning disable
// Chapter25/ExceptionHandling.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter25/ExceptionHandling.cs
// ANTI-PATTERN: swallowing exceptions
public async Task<Order?> GetOrderAsync(int id)
{
    try { return await _repo.GetAsync(id); }
    catch (Exception) { return null; } // The exception is gone. Nobody knows.
}

// ANTI-PATTERN: catching to rethrow without value
try { await DoWorkAsync(); }
catch (Exception ex) { throw ex; } // Destroys the stack trace

// CORRECT: only catch what you can handle; let the rest propagate
public async Task<Order?> GetOrderAsync(int id)
{
    try { return await _repo.GetAsync(id); }
    catch (EntityNotFoundException) { return null; }  // Expected, handle it
    // SqlException, TimeoutException, etc. propagate to the global handler
}

// Global exception handler in ASP.NET Core (one place, full context)
app.UseExceptionHandler(handler =>
{
    handler.Run(async context =>
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        _logger.LogError(ex, "Unhandled exception for request {Path}", context.Request.Path);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "An error occurred" });
    });
});

#pragma warning restore
