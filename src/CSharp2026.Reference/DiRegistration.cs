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

namespace CSharp2026.Reference;

#pragma warning disable
// Reference/DiRegistration.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/DiRegistration.cs
// Lifetime reference:
// Transient: new instance every time — for stateless services
// Scoped:    new instance per HTTP request — for most services, DbContext
// Singleton: one instance for app lifetime — for caches, config, IHttpClientFactory

// Correct EF Core registration (Scoped — one context per request)
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(connectionString)
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution));

// HttpClient — always use IHttpClientFactory, never new HttpClient()
builder.Services.AddHttpClient<OrderServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Orders"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddStandardResilienceHandler(); // Polly retry + circuit breaker

// Assembly scanning with Scrutor
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(c => c.AssignableTo<IValidator>()).AsImplementedInterfaces().WithTransientLifetime()
    .AddClasses(c => c.AssignableTo<ICommandHandler>()).AsImplementedInterfaces().WithScopedLifetime()
    .AddClasses(c => c.AssignableTo<IQueryHandler>()).AsImplementedInterfaces().WithScopedLifetime());

// Options pattern — strongly typed config
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

#pragma warning restore
