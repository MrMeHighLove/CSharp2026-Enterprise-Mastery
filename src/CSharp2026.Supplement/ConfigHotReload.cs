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
// Supplement/ConfigHotReload.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ConfigHotReload.cs
// IOptionsMonitor: responds to configuration changes at runtime
public class FeatureFlagService
{
    private readonly IOptionsMonitor<FeatureFlags> _flags;
    private readonly ILogger<FeatureFlagService> _logger;

    public FeatureFlagService(IOptionsMonitor<FeatureFlags> flags,
        ILogger<FeatureFlagService> logger)
    {
        _flags = flags;
        // Subscribe to changes — executed when config file is updated
        _flags.OnChange(newFlags =>
        {
            _logger.LogInformation("Feature flags updated: {Flags}",
                JsonSerializer.Serialize(newFlags));
        });
    }

    public bool IsEnabled(string featureName)
        => _flags.CurrentValue.IsEnabled(featureName);
}

// In Kubernetes: ConfigMap mounted as a file + IConfiguration file watcher
// When you apply: kubectl apply -f configmap.yaml
// The file changes, IConfiguration detects it, IOptionsMonitor fires

// appsettings configuration provider (automatic in ASP.NET Core):
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(
    $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

// For Azure App Configuration with real-time push:
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["AzureAppConfig:ConnectionString"])
        .UseFeatureFlags(ff => ff.CacheExpirationInterval = TimeSpan.FromSeconds(30))
        .ConfigureRefresh(refresh =>
        {
            refresh.Register("Sentinel", refreshAll: true)
                   .SetCacheExpiration(TimeSpan.FromSeconds(30));
        });
});
builder.Services.AddAzureAppConfiguration();
app.UseAzureAppConfiguration(); // Registers middleware to poll for changes

#pragma warning restore
