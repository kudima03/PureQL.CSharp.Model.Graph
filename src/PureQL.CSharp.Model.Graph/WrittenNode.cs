namespace PureQL.CSharp.Model.Graph;

/// <summary>A node to store: its labels and scalar properties.</summary>
/// <param name="Index">
/// The node's index in <see cref="QueryGraph.Nodes"/>, by which relationships refer to it.
/// </param>
/// <param name="Labels">
/// The record label first, then <see cref="GraphLabels.Node"/>, then, on the root only,
/// <see cref="GraphLabels.Query"/>.
/// </param>
/// <param name="Properties">
/// The scalar properties: strings, longs, booleans and arrays of them.
/// </param>
public sealed record WrittenNode(
    int Index,
    IReadOnlyList<string> Labels,
    IReadOnlyDictionary<string, object> Properties
);
