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

namespace CSharp2026.Chapter08;

#pragma warning disable
// Chapter08/Span.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: Old approach: Substring allocates on every parse operation
public static (string User, string Domain) ParseEmailOld(string email)
{
    var atIndex = email.IndexOf('@');
    var user    = email.Substring(0, atIndex);       // allocation!
    var domain  = email.Substring(atIndex + 1);      // allocation!
    return (user, domain);
}

// GOOD: Span<T> approach: zero allocation — works with slices of existing memory
public static (ReadOnlySpan<char> User, ReadOnlySpan<char> Domain)
    ParseEmail(ReadOnlySpan<char> email)
{
    var atIndex = email.IndexOf('@');
    if (atIndex < 0) throw new FormatException("Invalid email.");
    return (email[..atIndex], email[(atIndex + 1)..]);
}

// Usage: no allocation at any point
ReadOnlySpan<char> raw = "victor@example.com".AsSpan();
var (user, domain) = ParseEmail(raw);
Console.WriteLine(user.ToString());   // "victor"
Console.WriteLine(domain.ToString()); // "example.com"

// Real-world parsing: extracting fields from a log line
public static void ParseLogLine(ReadOnlySpan<char> line,
    out ReadOnlySpan<char> timestamp, out ReadOnlySpan<char> level,
    out ReadOnlySpan<char> message)
{
    // Format: "2026-01-01T12:00:00Z [INFO] Message content here"
    var firstSpace = line.IndexOf(' ');
    timestamp = line[..firstSpace];
    var rest  = line[(firstSpace + 1)..];
    var closeB = rest.IndexOf(']');
    level   = rest[1..closeB];   // skip '['
    message = rest[(closeB + 2)..];
}

#pragma warning restore
