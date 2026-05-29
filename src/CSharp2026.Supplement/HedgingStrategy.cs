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
// Supplement/HedgingStrategy.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/HedgingStrategy.cs
// Hedging: if first call doesn't respond in 200ms, fire a second in parallel
var pipeline = new ResiliencePipelineBuilder<string>()
    .AddHedging(new HedgingStrategyOptions<string>
    {
        MaxHedgedAttempts = 2,
        Delay = TimeSpan.FromMilliseconds(200),
        // Only hedge on slow responses, not failures
        ShouldHandle = new PredicateBuilder<string>()
            .HandleResult(_ => false), // Only hedge on delay, not result
        ActionGenerator = args =>
        {
            // Try a different replica for the hedged request
            return () => GetFromReplicaAsync(args.AttemptNumber, args.ActionContext.CancellationToken);
        }
    })
    .Build();

// P99 latency improves dramatically because the second attempt often returns
// before the first slow one finishes. The faster response wins.
// Caution: only use on idempotent read endpoints.

#pragma warning restore
