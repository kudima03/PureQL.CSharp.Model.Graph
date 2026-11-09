namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// The labels every node of a query graph carries besides its record label (the name
/// of the model record it stores, such as <c>AddIntegerRow</c>).
/// </summary>
public static class GraphLabels
{
    /// <summary>The label of every node of every query graph.</summary>
    public const string Node = "Node";

    /// <summary>The label of the root node of a query graph, the query itself.</summary>
    public const string Query = "Query";
}
