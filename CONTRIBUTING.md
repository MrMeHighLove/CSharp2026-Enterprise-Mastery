# Contributing

## Local setup

```bash
git clone <repo-url>
cd c-sharp-book-code
dotnet restore
dotnet build  --configuration Release
dotnet test   --configuration Release
```

`global.json` pins .NET SDK 10.0.x. If your machine has only an older SDK,
install the matching one before `dotnet restore` — the CLI will refuse a
mismatch by design.

## Branching

* `main` is always green on CI (build + test on Ubuntu and Windows).
* Feature branches use the prefix `feat/`, fixes `fix/`, chapter work
  `chapter/NN-short-name`.

## Commits

Conventional Commits, short subject (≤72 chars), imperative voice:

```
feat(chapter05): add LoggingPaymentProcessor decorator
fix(common): correct ReadOnlyDictionary construction in HttpRequestOptions
docs(architecture): explain Result<T> versus exceptions
test(common): cover Money cross-currency rejection
```

Squash on merge — `main` keeps one commit per logical change.

## Code style

Style is enforced by `.editorconfig` + analyzers (set up in
`Directory.Build.props`). Before pushing:

```bash
dotnet format --verify-no-changes --severity warn
```

CI runs the same command and fails the build on diffs.

## Adding a chapter to the working set

The chapter projects ship with permissive warning settings so book
snippets compile loosely. When you bring a chapter up to senior-engineer
quality:

1. Open `src/CSharp2026.ChapterNN/CSharp2026.ChapterNN.csproj`.
2. Delete:
   ```xml
   <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
   <Nullable>annotations</Nullable>
   <NoWarn>$(NoWarn);CS0246;...</NoWarn>
   ```
3. `dotnet build src/CSharp2026.ChapterNN`. The compiler will list every
   real issue. Fix in order:
   * Missing type or namespace? Add to `CSharp2026.Common` if shared, or
     create a stub locally.
   * Nullable warning? Annotate the API correctly — don't sprinkle `!`.
   * Missing async? Either make the method `async`/`await` or document
     why a sync path is fine.
4. Replace the smoke test in `tests/CSharp2026.ChapterNN.Tests/` with
   actual coverage. Use FluentAssertions, one logical assertion per test.
5. PR title: `chapter(NN): bring to green`.

## Tests

We use **xUnit 2.x** + **FluentAssertions 6.x** + **coverlet** for
coverage. Every public type should have at least:

* a happy-path test;
* one failure mode test;
* an invariant test (where the type has invariants).

Tests live in `tests/<ProjectName>.Tests/` mirroring the source layout.

```csharp
namespace CSharp2026.Chapter05.Tests.Strategy;   // mirrors src namespace

public sealed class StrategyPatternTests
{
    [Fact]
    public async Task Orchestrator_Routes_To_The_Correct_Processor() { ... }
}
```

## Dependencies

All package versions live in `Directory.Packages.props` (Central Package
Management). Adding a NuGet:

1. Open `Directory.Packages.props`.
2. Add `<PackageVersion Include="..." Version="..." />`.
3. In the project that needs it, add `<PackageReference Include="..." />`
   — **no `Version` attribute**, the CPM file owns it.

Upgrades happen in a single PR against `Directory.Packages.props`.

## CI

`.github/workflows/ci.yml` builds and tests on Ubuntu and Windows on every
push and PR to `main`. The format job runs `dotnet format --verify-no-changes`
and gates merges.

## Reporting issues with book code

If you find a snippet in the book that doesn't match the repo (or vice
versa), open an issue tagged `book-code-drift`. Include the chapter, page
number from the printed book, and the file path in the repo. The book
text wins for prose decisions; the repo wins for executable correctness.
