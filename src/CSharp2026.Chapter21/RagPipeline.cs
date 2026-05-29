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
// Chapter21/RagPipeline.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter21/RagPipeline.cs
public class RagSearchService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddings;
    private readonly IVectorStore _store;
    private readonly IChatClient _chat;

    public RagSearchService(
        IEmbeddingGenerator<string, Embedding<float>> embeddings,
        IVectorStore store,
        IChatClient chat)
    {
        _embeddings = embeddings;
        _store = store;
        _chat = chat;
    }

    public async Task<string> AnswerAsync(string question)
    {
        // 1. Embed the question
        var questionEmbedding = await _embeddings.GenerateEmbeddingVectorAsync(question);

        // 2. Retrieve top 5 relevant chunks
        var collection = _store.GetCollection<string, KnowledgeChunk>("docs");
        var results = await collection.VectorizedSearchAsync(questionEmbedding,
            new VectorSearchOptions { Top = 5 });

        // 3. Build context
        var context = new StringBuilder();
        await foreach (var result in results.Results)
            context.AppendLine(result.Record.Content);

        // 4. Prompt with grounding
        var prompt = "Answer the question using ONLY the context below. " +
                     "If the answer is not in the context, say I do not know. " +
                     "Context: " + context + " Question: " + question;

        var response = await _chat.CompleteAsync(prompt);
        return response.Message.Text ?? string.Empty;
    }
}

#pragma warning restore
