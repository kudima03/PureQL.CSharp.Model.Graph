namespace PureQL.CSharp.Model.Graph;

/// <summary>A whole query as a property graph; node 0 is the root.</summary>
/// <param name="Nodes">Every node of the query, in the order they were written.</param>
/// <param name="Relationships">Every relationship between those nodes.</param>
public sealed record QueryGraph(
    IReadOnlyList<WrittenNode> Nodes,
    IReadOnlyList<WrittenRelationship> Relationships
);
