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
// Chapter09/ConfigureAwait.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// ConfigureAwait(false) tells the runtime: don't capture the current SynchronizationContext
// This matters in library code and in WinForms/WPF apps.
// In ASP.NET Core there is NO SynchronizationContext — ConfigureAwait(false) is a no-op
// there, but it's still good practice in library code.

// AVOID: Library code that may deadlock if called from a context with SynchronizationContext
public async Task<string> GetDataAsync(string url)
{
    using var client = new HttpClient();
    var response = await client.GetAsync(url);    // captures context
    return await response.Content.ReadAsStringAsync();  // resumes on original thread
}

// GOOD: Library code — always use ConfigureAwait(false) in library projects
public async Task<string> GetDataAsync(string url, CancellationToken ct)
{
    using var client = new HttpClient();
    var response = await client.GetAsync(url, ct).ConfigureAwait(false);
    return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
}

// Rule of thumb:
// - Application code (ASP.NET Core, console apps): omit ConfigureAwait(false)
// - Library code (NuGet packages, shared infrastructure): always use ConfigureAwait(false)

#pragma warning restore
