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

namespace CSharp2026.Chapter23;

#pragma warning disable
// Chapter23/FieldKeyword.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter23/FieldKeyword.cs
// GOOD: eliminates boilerplate for simple validated properties
public class UserProfile
{
    public string Email
    {
        get => field;
        set => field = value?.Trim().ToLowerInvariant()
            ?? throw new ArgumentNullException(nameof(value));
    }

    public int Age
    {
        get => field;
        set => field = value is >= 0 and <= 150
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));
    }
}

// CAUTION: don't put complex business logic in property setters
// If validation requires calling a service or raising an event,
// use a factory method or domain method instead of a property setter

#pragma warning restore
