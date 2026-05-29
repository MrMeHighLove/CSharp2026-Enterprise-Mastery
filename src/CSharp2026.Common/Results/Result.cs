// -----------------------------------------------------------------------
//   Result.cs
//   Functional Result<T> — explicit success/failure without exceptions
//   for control flow. Used across chapters that discuss error handling.
// -----------------------------------------------------------------------

namespace CSharp2026.Common.Results;

/// <summary>
/// Result of an operation that can fail. Exceptions remain reserved for
/// genuinely exceptional infrastructure problems; expected failures
/// (validation, business rules) flow through Result instead.
/// </summary>
public readonly record struct Result<T>
{
    public T?      Value   { get; }
    public string? Error   { get; }
    public bool    IsSuccess { get; }
    public bool    IsFailure => !IsSuccess;

    internal Result(T? value, string? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Error!);

    public Result<TOut> Map<TOut>(Func<T, TOut> map) =>
        IsSuccess ? Result.Success(map(Value!)) : Result.Failure<TOut>(Error!);

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind) =>
        IsSuccess ? bind(Value!) : Result.Failure<TOut>(Error!);
}

public static class Result
{
    public static Result<T> Success<T>(T value) => new(value, null, true);
    public static Result<T> Failure<T>(string error) => new(default, error, false);
}
