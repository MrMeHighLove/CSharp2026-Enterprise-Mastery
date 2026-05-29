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
// Chapter21/StreamingExample.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter21/StreamingExample.cs
// Minimal API endpoint streaming AI response
app.MapGet("/chat/stream", async (string question, IChatClient chat,
    CancellationToken ct) =>
{
    return Results.Stream(async stream =>
    {
        var writer = new StreamWriter(stream);
        await foreach (var update in chat.CompleteStreamingAsync(question, null, ct))
        {
            if (update.Text is { Length: > 0 } text)
            {
                await writer.WriteAsync($"data: {text}

");
                await writer.FlushAsync(ct);
            }
        }
    }, "text/event-stream");
});

#pragma warning restore
