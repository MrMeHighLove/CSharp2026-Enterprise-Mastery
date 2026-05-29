// -----------------------------------------------------------------------
//   Identifiers.cs
//   Strongly-typed IDs — kill the "you passed a CustomerId to a method
//   expecting OrderId" class of bugs at compile time.
// -----------------------------------------------------------------------

namespace CSharp2026.Common.ValueObjects;

public readonly record struct OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString("D");
}

public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString("D");
}

public readonly record struct ProductId(Guid Value)
{
    public static ProductId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString("D");
}

/// <summary>
/// Email address with format validation at construction time.
/// </summary>
public readonly record struct Email
{
    public string Value { get; }

    public Email(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (!IsValid(value))
        {
            throw new ArgumentException($"'{value}' is not a valid email address.", nameof(value));
        }
        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string trimmed = value.Trim();
        int at = trimmed.IndexOf('@', StringComparison.Ordinal);

        if (at <= 0 || at >= trimmed.Length - 1)
        {
            return false;
        }

        string local = trimmed[..at];
        string domain = trimmed[(at + 1)..];

        if (local.Length == 0 || domain.Length == 0)
        {
            return false;
        }

        if (!domain.Contains('.', StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }
}
