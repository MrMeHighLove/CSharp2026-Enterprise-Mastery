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

namespace CSharp2026.Chapter24;

#pragma warning disable
// Chapter24/MigrationChecklist.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter24/MigrationChecklist.cs
// Step 1: Run upgrade-assistant to get the migration report
// dotnet tool install -g upgrade-assistant
// upgrade-assistant analyze MyApp.sln

// Step 2: Address API compatibility issues
// Common .NET Framework APIs removed in modern .NET:

// REMOVED: HttpContext.Current (thread-static, incompatible with async)
// REPLACE: inject IHttpContextAccessor
public class LegacyService
{
    private readonly IHttpContextAccessor _http;
    public LegacyService(IHttpContextAccessor http) => _http = http;

    public string GetCurrentUser() =>
        _http.HttpContext?.User.Identity?.Name ?? "anonymous";
}

// REMOVED: ConfigurationManager.AppSettings
// REPLACE: IConfiguration
public class LegacyConfig
{
    private readonly IConfiguration _config;
    public LegacyConfig(IConfiguration config) => _config = config;

    public string GetSetting(string key) => _config[key] ?? string.Empty;
}

// REMOVED: Thread.Abort
// REPLACE: CancellationToken throughout the call stack

#pragma warning restore
