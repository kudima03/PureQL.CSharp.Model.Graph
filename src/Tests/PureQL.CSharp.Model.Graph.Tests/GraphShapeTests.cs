using System.Text.Json;
using PureQL.CSharp.Model.Fields;
using PureQL.CSharp.Model.Lists;
using PureQL.CSharp.Model.Literals;
using PureQL.CSharp.Model.Parameters;
using PureQL.CSharp.Model.ProjectionExpressions;
using PureQL.CSharp.Model.RowExpressions;

namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record GraphShapeTests
{
    private static readonly Guid Id = Guid.Parse("8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37");

    [Theory]
    [MemberData(nameof(Samples))]
    public void NodesAreLabelledByRecordAndCarryTheQueryId(string name)
    {
        PureQLQuery query = QuerySamples.All[name];

        QueryGraph graph = QueryGraphs.Of(query, Id);

        WrittenNode root = graph.Nodes[0];
        Assert.Equal(
            [query.Value.GetType().Name, GraphLabels.Node, GraphLabels.Query],
            root.Labels
        );
        Assert.Equal(Id.ToString("D"), root.Properties[GraphProperties.Id]);
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            WrittenNode node = graph.Nodes[i];
            Assert.Equal(i, node.Index);
            Assert.Equal(Id.ToString("D"), node.Properties[GraphProperties.QueryId]);
            Assert.Equal(GraphLabels.Node, node.Labels[1]);
            Assert.NotNull(
                typeof(PureQLQuery)
                    .Assembly.GetTypes()
                    .SingleOrDefault(t => t.Name == node.Labels[0])
            );
            if (i > 0)
            {
                Assert.Equal(2, node.Labels.Count);
                Assert.False(node.Properties.ContainsKey(GraphProperties.Id));
            }
        }
    }

    [Theory]
    [MemberData(nameof(Samples))]
    public void RelationshipsFormATreeWithPositionsOnLists(string name)
    {
        QueryGraph graph = QueryGraphs.Of(QuerySamples.All[name], Id);

        Assert.Equal(
            Enumerable.Range(1, graph.Nodes.Count - 1),
            graph.Relationships.Select(r => r.To).Order()
        );
        foreach (
            IGrouping<
                (int From, string Type),
                WrittenRelationship
            > group in graph.Relationships.GroupBy(r => (r.From, r.Type))
        )
        {
            Assert.True(char.IsUpper(group.Key.Type[0]), group.Key.Type);
            Assert.True(group.Key.From < group.Min(r => r.To));
            long?[] positions = [.. group.Select(r => r.Position)];
            Assert.True(
                positions is [null]
                    || positions.SequenceEqual(
                        Enumerable.Range(0, positions.Length).Select(p => (long?)p)
                    ),
                $"{group.Key}: {string.Join(", ", positions)}"
            );
        }
    }

    [Fact]
    public void ListPropertiesArePositionedAndSingleOnesAreNot()
    {
        QueryGraph graph = QueryGraphs.Of(Query(joins: null), Id);

        Assert.Equal(
            [
                new WrittenRelationship(0, 1, "From", null),
                new WrittenRelationship(2, 3, "Expression", null),
                new WrittenRelationship(0, 2, "Select", 0),
                new WrittenRelationship(4, 5, "Expression", null),
                new WrittenRelationship(0, 4, "Select", 1),
            ],
            graph.Relationships
        );
        Assert.Equal(
            [
                "MainPlainQueryWithoutPagination",
                "FromEntity",
                "SelectItemProjection",
                "FieldString",
                "SelectItemProjection",
                "LiteralInteger",
            ],
            graph.Nodes.Select(n => n.Labels[0])
        );
        Assert.Equal("users", graph.Nodes[1].Properties["entity"]);
        Assert.Equal("users", graph.Nodes[3].Properties["source"]);
        Assert.Equal("name", graph.Nodes[3].Properties["field"]);
        Assert.Equal(7L, graph.Nodes[5].Properties["value"]);
        Assert.False(graph.Nodes[1].Properties.ContainsKey("alias"));
    }

    [Fact]
    public void AnEmptyOptionalListIsMarkedAndAnAbsentOneIsNot()
    {
        QueryGraph empty = QueryGraphs.Of(Query(joins: []), Id);
        QueryGraph absent = QueryGraphs.Of(Query(joins: null), Id);

        Assert.Equal(Array.Empty<string>(), empty.Nodes[0].Properties["joins"]);
        Assert.False(absent.Nodes[0].Properties.ContainsKey("joins"));
        Assert.DoesNotContain(empty.Relationships, r => r.Type == "Joins");
        MainPlainQueryWithoutPagination emptyBack = Read(empty);
        MainPlainQueryWithoutPagination absentBack = Read(absent);
        Assert.Empty(emptyBack.Joins!);
        Assert.Null(absentBack.Joins);
        Assert.Null(absentBack.Subqueries);
        Assert.Null(absentBack.Where);
    }

    [Fact]
    public void AScalarListIsAnArrayEvenWhenEmpty()
    {
        QueryGraph some = QueryGraphs.Of(Query(null, ["a", "b"]), Id);
        QueryGraph none = QueryGraphs.Of(Query(null, []), Id);

        Assert.Equal(["a", "b"], (string[])some.Nodes[0].Properties["distinctBy"]);
        Assert.Equal(Array.Empty<string>(), none.Nodes[0].Properties["distinctBy"]);
        Assert.Equal(["a", "b"], Read(some).DistinctBy!);
        Assert.Empty(Read(none).DistinctBy!);
    }

    [Fact]
    public void ANumberOfSkipOrTakeIsAPropertyAndAParameterIsANode()
    {
        GraphBuilder builder = new GraphBuilder(Id);
        _ = GraphMapping.WritePagination(
            builder,
            new Pagination(
                new PaginationSkip(20L),
                new PaginationTake(new ParamInteger("pageSize"))
            )
        );
        QueryGraph graph = builder.Graph();

        Assert.Equal(20L, graph.Nodes[0].Properties["skip"]);
        Assert.False(graph.Nodes[0].Properties.ContainsKey("take"));
        Assert.Equal([new WrittenRelationship(0, 1, "Take", null)], graph.Relationships);
        Assert.Equal(["ParamInteger", GraphLabels.Node], graph.Nodes[1].Labels);
        Assert.Equal("pageSize", graph.Nodes[1].Properties["param"]);
        Pagination back = GraphMapping.ReadPagination(StoredGraphs.Root(graph));
        Assert.Equal(20L, back.Skip.Value);
        Assert.Equal(new ParamInteger("pageSize"), back.Take.Value);
    }

    [Fact]
    public void EnumsAreStoredByNameAndDefaultsAreWritten()
    {
        GraphBuilder builder = new GraphBuilder(Id);
        _ = GraphMapping.WriteOrderItemProjection(
            builder,
            new OrderItemProjection(new ExprProjection(new FieldString("users", "name")))
        );
        _ = GraphMapping.WriteJoinEntity(
            builder,
            new JoinEntity(
                JoinKind.Left,
                "orders",
                new BooleanRow(new FieldBoolean("orders", "paid"))
            )
        );
        QueryGraph graph = builder.Graph();

        Assert.Equal("Asc", graph.Nodes[0].Properties["direction"]);
        Assert.Equal("Left", graph.Nodes[2].Properties["kind"]);
        Assert.Equal(
            SortDirection.Desc,
            GraphMapping.ReadSortDirection(
                TestNodes.Of("OrderItemProjection", ("direction", "Desc")),
                "direction",
                SortDirection.Asc
            )
        );
        Assert.Equal(
            SortDirection.Asc,
            GraphMapping.ReadSortDirection(
                TestNodes.Of("OrderItemProjection"),
                "direction",
                SortDirection.Asc
            )
        );
    }

    [Theory]
    [MemberData(nameof(Literals))]
    public void LiteralValuesAreSpelledAsTheJsonSerializationWritesThem(
        string typeName,
        string expected
    )
    {
        object literal = LiteralValues().Single(l => l.GetType().Name == typeName);
        object value = Write(literal).Nodes[0].Properties["value"];

        string spelled = value is Array items
            ? $"[{string.Join(",", items.Cast<object>().Select(Spell))}]"
            : Spell(value);
        Assert.Equal(expected, spelled);
        using JsonDocument json = JsonDocument.Parse(
            PureQLJson.Serialize(literal, literal.GetType())
        );
        Assert.Equal(
            json.RootElement.GetProperty("value").GetRawText(),
            JsonSerializer.Serialize(value, value.GetType())
        );
    }

    public static TheoryData<string, string> Literals()
    {
        return new TheoryData<string, string>
        {
            { nameof(LiteralInteger), "-42" },
            { nameof(LiteralDecimal), "\"12.5\"" },
            { nameof(LiteralString), "\"name\"" },
            { nameof(LiteralBoolean), "true" },
            { nameof(LiteralDate), "\"2024-02-29\"" },
            { nameof(LiteralTime), "\"23:59:59.123456\"" },
            { nameof(LiteralDatetime), "\"2024-01-31T18:30:05.5Z\"" },
            { nameof(LiteralUuid), "\"3f2a6c1e-8b4d-4e2a-9c1f-1a2b3c4d5e60\"" },
            { nameof(ListLiteralInteger), "[1,2]" },
            { nameof(ListLiteralDecimal), "[\"1.5\",\"2\"]" },
            { nameof(ListLiteralString), "[\"a\",\"b\"]" },
            { nameof(ListLiteralBoolean), "[true,false]" },
            { nameof(ListLiteralDate), "[\"2024-02-29\"]" },
            { nameof(ListLiteralTime), "[\"00:00:01\"]" },
            { nameof(ListLiteralDatetime), "[\"1970-01-01T00:00:00Z\"]" },
            { nameof(ListLiteralUuid), "[\"3f2a6c1e-8b4d-4e2a-9c1f-1a2b3c4d5e60\"]" },
        };
    }

    [Fact]
    public void ANullLiteralHasNoValue()
    {
        QueryGraph graph = Write(new LiteralIntegerNullable());

        Assert.Equal(
            [GraphProperties.QueryId, GraphProperties.Id],
            graph.Nodes[0].Properties.Keys
        );
    }

    public static TheoryData<string> Samples()
    {
        return QuerySamples.Names();
    }

    private static IEnumerable<object> LiteralValues()
    {
        Guid uuid = Guid.Parse("3f2a6c1e-8b4d-4e2a-9c1f-1a2b3c4d5e60");
        return
        [
            new LiteralInteger(-42),
            new LiteralDecimal(12.50m),
            new LiteralString("name"),
            new LiteralBoolean(true),
            new LiteralDate(new DateOnly(2024, 2, 29)),
            new LiteralTime(new TimeOnly(23, 59, 59).Add(TimeSpan.FromTicks(1_234_560))),
            new LiteralDatetime(
                new DateTimeOffset(2024, 1, 31, 18, 30, 5, 500, TimeSpan.Zero)
            ),
            new LiteralUuid(uuid),
            new ListLiteralInteger([1, 2]),
            new ListLiteralDecimal([1.50m, 2m]),
            new ListLiteralString(["a", "b"]),
            new ListLiteralBoolean([true, false]),
            new ListLiteralDate([new DateOnly(2024, 2, 29)]),
            new ListLiteralTime([new TimeOnly(0, 0, 1)]),
            new ListLiteralDatetime([DateTimeOffset.UnixEpoch]),
            new ListLiteralUuid([uuid]),
        ];
    }

    private static string Spell(object value)
    {
        return value switch
        {
            string text => $"\"{text}\"",
            bool flag => flag ? "true" : "false",
            _ => $"{value}",
        };
    }

    private static QueryGraph Write(object record)
    {
        GraphBuilder builder = new GraphBuilder(Id);
        _ = ModelRoundTripTests.Writer(record.GetType()).Invoke(null, [builder, record]);
        return builder.Graph();
    }

    private static MainPlainQueryWithoutPagination Read(QueryGraph graph)
    {
        return Assert.IsType<MainPlainQueryWithoutPagination>(
            QueryGraphs.Read(StoredGraphs.Root(graph)).Value
        );
    }

    internal static PureQLQuery Query(
        IEnumerable<Join>? joins,
        IEnumerable<string>? distinctBy = null
    )
    {
        return new PureQLQuery(
            new MainPlainQueryWithoutPagination(
                new From(new FromEntity("users")),
                [
                    new SelectItemProjection(
                        "name",
                        new ExprProjection(new FieldString("users", "name"))
                    ),
                    new SelectItemProjection(
                        "seven",
                        new ExprProjection(new LiteralInteger(7))
                    ),
                ],
                joins: joins,
                distinctBy: distinctBy
            )
        );
    }
}
