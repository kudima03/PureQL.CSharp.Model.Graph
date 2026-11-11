# Changelog

All notable changes to `PureQL.CSharp.Model.Graph` are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0-preview.1.1.0] - 2026-11-11

### Changed

- Updated the `PureQL.CSharp.Model` dependency to `0.1.0-preview.14.1.0`.
- Built and tested with the .NET 11 GA SDK.

## [0.1.0-preview.1.0.0] - 2026-11-09

A stored query no longer knows which visualization uses it: visualizations hold the
query's id instead.

### Changed

- **Breaking:** `QueryGraphs.Of(PureQLQuery, Guid id)` replaces
  `QueryGraphs.Of(PureQLQuery, Guid id, Guid visualizationId)`. The root node keeps `id`
  and `queryId` and no longer carries a `visualizationId` property.

### Removed

- **Breaking:** `GraphProperties.VisualizationId`.

## [0.1.0-preview.0.1.0] - 2026-11-09

First release.

### Added

- `QueryGraphs.Of(PureQLQuery, Guid id, Guid visualizationId)` maps a PureQL query to a
  property graph (`QueryGraph`): one `WrittenNode` per model record, labelled with its type
  name, and one `WrittenRelationship` per child, typed with the property name and carrying
  a `position` for list items.
- `QueryGraphs.Read(IReadNode root)` reads a stored graph back into a `PureQLQuery`;
  `ReadNode` implements `IReadNode` over a dictionary, a lookup or a function.
- `GraphLabels` and `GraphProperties` name the labels (`Node`, `Query`) and properties
  (`id`, `queryId`, `visualizationId`, `position`) the graph adds to the model.
- `GraphMappingException` reports a query that cannot be written or a graph that cannot be
  read, naming the node and the property or relationship at fault.
- Scalars are stored as `PureQL.CSharp.Model.Serialization` spells them in JSON.
- Native AOT compatible: the library uses no reflection.
- Targets `net11.0` and depends on `PureQL.CSharp.Model` `0.1.0-preview.14.0.0`.

[Unreleased]: https://github.com/kudima03/PureQL.CSharp.Model.Graph/compare/0.1.0-preview.1.0.0...HEAD
[0.1.0-preview.1.0.0]: https://github.com/kudima03/PureQL.CSharp.Model.Graph/compare/0.1.0-preview.0.1.0...0.1.0-preview.1.0.0
[0.1.0-preview.0.1.0]: https://github.com/kudima03/PureQL.CSharp.Model.Graph/releases/tag/0.1.0-preview.0.1.0
