namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// The properties a query graph adds to the model's own: the identifiers of the query
/// on its nodes and the position of an item on the relationships of a list.
/// </summary>
public static class GraphProperties
{
    /// <summary>The identifier of the query, on its root node only.</summary>
    public const string Id = "id";

    /// <summary>The identifier of the query a node belongs to, on every node.</summary>
    public const string QueryId = "queryId";

    /// <summary>The identifier of the visualization of the query, on its root node only.</summary>
    public const string VisualizationId = "visualizationId";

    /// <summary>The 0-based position of an item, on every relationship of a list.</summary>
    public const string Position = "position";
}
