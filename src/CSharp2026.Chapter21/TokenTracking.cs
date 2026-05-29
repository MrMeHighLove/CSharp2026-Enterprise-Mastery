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

namespace CSharp2026.Chapter21;

#pragma warning disable
// Chapter21/TokenTracking.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter21/TokenTracking.cs
public class TokenTrackingMiddleware : DelegatingChatClient
{
    private readonly ITokenUsageStore _store;

    public TokenTrackingMiddleware(IChatClient inner, ITokenUsageStore store)
        : base(inner) => _store = store;

    public override async Task<ChatCompletion> CompleteAsync(
        IList<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken ct = default)
    {
        var result = await base.CompleteAsync(messages, options, ct);

        if (result.Usage is { } usage)
        {
            await _store.RecordAsync(new TokenRecord
            {
                Timestamp = DateTimeOffset.UtcNow,
                InputTokens = usage.InputTokenCount ?? 0,
                OutputTokens = usage.OutputTokenCount ?? 0,
                ModelId = result.ModelId ?? "unknown"
            });
        }
        return result;
    }
}

#pragma warning restore
