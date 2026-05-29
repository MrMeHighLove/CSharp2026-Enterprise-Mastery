# Architecture Notes

The design choices in this repository — recorded so they can be debated,
revised, and (most importantly) understood by anyone joining later.

## Why one project per chapter

Three reasons.

**Tractable build failures.** A single mega-project would let one
broken snippet block compilation of everything else. With one project per
chapter, the reader sees "Chapter 14 has gaps" rather than "the solution
won't build."

**Independent dependencies.** Chapter 21 (AI) needs Semantic Kernel.
Chapter 17 (messaging) needs RabbitMQ.Client. Chapter 13 needs ASP.NET
Core and gRPC. Putting these into separate projects keeps the dependency
graph honest — `dotnet list package` on any single project shows exactly
what that chapter actually uses.

**Reader's mental model.** The book is structured by chapter; the code
should mirror that. A reader who wants only the design patterns code
opens `CSharp2026.Chapter05` and doesn't have to grep across a flat tree.

## Why `CSharp2026.Common` exists

Several types appear across many chapters: `Money`, `OrderId`, `Order`,
`IDomainEvent`, `Result<T>`. The book defines them once in Chapter 6
(DDD) and uses them throughout. If we copied them into every chapter
project, edits would drift; if we left them in Chapter 6 only, chapters
that come earlier in the book could not reference them without an
upward dependency.

Extracting them into a shared library matches what a real team does and
makes the dependency graph one-way: every chapter project can depend on
`Common`, never on another chapter.

## Why Central Package Management

A solution with 27 chapter projects can easily develop version skew on
shared packages — Chapter 14 referencing `Microsoft.Extensions.Caching.Memory`
9.0.0 while Chapter 20 references 9.0.1. CPM (`Directory.Packages.props`)
makes the version a single source of truth and bans the `Version` attribute
on individual `<PackageReference>` items. Upgrades happen in one PR.

## Why analyzers are strict in `Common` and `Chapter05`, lenient elsewhere

The exemplar chapter is held to the standard the whole repo should reach.
The rest are loose so the book's illustrative snippets, which often refer
to types the reader is expected to imagine, can sit in the namespace
without an avalanche of compile errors blocking other progress.

When a chapter is brought to green, the developer removes the lenient
`<NoWarn>` and `<Nullable>annotations</Nullable>` overrides from its
csproj — `Directory.Build.props` then enforces the strict defaults
automatically. That single deletion is the chapter's graduation ceremony.

## Why deterministic and CI-aware builds

`Deterministic=true` plus the conditional `ContinuousIntegrationBuild=true`
make binary output reproducible. Two clean builds of the same source on
the same SDK produce byte-identical assemblies. This matters for
attestation, for diff-based regression detection, and for SBOM tooling
that hashes outputs.

## Why xUnit + FluentAssertions, not MSTest, not NUnit

xUnit is the de facto standard in modern .NET OSS; the test fixtures map
naturally onto records and the test pattern (`[Fact]` / `[Theory]`) is
minimal. FluentAssertions reads like an English specification at the
assertion site, which makes test failures self-documenting:

```csharp
result.Should().BeOfType<OrderSubmittedEvent>()
      .Which.Total.Should().Be(new Money(99m, "USD"));
```

## Why `IPaymentProcessorFactory` instead of `IServiceProvider`

This is the design decision that gives the chapter its title.

A class that takes `IServiceProvider` has its real dependencies invisible
at the call site — the constructor parameter says "anything", and the
unit test has to spin up a container. The keyed-DI feature of
`Microsoft.Extensions.DependencyInjection` 8+ tempts people to inject
`IServiceProvider` for "just one little keyed lookup," and then the
pattern spreads.

Wrapping the keyed lookup in a one-method `IPaymentProcessorFactory`:

* makes the dependency visible (a payment orchestrator clearly needs
  payment processors);
* makes unit tests trivial (`new PaymentOrchestrator(fakeFactory)`);
* contains the single intentional use of `IServiceProvider` inside the
  factory implementation, where it belongs.

The implementation cost is one interface and one class. The architectural
benefit is permanent.

## Why `Result<T>` instead of exceptions for expected failures

Exceptions express a control-flow that says "this can't happen during
normal operation". A validation rejecting "abc" as a non-number is normal
operation. Reaching for `throw` there reframes a routine failure as an
emergency, costs the runtime an exception object and a stack walk, and
hides the failure mode at the call site (`int.Parse(input)` looks
infallible).

`Result<int> Parse(string)` makes the failure mode visible in the type,
costs nothing at runtime when there is no failure, and composes cleanly
through `Bind` and `Map`.

We do not use it everywhere — infrastructure failures (database down,
disk full, malformed wire payloads) remain exceptions. The line lives at
"is this a normal outcome of valid input?"

## Why each entity has its own readonly record struct id

```csharp
public readonly record struct OrderId(Guid Value);
public readonly record struct CustomerId(Guid Value);
```

Three properties at once:

* **Type safety.** `void Cancel(OrderId id)` cannot accidentally receive
  a `CustomerId`. The compiler catches the swap that would have been a
  hard-to-find production bug.
* **Value semantics.** Equality and hashing are structural by default —
  no overriding `Equals` boilerplate.
* **Zero overhead.** A readonly record struct lives on the stack or
  embedded inline; identity allocation is exactly one `Guid`'s worth of
  memory. Compare to wrapping each id in a class (one heap allocation
  per identifier).

The cost is one line per id type, paid once.

## Why we keep an `IReadOnlyDictionary` constant in a `static readonly` field

```csharp
private static readonly IReadOnlyDictionary<string, string> EmptyHeaders =
    new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
```

The .NET BCL has `Array.Empty<T>()` and `Enumerable.Empty<T>()` but no
`ReadOnlyDictionary<TKey, TValue>.Empty`. Without this constant, every
default-constructed `HttpRequestOptions` would allocate a fresh empty
dictionary and a fresh read-only wrapper. The field hoists the allocation
once, statically, for the lifetime of the application.

(`ImmutableDictionary<,>.Empty` is the alternative if you want an actually
immutable map; we use `ReadOnlyDictionary` here to demonstrate the wrapper
pattern the book discusses.)
