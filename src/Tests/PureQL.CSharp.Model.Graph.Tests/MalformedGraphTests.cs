using System.Reflection;
using PureQL.CSharp.Model.Literals;

namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record MalformedGraphTests
{
    private static readonly Guid Id = Guid.Parse("8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37");

    [Fact]
    public void TheRootIsRequired()
    {
        _ = Assert.Throws<ArgumentNullException>(() => QueryGraphs.Read(null!));
    }

    [Fact]
    public void ARootOfAnotherRecordIsRejected()
    {
        AssertRejected(
            "A FromEntity node cannot be read as a PureQLQuery: FromEntity is not one of "
                + "its cases.",
            Graph(),
            nodes: n => n.Index == 0 ? n with { Labels = ["FromEntity", "Node"] } : n
        );
    }

    [Fact]
    public void AMissingPropertyIsRejected()
    {
        AssertRejected(
            "The FromEntity node has no 'entity' property.",
            Graph(),
            nodes: n => n.Labels[0] == "FromEntity" ? Without(n, "entity") : n
        );
    }

    [Fact]
    public void AMistypedPropertyIsRejected()
    {
        AssertRejected(
            "The 'value' property of the LiteralInteger node is 'seven', not an integer.",
            Graph(),
            nodes: n => n.Labels[0] == "LiteralInteger" ? With(n, "value", "seven") : n
        );
    }

    [Fact]
    public void AnUnknownEnumNameIsRejected()
    {
        AssertRejected(
            "The 'kind' property of the JoinEntity node is 'Sideways', not a JoinKind.",
            Graph(
                joins:
                [
                    new Join(
                        new JoinEntity(
                            JoinKind.Inner,
                            "orders",
                            new RowExpressions.BooleanRow(new LiteralBoolean(true))
                        )
                    ),
                ]
            ),
            nodes: n => n.Labels[0] == "JoinEntity" ? With(n, "kind", "Sideways") : n
        );
    }

    [Fact]
    public void AMissingRelationshipIsRejected()
    {
        AssertRejected(
            "The MainPlainQueryWithoutPagination node has no 'From' relationship.",
            Graph(),
            relationships: r => r.Type != "From"
        );
    }

    [Fact]
    public void ARepeatedSingleRelationshipIsRejected()
    {
        QueryGraph graph = Graph();
        WrittenRelationship from = graph.Relationships.Single(r => r.Type == "From");
        QueryGraph twice = graph with { Relationships = [.. graph.Relationships, from] };

        AssertRejected(
            "The MainPlainQueryWithoutPagination node has 2 'From' relationships, where "
                + "one is expected.",
            twice
        );
    }

    [Fact]
    public void AChildOfAnotherRecordIsRejected()
    {
        AssertRejected(
            "A LiteralInteger node cannot be read as a From: LiteralInteger is not one of "
                + "its cases.",
            Graph(),
            nodes: n =>
                n.Labels[0] == "FromEntity"
                    ? n with
                    {
                        Labels = ["LiteralInteger", "Node"],
                    }
                    : n
        );
    }

    [Fact]
    public void PaginationNeedsANumberOrAParameter()
    {
        IReadNode none = TestNodes.Of("Pagination", ("take", 10L));
        IReadNode wrong = TestNodes.With(
            "Pagination",
            [("skip", 0L)],
            ("Take", TestNodes.Of("ParamString", ("param", "take")))
        );

        AssertRejected(
            "The Pagination node has no 'Skip' relationship.",
            () => GraphMapping.ReadPagination(none)
        );
        AssertRejected(
            "A ParamString node cannot be read as a PaginationTake",
            () => GraphMapping.ReadPagination(wrong)
        );
        AssertRejected(
            "A ParamString node cannot be read as a PaginationSkip",
            () =>
                GraphMapping.ReadPagination(
                    TestNodes.With(
                        "Pagination",
                        [("take", 1L)],
                        ("Skip", TestNodes.Of("ParamString", ("param", "skip")))
                    )
                )
        );
        AssertRejected(
            "The 'skip' property of the Pagination node is 'zero', not an integer.",
            () =>
                GraphMapping.ReadPagination(TestNodes.Of("Pagination", ("skip", "zero")))
        );
    }

    [Fact]
    public void AQueryWithoutAValueIsNotWritten()
    {
        AssertRejected(
            "The PureQLQuery holds no value.",
            () => QueryGraphs.Of(default, Id, Id)
        );
        AssertRejected(
            "The FromEntity has no entity",
            () =>
                QueryGraphs.Of(
                    new PureQLQuery(
                        new MainPlainQueryWithoutPagination(
                            new From(new FromEntity(null!)),
                            []
                        )
                    ),
                    Id,
                    Id
                )
        );
        AssertRejected(
            "The PaginationSkip holds no value.",
            () =>
                GraphMapping.WritePagination(
                    new GraphBuilder(Id),
                    new Pagination(default, new PaginationTake(1L))
                )
        );
    }

    [Theory]
    [MemberData(nameof(Unions))]
    public void EveryUnionRejectsAForeignNodeAndAnEmptyValue(string typeName)
    {
        Type union = ModelRoundTripTests
            .MappedTypes()
            .Single(t => t.FullName == typeName);

        AssertRejected(
            $"A Foreign node cannot be read as a {union.Name}",
            () => Invoke(ModelRoundTripTests.Reader(union), TestNodes.Of("Foreign"))
        );
        AssertRejected(
            $"The {union.Name} holds no value.",
            () =>
                Invoke(
                    ModelRoundTripTests.Writer(union),
                    new GraphBuilder(Id),
                    Activator.CreateInstance(union)
                )
        );
    }

    public static TheoryData<string> Unions()
    {
        TheoryData<string> names = [];
        foreach (
            Type type in ModelRoundTripTests.MappedTypes().Where(ModelSamples.IsUnion)
        )
        {
            names.Add(type.FullName!);
        }

        return names;
    }

    [Fact]
    public void EveryEnumRejectsAnUnknownValue()
    {
        Type[] enums = [typeof(AggregateOver), typeof(JoinKind), typeof(SortDirection)];
        foreach (Type type in enums)
        {
            MethodInfo write = Mapping("Write" + type.Name);
            MethodInfo read = Mapping("Read" + type.Name);

            AssertRejected($"99 is not a", () => Invoke(write, Enum.ToObject(type, 99)));
            AssertRejected(
                $"The 'name' property of the Some node is 'Unknown', not a",
                () =>
                    Invoke(read, TestNodes.Of("Some", ("name", "Unknown")), "name", null)
            );
            AssertRejected(
                "The Some node has no 'name' property.",
                () => Invoke(read, TestNodes.Of("Some"), "name", null)
            );
        }
    }

    private static MethodInfo Mapping(string name)
    {
        return typeof(GraphBuilder)
            .Assembly.GetType("PureQL.CSharp.Model.Graph.GraphMapping")!
            .GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;
    }

    private static object? Invoke(MethodInfo method, params object?[] arguments)
    {
        try
        {
            return method.Invoke(null, arguments);
        }
        catch (TargetInvocationException error) when (error.InnerException is not null)
        {
            throw error.InnerException;
        }
    }

    private static QueryGraph Graph(IEnumerable<Join>? joins = null)
    {
        return QueryGraphs.Of(GraphShapeTests.Query(joins), Id, Id);
    }

    private static WrittenNode With(WrittenNode node, string name, object value)
    {
        Dictionary<string, object> properties = new Dictionary<string, object>(
            node.Properties
        )
        {
            [name] = value,
        };
        return node with { Properties = properties };
    }

    private static WrittenNode Without(WrittenNode node, string name)
    {
        Dictionary<string, object> properties = [with(node.Properties)];
        _ = properties.Remove(name);
        return node with { Properties = properties };
    }

    private static void AssertRejected(
        string message,
        QueryGraph graph,
        Func<WrittenNode, WrittenNode>? nodes = null,
        Func<WrittenRelationship, bool>? relationships = null
    )
    {
        QueryGraph changed = new QueryGraph(
            [.. graph.Nodes.Select(nodes ?? (n => n))],
            [.. graph.Relationships.Where(relationships ?? (_ => true))]
        );
        AssertRejected(message, () => QueryGraphs.Read(StoredGraphs.Root(changed)));
    }

    private static void AssertRejected(string message, Func<object?> map)
    {
        GraphMappingException error = Assert.Throws<GraphMappingException>(() => map());
        Assert.Contains(message, error.Message, StringComparison.Ordinal);
    }
}
