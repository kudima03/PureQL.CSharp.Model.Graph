namespace PureQL.CSharp.Model.Graph;

/// <summary>A stored node read back: its record label, properties and outgoing relationships.</summary>
public interface IReadNode
{
    /// <summary>
    /// The record label, such as <c>AddIntegerRow</c>; never <see cref="GraphLabels.Node"/>
    /// or <see cref="GraphLabels.Query"/>.
    /// </summary>
    public string Label { get; }

    /// <summary>The node's properties as the store returns them.</summary>
    public IReadOnlyDictionary<string, object?> Properties { get; }

    /// <summary>
    /// The targets of the node's outgoing relationships of a type, in position order.
    /// </summary>
    /// <param name="relationshipType">The relationship type, such as <c>Values</c>.</param>
    /// <returns>The children, or an empty list if there are none.</returns>
    public IReadOnlyList<IReadNode> Children(string relationshipType);
}
