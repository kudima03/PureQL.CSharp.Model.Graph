using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using PureQL.CSharp.Model;
using PureQL.CSharp.Model.Graph;
using PureQL.CSharp.Model.Graph.Tests;
using PureQL.CSharp.Model.Serialization;

// Every query of the samples package is written as a graph, read back and compared as
// JSON through the type info resolver. Prints the counts and exits with 1 on the first
// failure.
JsonSerializerOptions options = new JsonSerializerOptions
{
    MaxDepth = 256,
    TypeInfoResolver = new PureQLTypeInfoResolver(),
};
JsonTypeInfo<PureQLQuery> queryInfo = options.GetTypeInfo<PureQLQuery>();

Guid id = Guid.NewGuid();
int nodes = 0;
foreach ((Type type, PureQLQuery query) in SampleQueries.All)
{
    string expected = JsonSerializer.Serialize(query, queryInfo);
    QueryGraph graph = QueryGraphs.Of(query, id);
    PureQLQuery back = QueryGraphs.Read(StoredGraphs.Root(graph));
    string actual = JsonSerializer.Serialize(back, queryInfo);
    if (actual != expected)
    {
        return Fail($"{type.Name} does not round-trip:\n{expected}\n{actual}");
    }

    nodes += graph.Nodes.Count;
}

try
{
    _ = QueryGraphs.Read(
        new ReadNode(
            "MainTotalQuery",
            new Dictionary<string, object?>(),
            new Dictionary<string, IReadOnlyList<IReadNode>>()
        )
    );
    return Fail("a malformed graph is read");
}
catch (GraphMappingException error)
{
    Console.WriteLine($"A malformed graph is rejected: {error.Message}");
}

Console.WriteLine(
    $"{SampleQueries.All.Count} queries ({nodes} nodes) round-trip as graphs."
);
return 0;

static int Fail(string message)
{
    Console.Error.WriteLine($"FAILED: {message}");
    return 1;
}
