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

namespace CSharp2026.Reference;

#pragma warning disable
// Reference/DynamicPgo.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/DynamicPgo.cs
// Dynamic PGO is enabled by default in .NET 8+
// To observe PGO effects in your application:

// 1. Check if tiered compilation is active (it enables PGO)
// Set DOTNET_TC_QuickJitForLoops=0 to disable for comparison benchmarks

// 2. Profile before and after warmup — PGO improves after the JIT observes patterns
// A benchmark run needs adequate warmup iterations to see PGO benefits:
[GlobalSetup]
public async Task Setup()
{
    // Warm up the JIT — first 100 iterations compile to Tier 0
    // Iterations 101+ use Tier 1 (PGO-optimised) compilation
    for (int i = 0; i < 200; i++)
        await ProcessOrderAsync(GetSampleOrder());
}

// 3. NativeAOT for startup-critical workloads (Lambda, CLI tools)
// Publish with: dotnet publish -r linux-x64 -p:PublishAot=true
// Tradeoffs: faster cold start, no dynamic code generation, larger binary

// 4. ReadyToRun (R2R): pre-compiled managed code for faster startup
// Publish with: dotnet publish -p:PublishReadyToRun=true
// Better startup than JIT, worse peak throughput than PGO after warmup

#pragma warning restore
