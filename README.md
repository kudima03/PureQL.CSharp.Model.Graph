# PureQL.CSharp.Model.Graph

Generated, driver-free mapping of a **PureQL** query model to a property graph and back.

[![.NET build & test](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/build-and-test.yml/badge.svg?branch=main)](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/build-and-test.yml)
[![Build and Deploy](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/publish-nuget.yml/badge.svg?branch=main)](https://github.com/kudima03/PureQL.CSharp.Model.Graph/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/v/PureQL.CSharp.Model.Graph)](https://www.nuget.org/packages/PureQL.CSharp.Model.Graph)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Overview

`PureQL.CSharp.Model.Graph` turns a `PureQLQuery` of
[`PureQL.CSharp.Model`](https://github.com/kudima03/PureQL.CSharp.Model) into a list of
nodes and relationships that any property-graph store can write in one statement, and
reads a stored graph back into the model. It has no database driver dependency.

Work in progress.

## Dependencies

- [`PureQL.CSharp.Model`](https://github.com/kudima03/PureQL.CSharp.Model) `0.1.0-preview.14.0.0`

## Target Frameworks

- .NET 11

## Installation

```shell
dotnet add package PureQL.CSharp.Model.Graph
```

## License

MIT
