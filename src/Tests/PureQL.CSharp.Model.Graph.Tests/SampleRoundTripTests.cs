namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record SampleRoundTripTests
{
    private static readonly Guid Id = Guid.Parse("8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37");

    private static readonly Guid VisualizationId = Guid.Parse(
        "3f2a6c1e-8b4d-4e2a-9c1f-1a2b3c4d5e6f"
    );

    [Fact]
    public void EverySampleIsLoaded()
    {
        Assert.Equal(86, QuerySamples.All.Count);
    }

    [Theory]
    [MemberData(nameof(Samples))]
    public void EverySampleReadsBackAsTheSameQuery(string name)
    {
        PureQLQuery query = QuerySamples.All[name];

        QueryGraph graph = QueryGraphs.Of(query, Id, VisualizationId);
        PureQLQuery back = QueryGraphs.Read(StoredGraphs.Root(graph));

        Assert.Equal(
            PureQLJson.Serialize(query, typeof(PureQLQuery)),
            PureQLJson.Serialize(back, typeof(PureQLQuery))
        );
        Assert.Equal(
            Describe(graph),
            Describe(QueryGraphs.Of(back, Id, VisualizationId))
        );
    }

    public static TheoryData<string> Samples()
    {
        return QuerySamples.Names();
    }

    private static string Describe(QueryGraph graph)
    {
        return string.Join(
            "\n",
            graph
                .Nodes.Select(n =>
                    $"{n.Index} {string.Join(":", n.Labels)} "
                    + string.Join(
                        ", ",
                        n.Properties.OrderBy(p => p.Key, StringComparer.Ordinal)
                            .Select(p => $"{p.Key}={Value(p.Value)}")
                    )
                )
                .Concat(graph.Relationships.Select(r => r.ToString()))
        );
    }

    private static string Value(object value)
    {
        return value is System.Collections.IEnumerable items and not string
            ? $"[{string.Join(", ", items.Cast<object>())}]"
            : $"{value}";
    }
}
