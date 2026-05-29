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
// Chapter02/ExtensionMembers.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// C# 14: Extension members
// Extends a type without inheriting from it

extension(string s) StringExtensions
{
    // Extension property (C# 14)
    public bool IsValidEmail => s.Contains('@') && s.Contains('.');

    // Extension indexer (C# 14)
    public char this[Index index] => s[index];

    // Classic extension method — still works
    public string Truncate(int maxLength) =>
        s.Length <= maxLength ? s : s[..maxLength] + "...";
}

// Usage
string email = "victor@example.com";
Console.WriteLine(email.IsValidEmail);   // true — extension property
Console.WriteLine(email.Truncate(10));   // "victor@ex..." — extension method

#pragma warning restore
