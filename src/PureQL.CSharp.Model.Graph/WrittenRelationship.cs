namespace PureQL.CSharp.Model.Graph;

/// <summary>A relationship to store between two written nodes, by their indexes.</summary>
/// <param name="From">The index of the node that holds the child.</param>
/// <param name="To">The index of the child node.</param>
/// <param name="Type">The model property that holds the child, such as <c>Where</c>.</param>
/// <param name="Position">
/// The 0-based position of the child in a list, or <see langword="null"/> for a single
/// child.
/// </param>
public sealed record WrittenRelationship(int From, int To, string Type, long? Position);
