namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// The labels every node of a query graph carries besides its record label (the name
/// of the model record it stores, such as <c>AddIntegerRow</c>), and the label of the
/// nodes that hold the inner lists of a list of lists.
/// </summary>
public static class GraphLabels
{
    /// <summary>The label of every node of every query graph.</summary>
    public const string Node = "Node";

    /// <summary>The label of the root node of a query graph, the query itself.</summary>
    public const string Query = "Query";

    /// <summary>
    /// The label, in place of a record label, of a node that holds one inner list of a
    /// list of lists (a row of <c>SubqueryRows.Rows</c>): the outer list links to one such
    /// node per inner list, and each links to its items, both through relationships
    /// typed with the property and carrying a <see cref="GraphProperties.Position"/>.
    /// </summary>
    public const string List = "List";
}
