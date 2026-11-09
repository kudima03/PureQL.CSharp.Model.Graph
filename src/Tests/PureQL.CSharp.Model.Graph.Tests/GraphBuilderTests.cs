namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record GraphBuilderTests
{
    private static readonly Guid QueryId = Guid.Parse(
        "11111111-2222-3333-4444-555555555555"
    );

    private static readonly Guid VisualizationId = Guid.Parse(
        "66666666-7777-8888-9999-000000000000"
    );

    [Fact]
    public void TheFirstNodeIsTheRootQuery()
    {
        GraphBuilder builder = new GraphBuilder(QueryId);
        int root = builder.Add("Root");
        int child = builder.Add("Child");
        builder.Link(root, "Where", child);

        QueryGraph graph = builder.Graph(VisualizationId);

        Assert.Equal(
            ["Root", GraphLabels.Node, GraphLabels.Query],
            graph.Nodes[0].Labels
        );
        Assert.Equal(["Child", GraphLabels.Node], graph.Nodes[1].Labels);
        Assert.Equal([0, 1], graph.Nodes.Select(n => n.Index));
        Assert.Equal(QueryId.ToString(), graph.Nodes[0].Properties[GraphProperties.Id]);
        Assert.Equal(
            VisualizationId.ToString(),
            graph.Nodes[0].Properties[GraphProperties.VisualizationId]
        );
        Assert.All(
            graph.Nodes,
            n => Assert.Equal(QueryId.ToString(), n.Properties[GraphProperties.QueryId])
        );
        Assert.False(graph.Nodes[1].Properties.ContainsKey(GraphProperties.Id));
        Assert.Equal([new WrittenRelationship(0, 1, "Where", null)], graph.Relationships);
    }

    [Fact]
    public void ScalarsAreStoredInTheirRepresentation()
    {
        GraphBuilder builder = new GraphBuilder(QueryId);
        int node = builder.Add("Values");
        builder.Set(node, "string", "text");
        builder.Set(node, "long", 5L);
        builder.Set(node, "bool", true);
        builder.Set(node, "decimal", 1.50m);
        builder.Set(node, "uuid", QueryId);
        builder.Set(node, "date", new DateOnly(2024, 2, 29));
        builder.Set(node, "time", new TimeOnly(1, 2, 3));
        builder.Set(node, "datetime", DateTimeOffset.UnixEpoch);
        builder.Set(node, "strings", ["a", "b"]);
        builder.Set(node, "longs", [1L, 2L]);
        builder.Set(node, "bools", [true, false]);
        builder.Set(node, "decimals", [1.50m]);
        builder.Set(node, "uuids", [QueryId]);
        builder.Set(node, "dates", [new DateOnly(2024, 2, 29)]);
        builder.Set(node, "times", [new TimeOnly(1, 2, 3)]);
        builder.Set(node, "datetimes", [DateTimeOffset.UnixEpoch]);
        builder.SetOptional(node, "optional", "here");
        builder.SetOptional(node, "absent", (string?)null);
        builder.SetOptional(node, "optionals", ["x"]);
        builder.SetOptional(node, "absents", (IEnumerable<string>?)null);

        IReadOnlyDictionary<string, object> properties = builder
            .Graph(VisualizationId)
            .Nodes[0]
            .Properties;

        Assert.Equal("text", properties["string"]);
        Assert.Equal(5L, properties["long"]);
        Assert.Equal(true, properties["bool"]);
        Assert.Equal("1.5", properties["decimal"]);
        Assert.Equal(QueryId.ToString(), properties["uuid"]);
        Assert.Equal("2024-02-29", properties["date"]);
        Assert.Equal("01:02:03", properties["time"]);
        Assert.Equal("1970-01-01T00:00:00Z", properties["datetime"]);
        Assert.Equal(["a", "b"], (string[])properties["strings"]);
        Assert.Equal([1L, 2L], (long[])properties["longs"]);
        Assert.Equal([true, false], (bool[])properties["bools"]);
        Assert.Equal(["1.5"], (string[])properties["decimals"]);
        Assert.Equal([QueryId.ToString()], (string[])properties["uuids"]);
        Assert.Equal(["2024-02-29"], (string[])properties["dates"]);
        Assert.Equal(["01:02:03"], (string[])properties["times"]);
        Assert.Equal(["1970-01-01T00:00:00Z"], (string[])properties["datetimes"]);
        Assert.Equal("here", properties["optional"]);
        Assert.Equal(["x"], (string[])properties["optionals"]);
        Assert.False(properties.ContainsKey("absent"));
        Assert.False(properties.ContainsKey("absents"));
    }

    [Fact]
    public void ListItemsAreLinkedWithTheirPosition()
    {
        GraphBuilder builder = new GraphBuilder(QueryId);
        int root = builder.Add("Root");
        builder.Many(root, "Values", "values", ["a", "b"], Leaf);
        builder.ManyOptional(root, "Joins", "joins", ["c"], Leaf);
        builder.ManyOptional(root, "Empty", "empty", Array.Empty<string>(), Leaf);
        builder.ManyOptional(root, "Absent", "absent", (IEnumerable<string>?)null, Leaf);
        builder.Link(root, "Pagination", "pagination", "d", Leaf);

        QueryGraph graph = builder.Graph(VisualizationId);

        Assert.Equal(
            [
                new WrittenRelationship(0, 1, "Values", 0),
                new WrittenRelationship(0, 2, "Values", 1),
                new WrittenRelationship(0, 3, "Joins", 0),
                new WrittenRelationship(0, 4, "Pagination", null),
            ],
            graph.Relationships
        );
        Assert.Equal(["a", "b", "c", "d"], graph.Nodes.Skip(1).Select(n => n.Labels[0]));
        Assert.Equal(Array.Empty<string>(), graph.Nodes[0].Properties["empty"]);
        Assert.False(graph.Nodes[0].Properties.ContainsKey("joins"));
        Assert.False(graph.Nodes[0].Properties.ContainsKey("absent"));
    }

    [Fact]
    public void MissingValuesAreRejected()
    {
        GraphBuilder builder = new GraphBuilder(QueryId);
        int node = builder.Add("FieldString");

        AssertMissing(
            "The FieldString has no source",
            () => builder.Set(node, "source", (string?)null)
        );
        AssertMissing(
            "has no distinctBy:",
            () => builder.Set(node, "distinctBy", (IEnumerable<string>?)null)
        );
        AssertMissing(
            "has no distinctBy item",
            () => builder.Set(node, "distinctBy", ["a", null!])
        );
        AssertMissing(
            "has no value",
            () => builder.Set(node, "value", (IEnumerable<long>?)null)
        );
        AssertMissing(
            "has no value",
            () => builder.Set(node, "value", (IEnumerable<bool>?)null)
        );
        AssertMissing(
            "has no value",
            () => builder.Set(node, "value", (IEnumerable<decimal>?)null)
        );
        AssertMissing(
            "has no pagination",
            () => builder.Link(node, "Pagination", "pagination", (string?)null, Leaf)
        );
        AssertMissing(
            "has no select:",
            () => builder.Many(node, "Select", "select", (IEnumerable<string>?)null, Leaf)
        );
        AssertMissing(
            "has no select item",
            () => builder.Many<string>(node, "Select", "select", [null!], Leaf)
        );
        Assert.Equal("The Where holds no value.", GraphBuilder.Empty("Where").Message);
    }

    private static void AssertMissing(string message, Action write)
    {
        GraphMappingException error = Assert.Throws<GraphMappingException>(write);
        Assert.Contains(message, error.Message, StringComparison.Ordinal);
    }

    private static int Leaf(GraphBuilder builder, string label)
    {
        return builder.Add(label);
    }
}
