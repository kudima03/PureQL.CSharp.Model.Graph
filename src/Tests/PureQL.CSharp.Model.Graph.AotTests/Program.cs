using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using PureQL.CSharp.Model;
using PureQL.CSharp.Model.Graph;
using PureQL.CSharp.Model.Graph.Tests;
using PureQL.CSharp.Model.Samples.Queries.Aggregates;
using PureQL.CSharp.Model.Samples.Queries.Dashboards;
using PureQL.CSharp.Model.Samples.Queries.GroupBy;
using PureQL.CSharp.Model.Samples.Queries.Joins;
using PureQL.CSharp.Model.Samples.Queries.Select;
using PureQL.CSharp.Model.Samples.Queries.Subqueries;
using PureQL.CSharp.Model.Samples.Queries.Temporal;
using PureQL.CSharp.Model.Samples.Queries.Where;
using PureQL.CSharp.Model.Serialization;

// Samples of every root query variant in the samples, and of every literal type, are
// written as graphs, read back and compared as JSON through the type info resolver.
// Prints the counts and exits with 1 on the first failure.
JsonSerializerOptions options = new JsonSerializerOptions
{
    MaxDepth = 256,
    TypeInfoResolver = new PureQLTypeInfoResolver(),
};
JsonTypeInfo<PureQLQuery> queryInfo = options.GetTypeInfo<PureQLQuery>();

(string Name, PureQLQuery Query)[] samples =
[
    (nameof(AggregatesOverAllRowsQuery), new AggregatesOverAllRowsQuery().Value),
    (nameof(LineRestockSuggestionQuery), new LineRestockSuggestionQuery().Value),
    (nameof(EmployeeScorecardQuery), new EmployeeScorecardQuery().Value),
    (nameof(HavingBooleanLogicQuery), new HavingBooleanLogicQuery().Value),
    (nameof(MultipleJoinsQuery), new MultipleJoinsQuery().Value),
    (
        nameof(LiteralOfEveryTypeNextToFieldQuery),
        new LiteralOfEveryTypeNextToFieldQuery().Value
    ),
    (nameof(FromSubqueryQuery), new FromSubqueryQuery().Value),
    (nameof(DatetimeUtcLiteralsQuery), new DatetimeUtcLiteralsQuery().Value),
    (
        nameof(InListParameterAndNotInListLiteralQuery),
        new InListParameterAndNotInListLiteralQuery().Value
    ),
];

Guid id = Guid.NewGuid();
Guid visualizationId = Guid.NewGuid();
int nodes = 0;
foreach ((string name, PureQLQuery query) in samples)
{
    string expected = JsonSerializer.Serialize(query, queryInfo);
    QueryGraph graph = QueryGraphs.Of(query, id, visualizationId);
    PureQLQuery back = QueryGraphs.Read(StoredGraphs.Root(graph));
    string actual = JsonSerializer.Serialize(back, queryInfo);
    if (actual != expected)
    {
        return Fail($"{name} does not round-trip:\n{expected}\n{actual}");
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

Console.WriteLine($"{samples.Length} queries ({nodes} nodes) round-trip as graphs.");
return 0;

static int Fail(string message)
{
    Console.Error.WriteLine($"FAILED: {message}");
    return 1;
}
