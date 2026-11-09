# PureQL.CSharp.Model.Graph

Generated, driver-free mapping of a **PureQL** query model to a property graph and back.

[![.NET build & test](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/build-and-test.yml)
[![Build and Deploy](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/PureQL.CSharp.Model.Graph)](https://www.nuget.org/packages/PureQL.CSharp.Model.Graph)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

`PureQL.CSharp.Model.Graph` turns a `PureQLQuery` of
[`PureQL.CSharp.Model`](https://github.com/kudima03/PureQL.CSharp.Model) into a list of
nodes and relationships that a property-graph store can write in one statement (for
example one `UNWIND` over the nodes and one over the relationships), and reads a stored
graph back into the model. It has no database driver dependency: store bindings, such as
the `PureQL.CSharp.Model.Graph.Neo4j.*` packages, pass the lists to their driver and build
`ReadNode`s from what the driver returns.

The writers and readers of every model record and union are generated ahead of time from
the model, so the library uses no reflection and is trim- and Native AOT-compatible.

## Graph shape

- **One node per model record.** Labels: the record's type name (`AddIntegerRow`) and
  `Node`; the root record also has `Query`. Every node has `queryId` (the query's id, a
  "D" uuid string); the root also has `id` (the same value).
- **One relationship per child.** Its type is the C# property that holds the child
  (`Where`, `Left`, `Values`, `Columns`, …). Each item of a list gets its own relationship
  with an integer `position` (0-based); a single child has none.
- **Unions are transparent.** Only the case a union holds is stored; on read, the node's
  record label picks the case. `PaginationSkip` and `PaginationTake` (`long | ParamInteger`)
  store a number as the property `skip` / `take` of the `Pagination` node and a parameter
  as a `Skip` / `Take` relationship to a `ParamInteger` node.
- **Scalars are node properties** named after the constructor parameter (`source`,
  `field`, `param`, `value`, `alias`, `kind`, `direction`, `over`, …), spelled as
  [`PureQL.CSharp.Model.Serialization`](https://github.com/kudima03/PureQL.CSharp.Model.Serialization)
  writes them in JSON: `long`, `bool` and `string` as they are; `decimal` as an invariant
  string without exponent or trailing zeros (`"12.5"`); `Guid` in the "D" format;
  `DateOnly` as `yyyy-MM-dd`; `TimeOnly` as `HH:mm:ss[.fffffff]` and `DateTimeOffset` as
  `yyyy-MM-ddTHH:mm:ss[.fffffff]Z` in UTC, the fraction without trailing zeros. Lists of
  scalars (`distinctBy`, list literals) are `string[]`, `long[]` or `bool[]`. Enums are
  stored by name (`Inner`, `Desc`, `All`), defaults included. The computed `Type` is not
  stored.
- **Absent and empty.** An absent optional value is not stored. An optional list of
  records that is present but empty is stored as an empty-array property named after the
  list (`joins = []`), so `[]` and absent read back apart.

Reading accepts that representation, any integer CLR type where a `long` is expected, the
CLR value itself, and any list where an array is expected; a property holding `null` is
absent. A graph that does not map to the model (a missing or extra relationship, a
missing or mistyped property, a label that is not a case of the union expected there)
throws a `GraphMappingException` naming the node and the property or relationship.

## Dependencies

- [`PureQL.CSharp.Model`](https://github.com/kudima03/PureQL.CSharp.Model) `0.1.0-preview.14.0.0`

## Target Frameworks

- .NET 11

## Installation

```shell
dotnet add package PureQL.CSharp.Model.Graph
```

## Usage

```csharp
using PureQL.CSharp.Model;
using PureQL.CSharp.Model.Graph;

QueryGraph graph = QueryGraphs.Of(query, id: Guid.NewGuid());

foreach (WrittenNode node in graph.Nodes)
{
    // node.Index, node.Labels (record label, "Node", and "Query" on node 0),
    // node.Properties (strings, longs, booleans and arrays of them)
}

foreach (WrittenRelationship relationship in graph.Relationships)
{
    // relationship.From, relationship.To (node indexes), relationship.Type, relationship.Position
}

// Reading back: a store binding builds one ReadNode per stored node, with its record
// label, its properties and its children by relationship type in position order.
Dictionary<string, IReadOnlyList<IReadNode>> children =
    new Dictionary<string, IReadOnlyList<IReadNode>>();
ReadNode root = new ReadNode("MainPlainQueryWithoutPagination", properties, children);
// ... fill children["From"], children["Select"], ... with the nodes they point to

PureQLQuery read = QueryGraphs.Read(root);
```

`ReadNode` also takes the children as an `ILookup<string, IReadNode>` or as a function
from relationship type to children; any other `IReadNode` implementation works too.

## Related packages

- [`PureQL.CSharp.Model`](https://github.com/kudima03/PureQL.CSharp.Model) — the model
- [`PureQL.CSharp.Model.Serialization`](https://github.com/kudima03/PureQL.CSharp.Model.Serialization) — JSON for the same model
- `PureQL.CSharp.Model.Graph.Neo4j.*` — storing, querying, updating and deleting query graphs in Neo4j

## License

MIT
