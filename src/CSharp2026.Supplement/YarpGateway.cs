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
// Supplement/YarpGateway.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/YarpGateway.cs
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilder =>
    {
        // Add correlation ID to all proxied requests
        transformBuilder.AddRequestTransform(async transform =>
        {
            var correlationId = transform.HttpContext.TraceIdentifier;
            transform.ProxyRequest.Headers.TryAddWithoutValidation(
                "X-Correlation-ID", correlationId);
            await Task.CompletedTask;
        });

        // Strip internal headers before forwarding to backends
        transformBuilder.AddRequestHeaderRemove("X-Internal-Secret");
    });

// Custom YARP middleware: JWT -> backend token exchange
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy(pipeline =>
{
    pipeline.Use(async (context, next) =>
    {
        // Exchange user JWT for a service-to-service token
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var token = await TokenExchangeService.GetBackendTokenAsync(context.User);
            context.Request.Headers["X-Service-Token"] = token;
        }
        await next(context);
    });
});

// appsettings.json routes
// "ReverseProxy": {
//   "Routes": {
//     "orders": {
//       "ClusterId": "orders-cluster",
//       "Match": { "Path": "/api/orders/{**catch-all}" }
//     }
//   },
//   "Clusters": {
//     "orders-cluster": {
//       "Destinations": {
//         "orders-primary": { "Address": "http://orders-service:8080/" }
//       }
//     }
//   }
// }

#pragma warning restore
