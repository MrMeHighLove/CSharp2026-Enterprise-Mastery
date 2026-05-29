# Known Compile Gaps Per Chapter

This document is the honest catalog of what each chapter project needs in
order to build cleanly. Two chapters are intentionally complete:

* **`CSharp2026.Common`** — production-grade shared types, fully tested.
* **`CSharp2026.Chapter05`** — fully compiling exemplar with comprehensive
  tests. Use it as the template when bringing other chapters to green.

The remaining chapter projects are configured permissively (`Nullable=annotations`,
relaxed `NoWarn`, `TreatWarningsAsErrors=false`) and their manuscript `.cs`
files are preserved as project content rather than compiled source. This is
intentional: many files are excerpts with ellipses, standalone statements,
or future-language examples. The project still emits a lightweight container
assembly so the solution can restore, build, and test cleanly while each
chapter is brought to green deliberately.

The smoke tests are intentionally lightweight for these snippet containers;
the source projects themselves are built by the solution/source build pass.
Replace each smoke test with real coverage and restore the project reference
as you complete the chapter.

## Per-chapter status

### Chapter 02 — Language features (C# 13 & 14)

* **What is here:** snippets for `params Collections`, `System.Threading.Lock`,
  partial properties, ref locals in async, `allows ref struct`, primary
  constructors, collection expressions, extension members.
* **What it needs to fully build:**
  * Several `Listing/` files are partial fragments (a method body without a
    containing class). Wrap each in a `public static class Examples` shell.
  * The `RefStruct` example assumes a `BufferReader` helper — supply a
    minimal struct.

### Chapter 03 — Clean code

* **What is here:** naming, function size, comment quality, null safety,
  pattern matching examples.
* **What it needs:**
  * The `NullSafety` snippet references a `Customer.FindBy(...)` method —
    bring it in from `CSharp2026.Common.Domain.Customer` or stub.

### Chapter 04 — SOLID

* **What is here:** SRP/OCP/LSP/ISP/DIP one-pagers each as a separate file.
* **What it needs:**
  * The DIP example references an `ILogger` and an `IEmailSender`. Wire
    `Microsoft.Extensions.Logging.Abstractions` (already in CPM) and add
    an `IEmailSender` interface to a `Services/` folder.

### Chapter 05 — Design patterns *(COMPLETE)*

Used as the canonical exemplar. All four patterns (Strategy, Builder,
Decorator, Mediator) are fully implemented, DI-wired, and tested.

### Chapter 06 — Domain-driven design

* **What is here:** Entity, Value Object, Aggregate, Repository, Domain Event.
* **What it needs:**
  * `Order`, `OrderLine`, `OrderId`, `CustomerId`, `Money`, `IDomainEvent`,
    `DomainException` are already in `CSharp2026.Common`. Replace the
    book's local copies with `using CSharp2026.Common.Domain;` etc.
  * `IOrderRepository` + `OrderRepository` reference EF Core's `DbContext`.
    Provide an in-memory EF Core context or a fake repository for tests
    (the CPM file has `Microsoft.EntityFrameworkCore.InMemory` for this).

### Chapter 07 — Performance fundamentals

* **What is here:** `Span<T>`, pooling, `ArrayPool`, `ValueTask`, `string.Create`.
* **What it needs:**
  * BenchmarkDotNet (in CPM) needs a `[MemoryDiagnoser]`-decorated benchmark
    class — wrap each `// AVOID: / // GOOD:` pair as a `[Benchmark]` method.

### Chapter 08 — Async I/O and pipelines

* **What is here:** channels, `System.IO.Pipelines`, async streams.
* **What it needs:**
  * Several snippets read from a `Stream` parameter. Provide a
    `MemoryStream`-based test double.

### Chapter 09 — Concurrency

* **What is here:** `System.Threading.Lock`, lock-free patterns, immutable
  data structures, channels.
* **What it needs:**
  * Mostly self-contained. Verify the `Lock` snippet builds on .NET 10 —
    `System.Threading.Lock` is C# 13 / .NET 9+.

### Chapter 10 — Memory and GC

* **What is here:** generation behaviour, LOH, pinning, `Span<T>` reuse.
* **What it needs:**
  * The `PinnedObjectHeap` snippet uses `GC.AllocateUninitializedArray<T>(..., pinned: true)`
    — verify the API and add a sanity test.

### Chapter 11 — LINQ deep dive

* **What is here:** `CountBy`, `AggregateBy`, `Index`, compiled queries,
  the `AsEnumerable` anti-pattern, no-tracking queries.
* **What it needs:**
  * EF Core `DbContext` — either depend on the InMemory provider or extract
    pure-LINQ examples into a separate file that doesn't need EF.

### Chapter 12 — Web APIs (Minimal + MVC)

* **What is here:** Minimal API endpoints, MVC controllers, model binding,
  filters, rate limiting.
* **What it needs:**
  * Switch SDK to `Microsoft.NET.Sdk.Web` (currently `Microsoft.NET.Sdk`).
  * Add `Microsoft.AspNetCore.OpenApi` package reference (already in CPM).

### Chapter 13 — Real-time / SignalR / gRPC

