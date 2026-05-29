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

namespace CSharp2026.Chapter20;

#pragma warning disable
// Chapter20/Authentication.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// JWT Bearer authentication with proper validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Authority = "https://identity.example.com";
        opts.Audience  = "order-service";
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ClockSkew                = TimeSpan.FromSeconds(30), // tolerance for clock drift
        };
        opts.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                // Enrich the principal with custom claims from a database
                var claims = ctx.Principal?.Claims;
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                // Log authentication failures for security monitoring
                var log = ctx.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                log.LogWarning("JWT authentication failed: {Error}", ctx.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// Resource-based authorisation: can this user access THIS order?
public class OrderAuthorizationHandler
    : AuthorizationHandler<SameCustomerRequirement, Order>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext ctx,
        SameCustomerRequirement requirement,
        Order resource)
    {
        var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (resource.CustomerId.ToString() == userId || ctx.User.IsInRole("Admin"))
            ctx.Succeed(requirement);
        return Task.CompletedTask;
    }
}

#pragma warning restore
