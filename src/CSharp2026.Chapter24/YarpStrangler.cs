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
// Chapter24/YarpStrangler.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter24/YarpStrangler.cs
// YARP (Yet Another Reverse Proxy) — route traffic between old and new
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Custom middleware: route /api/orders to new service, everything else to legacy
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/orders") &&
        FeatureFlags.IsEnabled("NewOrderService"))
    {
        context.Request.Headers["X-Route"] = "new";
    }
    await next(context);
});

app.MapReverseProxy();
app.Run();

#pragma warning restore
