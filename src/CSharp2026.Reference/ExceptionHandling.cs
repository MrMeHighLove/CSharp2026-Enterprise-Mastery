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
// Reference/ExceptionHandling.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/ExceptionHandling.cs
// Define exception hierarchy for your domain
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entity, object id)
        : base($"{entity} with id {id} was not found") { }
}

public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }
    public BusinessRuleViolationException(string rule, string message)
        : base(message) => RuleName = rule;
}

// Global exception handler (catches everything unhandled)
app.UseExceptionHandler(handler =>
{
    handler.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        (int status, string title) = ex switch
        {
            EntityNotFoundException => (404, "Not Found"),
            BusinessRuleViolationException => (422, "Business Rule Violation"),
            ValidationException => (400, "Validation Failed"),
            UnauthorizedAccessException => (403, "Forbidden"),
            _ => (500, "Internal Server Error")
        };

        if (status == 500)
            logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
        else
            logger.LogWarning(ex, "Domain error {Status} on {Path}", status, context.Request.Path);

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = app.Environment.IsDevelopment() ? ex?.Message : null,
            Instance = context.Request.Path
        });
    });
});

#pragma warning restore
