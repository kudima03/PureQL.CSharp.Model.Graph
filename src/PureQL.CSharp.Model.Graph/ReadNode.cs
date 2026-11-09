namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// A node read back from a store, built by a store binding from what the store returns:
/// the record label (the node's labels without <see cref="GraphLabels.Node"/> and
/// <see cref="GraphLabels.Query"/>), its properties, and its children by relationship
/// type, each list in the order of the relationships' <see cref="GraphProperties.Position"/>.
/// </summary>
public sealed class ReadNode : IReadNode
{
    private static readonly IReadOnlyList<IReadNode> None = [];

    private readonly Func<string, IReadOnlyList<IReadNode>?> _children;

    /// <summary>Creates a node whose children are looked up in a dictionary.</summary>
    /// <param name="label">The record label.</param>
    /// <param name="properties">The node's properties.</param>
    /// <param name="children">
    /// The children by relationship type, in position order. The dictionary is not
    /// copied, so it can be filled after the node is created.
    /// </param>
    public ReadNode(
        string label,
        IReadOnlyDictionary<string, object?> properties,
        IReadOnlyDictionary<string, IReadOnlyList<IReadNode>> children
    )
        : this(label, properties, Lookup(children)) { }

    /// <summary>Creates a node whose children are grouped by relationship type.</summary>
    /// <param name="label">The record label.</param>
    /// <param name="properties">The node's properties.</param>
    /// <param name="children">The children by relationship type, in position order.</param>
    public ReadNode(
        string label,
        IReadOnlyDictionary<string, object?> properties,
        ILookup<string, IReadNode> children
    )
        : this(label, properties, Lookup(children)) { }

    /// <summary>Creates a node whose children are resolved by a function.</summary>
    /// <param name="label">The record label.</param>
    /// <param name="properties">The node's properties.</param>
    /// <param name="children">
    /// The children of a relationship type, in position order, or
    /// <see langword="null"/> for none.
    /// </param>
    public ReadNode(
        string label,
        IReadOnlyDictionary<string, object?> properties,
        Func<string, IReadOnlyList<IReadNode>?> children
    )
    {
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(children);
        Label = label;
        Properties = properties;
        _children = children;
    }

    /// <inheritdoc/>
    public string Label { get; }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object?> Properties { get; }

    /// <inheritdoc/>
    public IReadOnlyList<IReadNode> Children(string relationshipType)
    {
        return _children(relationshipType) ?? None;
    }

    private static Func<string, IReadOnlyList<IReadNode>?> Lookup(
        IReadOnlyDictionary<string, IReadOnlyList<IReadNode>> children
    )
    {
        ArgumentNullException.ThrowIfNull(children);
        return type => children.GetValueOrDefault(type);
    }

    private static Func<string, IReadOnlyList<IReadNode>?> Lookup(
        ILookup<string, IReadNode> children
    )
    {
        ArgumentNullException.ThrowIfNull(children);
        Dictionary<string, IReadOnlyList<IReadNode>> lists = children.ToDictionary(
            group => group.Key,
            group => (IReadOnlyList<IReadNode>)[.. group],
            StringComparer.Ordinal
        );
        return type => lists.GetValueOrDefault(type);
    }
}
