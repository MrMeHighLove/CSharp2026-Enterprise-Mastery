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
// Supplement/ObjectPooling.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/ObjectPooling.cs
using Microsoft.Extensions.ObjectPool;

// 1. Register a pool in DI
builder.Services.AddSingleton<ObjectPool<StringBuilder>>(sp =>
{
    var provider = new DefaultObjectPoolProvider();
    return provider.CreateStringBuilderPool(initialCapacity: 256, maximumRetainedCapacity: 4096);
});

// 2. Use the pool in a service
public class ReportFormatter
{
    private readonly ObjectPool<StringBuilder> _pool;

    public ReportFormatter(ObjectPool<StringBuilder> pool) => _pool = pool;

    public string Format(IEnumerable<ReportLine> lines)
    {
        var sb = _pool.Get();
        try
        {
            foreach (var line in lines)
            {
                sb.Append(line.Date.ToString("yyyy-MM-dd"))
                  .Append('	')
                  .AppendLine(line.Description);
            }
            return sb.ToString();
        }
        finally
        {
            _pool.Return(sb); // Always return — use try/finally
        }
    }
}

// 3. Custom pooled object — implement IResettable for auto-reset
public class ParseContext : IResettable
{
    public List<Token> Tokens { get; } = new(64);
    public Dictionary<string, object> Variables { get; } = new(16);
    public int Position { get; set; }

    public bool TryReset()
    {
        Tokens.Clear();
        Variables.Clear();
        Position = 0;
        return true; // Return false to discard the object instead of pooling it
    }
}

builder.Services.AddSingleton(
    new DefaultObjectPool<ParseContext>(new DefaultPooledObjectPolicy<ParseContext>()));

#pragma warning restore
