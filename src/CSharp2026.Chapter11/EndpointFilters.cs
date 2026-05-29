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

namespace CSharp2026.Chapter11;

#pragma warning disable
// Chapter11/EndpointFilters.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Endpoint filter: validation using FluentValidation or DataAnnotations
public class ValidationFilter<TRequest> : IEndpointFilter
{
    private readonly IValidator<TRequest> _validator;
    public ValidationFilter(IValidator<TRequest> validator)
        => _validator = validator;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next)
    {
        if (ctx.Arguments.OfType<TRequest>().FirstOrDefault() is not { } request)
            return await next(ctx);

        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
            return Results.ValidationProblem(result.ToDictionary());

        return await next(ctx);
    }
}

// Timing filter: log endpoint duration
public class TimingFilter : IEndpointFilter
{
    private readonly ILogger<TimingFilter> _log;
    public TimingFilter(ILogger<TimingFilter> log) => _log = log;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await next(ctx);
        }
        finally
        {
            _log.LogInformation("Endpoint {Name} completed in {ElapsedMs}ms",
                ctx.HttpContext.GetEndpoint()?.DisplayName,
                sw.ElapsedMilliseconds);
        }
    }
}

#pragma warning restore
