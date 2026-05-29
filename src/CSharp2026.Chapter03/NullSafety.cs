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

namespace CSharp2026.Chapter03;

#pragma warning disable
// Chapter03/NullSafety.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// With nullable reference types enabled (<Nullable>enable</Nullable>)

// The compiler warns: 'Name' may be null, unboxing or calling a member may fail
public string GetDisplayName(User? user)
    => user.Name.ToUpper(); // WARNING CS8602: Dereference of a possibly null reference

// GOOD: Explicit null-checking makes intent clear
public string GetDisplayName(User? user)
{
    if (user is null) return "Anonymous";
    return user.Name.ToUpper();
}

// GOOD: Or use the null-conditional operator cleanly
public string GetDisplayName(User? user)
    => user?.Name?.ToUpper() ?? "Anonymous";

// GOOD: Eliminating null from domain models with required + init
public class CustomerProfile
{
    public required string FullName   { get; init; }
    public required string Email      { get; init; }
    public          string? Phone     { get; init; }  // Genuinely optional
}

// Compiler enforces FullName and Email at construction site:
var profile = new CustomerProfile
{
    FullName = "Victor Mihailov",
    Email    = "victor@example.com"
    // Phone is optional — no compiler warning
};

#pragma warning restore
