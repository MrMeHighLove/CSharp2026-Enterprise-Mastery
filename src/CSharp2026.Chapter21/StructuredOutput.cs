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
// Chapter21/StructuredOutput.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter21/StructuredOutput.cs
public record ProductAnalysis(
    string Sentiment,
    int Score,
    List<string> Keywords,
    string Summary);

public class ReviewAnalyzer
{
    private readonly IChatClient _chat;

    public ReviewAnalyzer(IChatClient chat) => _chat = chat;

    public async Task<ProductAnalysis> AnalyzeAsync(string reviewText)
    {
        // OpenAI structured output — guaranteed to match schema
        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(
                JsonSerializerOptions.Default.GetJsonSchemaAsNode(
                    typeof(ProductAnalysis)))
        };

        var response = await _chat.CompleteAsync(
            $"Analyze this product review: {reviewText}", options);

        return JsonSerializer.Deserialize<ProductAnalysis>(
            response.Message.Text!)!;
    }
}

#pragma warning restore
