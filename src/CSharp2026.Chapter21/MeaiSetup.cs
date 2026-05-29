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
// Chapter21/MeaiSetup.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter21/MeaiSetup.cs
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register Azure OpenAI chat client
builder.Services.AddChatClient(services =>
    new AzureOpenAIClient(
        new Uri(builder.Configuration["AzureOpenAI:Endpoint"]!),
        new AzureKeyCredential(builder.Configuration["AzureOpenAI:Key"]!))
    .AsChatClient("gpt-4o"));

// Swap to local Ollama with one line change
// builder.Services.AddChatClient(new OllamaChatClient("http://localhost:11434", "llama3.2"));

// Pipeline: logging -> caching -> rate-limiting -> underlying client
builder.Services.AddChatClient(services =>
    services.GetRequiredService<IUnderlyingChatClient>()
        .AsBuilder()
        .UseLogging()
        .UseDistributedCache()
        .UseRateLimiting()
        .Build());

#pragma warning restore
