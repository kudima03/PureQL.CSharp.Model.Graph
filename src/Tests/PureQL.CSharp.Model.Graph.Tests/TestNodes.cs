namespace PureQL.CSharp.Model.Graph.Tests;

/// <summary>Builds <see cref="ReadNode"/>s by hand.</summary>
internal static class TestNodes
{
    public static ReadNode Of(
        string label,
        params (string Name, object? Value)[] properties
    )
    {
        return With(label, properties, []);
    }

    public static ReadNode With(
        string label,
        (string Name, object? Value)[] properties,
        params (string Type, IReadNode Child)[] children
    )
    {
        Dictionary<string, object?> values = properties.ToDictionary(
            p => p.Name,
            p => p.Value,
            StringComparer.Ordinal
        );
        Dictionary<string, IReadOnlyList<IReadNode>> lists = children
            .GroupBy(c => c.Type, StringComparer.Ordinal)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<IReadNode>)[.. g.Select(c => c.Child)],
                StringComparer.Ordinal
            );
        return new ReadNode(label, values, lists);
    }
}