* **What is here:** SignalR hub, gRPC service definition snippet.
* **What it needs:**
  * `Grpc.AspNetCore` (add to CPM), `Microsoft.AspNetCore.SignalR`.
  * `.proto` file for the gRPC sample.

### Chapter 14 — Caching strategies

* **What is here:** L1/L2 hybrid cache, stampede protection, invalidation,
  outbox pattern, distributed cache key design.
* **What it needs:**
  * `Microsoft.Extensions.Caching.Hybrid` + `StackExchange.Redis` (both in CPM).
  * The outbox-pattern example references EF Core — same as Chapter 6.

### Chapter 15 — Microservices architecture

* **What is here:** boundary discussion + service-to-service communication code.
* **What it needs:** illustrative only — most examples are diagrams; the
  snippets that are real code reference Polly resilience pipelines (CPM).

### Chapter 16 — Resilience (Polly)

* **What is here:** retry, circuit breaker, timeout, hedging, fallback
  composed through `ResiliencePipelineBuilder`.
* **What it needs:** Polly is already in CPM. Add the package reference
  and the snippets should build with light wrapping.

### Chapter 17 — Distributed messaging

* **What is here:** outbox dispatcher, RabbitMQ consumer, idempotency key.
* **What it needs:** `RabbitMQ.Client` package reference; an
  `IConnectionFactory` test double.

### Chapter 18 — Observability

* **What is here:** OpenTelemetry tracing, structured logging with Serilog,
  metrics.
* **What it needs:** `OpenTelemetry.Extensions.Hosting`,
  `OpenTelemetry.Exporter.Otlp`, `Serilog.Sinks.OpenTelemetry` —
  add these to `Directory.Packages.props`.

### Chapter 19 — Security

* **What is here:** authentication, authorisation, JWT, data protection.
* **What it needs:** `Microsoft.AspNetCore.Authentication.JwtBearer`,
  `Microsoft.AspNetCore.DataProtection`.

### Chapter 20 — Secrets management

* **What is here:** User Secrets, Azure Key Vault, environment variables,
  data protection. The `prod-db` connection string example was renamed
  to `proddb01` to avoid hyphen rendering issues in print.
* **What it needs:** `Azure.Identity`, `Azure.Security.KeyVault.Secrets`.

### Chapter 21 — AI integration

* **What is here:** Semantic Kernel kernel setup, RAG pipeline,
  `Microsoft.Extensions.AI` chat client.
* **What it needs:** `Microsoft.SemanticKernel`,
  `Microsoft.Extensions.AI` package references.

### Chapter 22 — Testing strategy

* **What is here:** test pyramid examples, FluentAssertions usage,
  property-based testing intro.
* **What it needs:** `FsCheck.Xunit` (add to CPM).

### Chapter 23 — Migration patterns

* **What is here:** strangler fig, expand-and-contract schema migrations,
  branch-by-abstraction.
* **What it needs:** illustrative only — wrap snippets in static classes
  so they sit in the namespace.

### Chapter 24 — Documenting decisions

* **What is here:** ADR template excerpts, XML doc comments, OpenAPI.
* **What it needs:** nothing — pure illustrative content.

### Chapter 25 — Career and craft

* **What is here:** code review checklist, technical debt taxonomy.
* **What it needs:** nothing — pure illustrative content.

### Supplement

* 43 deep-dive snippets distributed across performance, concurrency,
  architecture, testing, security, observability, design patterns II/III,
  Minimal APIs, EF Core, caching, gRPC/SignalR, advanced DI, cloud
  patterns, microservices, LINQ, resilience, Span/Memory, and domain modelling.
* These are organised by topic, not by chapter. The intent is for the
  reader to dip into individual files; a single project compiling
  everything together is not required.

### Benchmarks

* **What is here:** `benchmarks/CSharp2026.Benchmarks` contains runnable
  BenchmarkDotNet suites for string processing and LINQ-vs-loop positive
  sums. `tests/CSharp2026.Benchmarks.Tests` verifies equivalent output so
  performance examples do not drift semantically.
* **How to run:** `dotnet run --configuration Release --project benchmarks/CSharp2026.Benchmarks -- --filter *StringProcessing*`

### AppendixB — Performance reference

* `MemoryReference.cs`, `CollectionReference.cs` — code-comment-only
  illustration tables. They compile as long as the wrapping namespace is
  defined. No production logic.

### Reference

* Cross-cutting reference snippets (cancellation tokens, async patterns,
  exception handling). Self-contained; many will build with the auto-wrap
  alone.

## How to drive a chapter to green

1. Open `src/CSharp2026.ChapterNN/CSharp2026.ChapterNN.csproj`.
2. Delete the `<Nullable>annotations</Nullable>` and `<NoWarn>...</NoWarn>`
   lines. `Directory.Build.props` then takes over with strict settings.
3. `dotnet build src/CSharp2026.ChapterNN` and address the errors in order.
   Most will be missing types — bring them in from `CSharp2026.Common` or
   add stubs locally.
4. When the chapter builds cleanly, replace `AssemblyLoadsTests.cs` in the
   matching test project with the actual coverage the chapter deserves.

A chapter is "done" when:
* It builds under `/warnaserror`.
* Its test project has at least one meaningful test per named pattern.
* The book's text references corresponding source files that exist in
  this repo, so a reader can `git clone` and follow along.
