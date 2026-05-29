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
// Reference/MiddlewarePipeline.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/MiddlewarePipeline.cs
// Correct middleware order for enterprise ASP.NET Core apps
var app = builder.Build();

// 1. Exception handling — must be first to catch all errors
app.UseExceptionHandler("/error");
// In development: app.UseDeveloperExceptionPage();

// 2. HSTS (only in production HTTPS)
if (!app.Environment.IsDevelopment())
    app.UseHsts();

// 3. HTTPS redirection
app.UseHttpsRedirection();

// 4. Static files (before routing — short-circuits the pipeline)
app.UseStaticFiles();

// 5. Routing
app.UseRouting();

// 6. Rate limiting
app.UseRateLimiter();

// 7. CORS (after routing so CORS policy can match routes)
app.UseCors();

// 8. Authentication
app.UseAuthentication();

// 9. Authorization (after authentication — needs identity context)
app.UseAuthorization();

// 10. Output caching (after auth — cache per-user responses correctly)
app.UseOutputCache();

// 11. Custom business middleware
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<AuditLoggingMiddleware>();

// 12. Endpoint mapping — last
app.MapControllers();
app.MapEndpointModules();
app.MapHealthChecks("/health");

#pragma warning restore
