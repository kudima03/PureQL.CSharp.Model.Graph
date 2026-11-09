# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All `dotnet` commands must be run from the `./src` directory. The SDK is pinned by `global.json` (.NET 11, for the model's C# 15 unions); where the system SDK is older, use `~/.dotnet/dotnet` or put it first on `PATH` with `DOTNET_ROOT=$HOME/.dotnet`.

```bash
dotnet restore
dotnet build --no-restore -warnaserror
dotnet run --project PureQL.CSharp.Model.Graph.Generator   # regenerate Generated/ from the model
dotnet format --verify-no-changes          # check code style (CI enforces this)
dotnet format                              # auto-fix code style
dotnet tool restore && dotnet csharpier check .   # check formatting (CI enforces this)
dotnet test --no-build --verbosity normal  # run tests
dotnet publish Tests/PureQL.CSharp.Model.Graph.AotTests -c Release -o aot && ./aot/PureQL.CSharp.Model.Graph.AotTests
dotnet pack --configuration Release -p:Version=<version> --output .
```

CI additionally passes `-p:AssemblyVersion` (pinned to the major) and `-p:FileVersion`; see `.github/workflows/publish-nuget.yml`.

## Architecture

This is a **model-to-property-graph mapping library** for PureQL query models. It depends on `PureQL.CSharp.Model` `0.1.0-preview.14.0.0` only (generated from PureQL specification `0.1.0-preview.2.0.1`; see its CLAUDE.md for the naming rules): sealed records, one per schema node, and flat C# 15 `union`s of them (structs implementing `IUnion`, one constructor per case, `Value` the record held). No database driver: the `PureQL.CSharp.Model.Graph.Neo4j.*` packages bind it to Neo4j. The public API is a shared contract of that family; do not rename or reshape it.

**Public API (namespace `PureQL.CSharp.Model.Graph`):** `QueryGraphs.Of(PureQLQuery, Guid id)` → `QueryGraph` (`WrittenNode`s with an index, labels and properties; `WrittenRelationship`s between node indexes with a type and an optional position; node 0 is the root) and `QueryGraphs.Read(IReadNode root)` → `PureQLQuery`. `IReadNode` is a stored node read back (record label, properties, children by relationship type in position order); `ReadNode` implements it over a dictionary, a lookup or a function. `GraphLabels` (`Node`, `Query`) and `GraphProperties` (`id`, `queryId`, `position`) name what the graph adds to the model. `GraphMappingException` reports a query that cannot be written or a graph that cannot be read.

**Graph shape:** one node per record, labelled with the record's type name and `Node` (`Query` too on the root); `queryId` on every node, `id` on the root. One relationship per child, typed with the C# property name; list items carry `position`. Unions store the case they hold; `PaginationSkip` / `PaginationTake` store a `long` as the property `skip` / `take` and a `ParamInteger` as a `Skip` / `Take` relationship. Scalars are properties named after the constructor parameter, spelled as `PureQL.CSharp.Model.Serialization` writes them (decimal without trailing zeros, "D" uuid, `yyyy-MM-dd`, `HH:mm:ss[.f]`, UTC `…Z`; fractions keep every tick, up to seven digits). Enums by name, defaults written. An absent optional value is not stored; a present but empty optional record list is an empty-array property named after the list (`joins`).

**Runtime (hand-written, project root):** `GraphBuilder` collects nodes and relationships as the writers walk a query (`Set` / `SetOptional` for scalars, `Link` / `Many` / `ManyOptional` for children, `Graph` marks the root); a value the model leaves missing (a `null` string, record or list, a `default` union) is a `GraphMappingException`. `GraphReads` reads properties and children of an `IReadNode` with errors naming the node and the property or relationship. `GraphValues` holds the value representation (formatting, and parsing that also accepts any integer CLR type for a `long`, the CLR value itself, and any list).

**Generated (`PureQL.CSharp.Model.Graph/Generated/*.g.cs`):** written by `PureQL.CSharp.Model.Graph.Generator`, which reflects over the model it references (keep its `PureQL.CSharp.Model` version equal to the library's). Never edit these files; change the generator and rerun it. CI regenerates and fails on any difference. `ModelCatalog` walks the types reachable from `PureQLQuery` through constructor parameters and classifies every property (scalar, optional string, scalar list, enum, child, optional union, list, optional list, scalar union); a shape it does not know, a nested union, a union with two scalar cases, a duplicate type name or a parameter named like a graph property throws. `Emitter` writes the partial class `GraphMapping`, one file per model namespace plus `GraphMapping.Enums.g.cs`: per record `Write{Name}` (adds the node, then its properties and children in constructor order) and `Read{Name}` (calls the constructor with the values read back); per union `Write{Name}` (a switch over `Value`) and `Read{Name}` (a switch over the node label); per enum a writer and a reader by name. It also writes the entry point `QueryGraphs.g.cs`. Output is sorted by type name, so it is deterministic.

**Test project:** `PureQL.CSharp.Model.Graph.Tests` targets `net11.0` and uses xunit. `SampleRoundTripTests` writes every query of `PureQL.CSharp.Model.Samples` (86), reads it back through `StoredGraphs` (the written graph as a store returns it: lists as `List<object>`, children grouped by type and ordered by position) and compares the JSON of `PureQL.CSharp.Model.Serialization`. `ModelRoundTripTests` builds every variant of every mapped record and union by reflection (`ModelSamples`, shared with the Serialization tests) and round-trips it through its generated writer and reader. `GraphShapeTests` checks labels, ids, positions, the `skip` / `take` handling, the empty-list marker, enum defaults and the spelling of every literal type against the JSON. `MalformedGraphTests` checks that malformed graphs (wrong labels, missing or mistyped properties, missing or repeated relationships, unknown enum names, empty unions) throw a `GraphMappingException` with its message. The runtime has unit tests of its own (`GraphValuesTests`, `GraphBuilderTests`, `GraphReadsTests`, `ReadNodeTests`).

**Native AOT check:** `PureQL.CSharp.Model.Graph.AotTests` is a console application with `PublishAot` (in the solution, so it builds under the trim and AOT analyzers with warnings as errors, but it is not an xunit project). CI publishes it and runs the native binary, which round-trips samples of every root query variant and literal type and compares them through `PureQLTypeInfoResolver`. The trim warnings the publish prints come from the samples package's own dependencies, not from this library.

**Target framework:** net11.0 only, as the model. `IsAotCompatible` is `true`; no reflection in the library.

**Package validation:** enabled against `PackageValidationBaselineVersion` `0.1.0-preview.0.1.0`, the first release.

**Mutation testing:** `src/stryker-config.json` sets `language-version: Preview`, which Stryker 5.0.0 needs to parse C# 15. CI does not run it.

**Publishing:** triggered by pushing a semver tag (pattern `*.*.*`). The tag becomes the `PackageVersion`. The workflow publishes to both GitHub Packages and NuGet.org.

## Code Style

Enforced via `.editorconfig`, `dotnet format --verify-no-changes` and `dotnet csharpier check .` in CI:

- No `var` — always use explicit types
- No expression-bodied methods or constructors — use block bodies
- Expression-bodied properties and accessors are required
- File-scoped namespace declarations (`namespace Foo.Bar;`)
- Private fields prefixed with `_` in camelCase (e.g., `_options`)
- Braces always required, even for single-statement blocks
- Max line length: 90 characters
- Allman brace style — opening brace on its own line for all blocks
- `System.*` using directives sorted first, no blank line between using groups
- Parentheses required in arithmetic and relational binary operators for clarity

## Commit Messages

Do not mention Claude or AI assistance in commit messages.
