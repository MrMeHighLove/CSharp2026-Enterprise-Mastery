# C# 2026: Enterprise Mastery — Code Examples

Companion source code for **C# 2026: Enterprise Mastery** by Victor Mihailov.

This repository hosts every code example printed in the book, organised as a
real .NET 10 solution with central package management, deterministic builds,
CI on Linux + Windows, and an analyzer-strict default configuration.

## Status at a glance

| Layer                     | What is here                                              | Compile? | Tests |
| ------------------------- | --------------------------------------------------------- | -------- | ----- |
| Infrastructure            | `.sln`, `.gitignore`, `Directory.{Build,Packages}.props`  | n/a      | n/a   |
| `CSharp2026.Common`       | Production-grade shared domain types                      | yes      | yes   |
| `CSharp2026.Chapter05`    | Strategy / Builder / Decorator / Mediator — exemplar      | yes      | yes   |
| `CSharp2026.Benchmarks`   | Runnable BenchmarkDotNet coverage for selected snippets   | yes      | yes   |
| `CSharp2026.Chapter02`...`Chapter25`, `Supplement`, `AppendixB`, `Reference` | Manuscript snippets preserved as project content | container assemblies compile — see `docs/ISSUES.md` | smoke tests |

The exemplar chapter (`Chapter05`) demonstrates the structure every other
chapter project should evolve toward: real interfaces, real implementations,
real DI wiring, real xUnit + FluentAssertions tests, and a strict analyzer
configuration with `TreatWarningsAsErrors=true`.

The remaining chapter projects preserve the book's snippets in the same
folder structure, but their `.cs` files are treated as content when the
project opts out of `TreatWarningsAsErrors`. That keeps raw manuscript
fragments, ellipses, and future-language examples available to readers
without pretending they are all complete compilation units. `docs/ISSUES.md`
catalogues the known gaps for each chapter — what stubs are needed, which
interfaces the snippets assume, and which examples are inherently
illustrative.

## Layout

```
.
├── CSharp2026-Enterprise-Mastery.sln
├── Directory.Build.props          # global build settings (every project)
├── Directory.Packages.props       # central package version management
├── global.json                    # pins .NET SDK 10.0.x
├── .editorconfig                  # C# style + naming + analyzer severities
├── .gitignore
├── .github/workflows/ci.yml       # build + test on Linux & Windows
├── docs/
│   ├── ISSUES.md                  # known compile gaps per chapter
│   └── ARCHITECTURE.md            # design decisions of Common + exemplar
├── src/
│   ├── CSharp2026.Common/         # ValueObjects, Domain, Events, Results
│   ├── CSharp2026.Chapter02/      # one project per chapter
│   ├── CSharp2026.Chapter05/      # exemplar: fully compiling
│   ├── ...
│   └── CSharp2026.Supplement/
├── benchmarks/
│   └── CSharp2026.Benchmarks/     # BenchmarkDotNet runner
└── tests/
    ├── CSharp2026.Common.Tests/
    ├── CSharp2026.Chapter05.Tests/   # exemplar test project
    ├── CSharp2026.Benchmarks.Tests/  # benchmark correctness checks
    └── ...                            # smoke tests for the others
```

## Prerequisites

* .NET 10 SDK (10.0.x)
* Any IDE that understands SDK-style projects (VS 2022 17.11+, Rider 2024.2+,
  VS Code with the C# Dev Kit, or just `dotnet` on the command line)

`global.json` pins the SDK so a `dotnet --version` mismatch will be caught
early, before a build silently picks up an older toolchain.

## Build & test

```bash
# from the repository root
dotnet restore
dotnet build  --configuration Release
dotnet test   --configuration Release --collect:"XPlat Code Coverage"
dotnet run    --configuration Release --project benchmarks/CSharp2026.Benchmarks -- --list flat
```

`Common`, `Chapter05`, and `Benchmarks` build under `/warnaserror` with no
warnings. Other chapter projects are configured as snippet containers so the
solution stays runnable while you resolve one chapter at a time — see
`docs/ISSUES.md`.

## Engineering conventions

These are the patterns the exemplar project follows; lift them into other
chapters as you fill them in.

**Strongly-typed identifiers.** Every entity has its own readonly record
struct id — `OrderId`, `CustomerId`, `ProductId`. Passing a customer id to a
method expecting an order id is a compile error, not a runtime mystery.

**Value objects for measurements.** `Money` carries amount + ISO currency
and refuses cross-currency arithmetic. Construction validates the currency
code. Tests demonstrate the failure modes are loud, not silent.

**Aggregate roots own their invariants.** `Order` exposes no setters from
outside; every state change goes through a method that also records a
domain event. The repository pattern dispatches events after the
transaction commits, then calls `ClearDomainEvents()` so they are not
dispatched twice.

**Factory abstractions over `IServiceProvider`.** When a class needs to
resolve a keyed dependency at runtime (e.g. a payment processor by
provider name), it depends on `IPaymentProcessorFactory`, not the
container. The factory has the single intentional resolution point. This
keeps business code unit-testable without spinning up DI.

**Result<T> for expected failures.** Validation and business-rule failures
flow through `Result<T>`; exceptions remain for infrastructure problems.
Avoids exception-as-flow-control and makes the failure mode visible at
the call site.

**Central package versions.** All NuGet versions live in
`Directory.Packages.props`. Project files reference packages by name,
without a `Version` attribute — upgrades happen in one place.

**Deterministic builds.** `Deterministic=true` plus
`ContinuousIntegrationBuild=true` under CI. Bit-identical output across
machines makes binary reproducibility a property of the build, not a
miracle.

## Contributing to the unfinished chapters

The recommended workflow:

1. Pick a chapter from `docs/ISSUES.md`.
2. Open `src/CSharp2026.ChapterNN/CSharp2026.ChapterNN.csproj` and remove
   the permissive `<NoWarn>` line — it serves as a TODO sentinel.
3. `dotnet build src/CSharp2026.ChapterNN` and fix what the compiler tells
   you, supplying stubs from `CSharp2026.Common` where they belong.
4. Replace the smoke test in `tests/CSharp2026.ChapterNN.Tests/` with real
   coverage of the chapter's named patterns.
5. Repeat for the next chapter.

Each chapter is a self-contained unit of work — there is no global
dependency to resolve before you can ship one chapter green.

## License

MIT — see [`LICENSE`](LICENSE).
