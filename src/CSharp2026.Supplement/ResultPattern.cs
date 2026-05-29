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
// Supplement/ResultPattern.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ResultPattern.cs
public class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private Result(T value) { Value = value; IsSuccess = true; }
    private Result(string error) { Error = error; IsSuccess = false; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);

    // Functional chaining
    public Result<TNext> Map<TNext>(Func<T, TNext> fn) =>
        IsSuccess ? Result<TNext>.Success(fn(Value!)) : Result<TNext>.Failure(Error!);

    public async Task<Result<TNext>> MapAsync<TNext>(Func<T, Task<TNext>> fn) =>
        IsSuccess ? Result<TNext>.Success(await fn(Value!)) : Result<TNext>.Failure(Error!);

    public Result<T> OnFailure(Action<string> action)
    {
        if (IsFailure) action(Error!);
        return this;
    }
}

// Usage: readable, no exceptions for flow control
public async Task<IActionResult> PlaceOrder(PlaceOrderRequest request)
{
    var emailResult = Email.Create(request.CustomerEmail);
    if (emailResult.IsFailure) return BadRequest(emailResult.Error);

    var commandResult = await _mediator.Send(new PlaceOrderCommand(emailResult.Value!));

    return commandResult.IsSuccess
        ? CreatedAtAction(nameof(GetOrder), new { id = commandResult.Value }, null)
        : commandResult.Error!.Contains("inventory")
            ? Conflict(commandResult.Error)
            : BadRequest(commandResult.Error);
}

#pragma warning restore
