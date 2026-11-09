using System.Collections;

namespace PureQL.CSharp.Model.Graph.Tests;

/// <summary>
/// A written graph read back as a store returns it, without a store: every node by its
/// record label, arrays as lists of objects, and children grouped by relationship type
/// and ordered by position.
/// </summary>
internal static class StoredGraphs
{
    public static IReadNode Root(QueryGraph graph)
    {
        List<Dictionary<string, IReadOnlyList<IReadNode>>> children = [];
        List<IReadNode> nodes = [];
        foreach (WrittenNode written in graph.Nodes)
        {
            Dictionary<string, IReadOnlyList<IReadNode>> lists =
            [
                with(StringComparer.Ordinal),
            ];
            children.Add(lists);
            nodes.Add(new ReadNode(Label(written.Labels), Properties(written), lists));
        }

        foreach (
            IGrouping<
                (int From, string Type),
                WrittenRelationship
            > group in graph.Relationships.GroupBy(r => (r.From, r.Type))
        )
        {
            children[group.Key.From][group.Key.Type] =
            [
                .. group.OrderBy(r => r.Position ?? 0).Select(r => nodes[r.To]),
            ];
        }

        return nodes[0];
    }

    private static string Label(IReadOnlyList<string> labels)
    {
        return labels.Single(l => l is not (GraphLabels.Node or GraphLabels.Query));
    }

    private static Dictionary<string, object?> Properties(WrittenNode node)
    {
        Dictionary<string, object?> properties = [with(StringComparer.Ordinal)];
        foreach (KeyValuePair<string, object> property in node.Properties)
        {
            properties[property.Key] = property.Value is IEnumerable items and not string
                ? items.Cast<object>().ToList()
                : property.Value;
        }

        return properties;
    }
}
