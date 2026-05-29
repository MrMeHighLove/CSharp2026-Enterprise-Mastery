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

namespace CSharp2026.Chapter02;

#pragma warning disable
// Chapter02/PartialProperties.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Defining file (user code)
public partial class UserProfile
{
    // Declaration — just the signature
    public partial string DisplayName { get; set; }
}

// Generated file (source generator output)
public partial class UserProfile
{
    private string _displayName = string.Empty;

    public partial string DisplayName
    {
        get => _displayName;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _displayName = value.Trim();
        }
    }
}

#pragma warning restore
