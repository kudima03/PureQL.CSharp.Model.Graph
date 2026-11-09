namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record GraphReadsTests
{
    [Fact]
    public void ScalarsReadBack()
    {
        ReadNode node = TestNodes.Of(
            "Values",
            ("string", "text"),
            ("long", 5),
            ("bool", false),
            ("decimal", "-1.5"),
            ("uuid", "8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37"),
            ("date", "2024-02-29"),
            ("time", "23:59:59.5"),
            ("datetime", "2024-02-29T23:59:59Z"),
            ("strings", new List<object> { "a", "b" }),
            ("longs", new List<object> { 1L, 2 }),
            ("bools", new List<bool> { true }),
            ("decimals", new List<string> { "1.5", "2" }),
            ("uuids", new List<string> { "8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37" }),
            ("dates", new List<string> { "2024-02-29" }),
            ("times", new List<string> { "01:02:03" }),
            ("datetimes", new List<string> { "1970-01-01T00:00:00Z" }),
            ("null", null)
        );

        Assert.Equal("text", GraphReads.String(node, "string"));
        Assert.Equal("text", GraphReads.OptionalString(node, "string"));
        Assert.Null(GraphReads.OptionalString(node, "absent"));
        Assert.Null(GraphReads.OptionalString(node, "null"));
        Assert.Equal(5L, GraphReads.Integer(node, "long"));
        Assert.False(GraphReads.Boolean(node, "bool"));
        Assert.Equal(-1.5m, GraphReads.Decimal(node, "decimal"));
        Assert.Equal(
            Guid.Parse("8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37"),
            GraphReads.Uuid(node, "uuid")
        );
        Assert.Equal(new DateOnly(2024, 2, 29), GraphReads.Date(node, "date"));
        Assert.Equal(new TimeOnly(23, 59, 59, 500), GraphReads.Time(node, "time"));
        Assert.Equal(
            new DateTimeOffset(2024, 2, 29, 23, 59, 59, TimeSpan.Zero),
            GraphReads.Datetime(node, "datetime")
        );
        Assert.Equal(["a", "b"], GraphReads.Strings(node, "strings"));
        Assert.Equal(["a", "b"], GraphReads.OptionalStrings(node, "strings")!);
        Assert.Null(GraphReads.OptionalStrings(node, "absent"));
        Assert.Equal([1L, 2L], GraphReads.Integers(node, "longs"));
        Assert.Equal([true], GraphReads.Booleans(node, "bools"));
        Assert.Equal([1.5m, 2m], GraphReads.Decimals(node, "decimals"));
        Assert.Equal(
            [Guid.Parse("8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37")],
            GraphReads.Uuids(node, "uuids")
        );
        Assert.Equal([new DateOnly(2024, 2, 29)], GraphReads.Dates(node, "dates"));
        Assert.Equal([new TimeOnly(1, 2, 3)], GraphReads.Times(node, "times"));
        Assert.Equal([DateTimeOffset.UnixEpoch], GraphReads.Datetimes(node, "datetimes"));
    }

    [Fact]
    public void MissingAndMistypedPropertiesAreRejected()
    {
        ReadNode node = TestNodes.Of(
            "LiteralInteger",
            ("value", "five"),
            ("list", "a,b"),
            ("items", new List<object?> { 1L, null }),
            ("null", null)
        );

        AssertRejected(
            "The LiteralInteger node has no 'absent' property.",
            () => GraphReads.Integer(node, "absent")
        );
        AssertRejected(
            "The LiteralInteger node has no 'null' property.",
            () => GraphReads.String(node, "null")
        );
        AssertRejected(
            "The 'value' property of the LiteralInteger node is 'five', not an integer.",
            () => GraphReads.Integer(node, "value")
        );
        AssertRejected(
            "The 'value' property of the LiteralInteger node is 'five', not a boolean.",
            () => GraphReads.Boolean(node, "value")
        );
        AssertRejected(
            "is 'five', not a decimal.",
            () => GraphReads.Decimal(node, "value")
        );
        AssertRejected("is 'five', not a uuid.", () => GraphReads.Uuid(node, "value"));
        AssertRejected("is 'five', not a date.", () => GraphReads.Date(node, "value"));
        AssertRejected("is 'five', not a time.", () => GraphReads.Time(node, "value"));
        AssertRejected(
            "is 'five', not a datetime.",
            () => GraphReads.Datetime(node, "value")
        );
        AssertRejected(
            "The 'list' property of the LiteralInteger node is 'a,b', not a list.",
            () => GraphReads.Strings(node, "list")
        );
        AssertRejected(
            "The LiteralInteger node has no 'absent' property.",
            () => GraphReads.Integers(node, "absent")
        );
        AssertRejected(
            "Item 1 of the 'items' property of the LiteralInteger node is null, not an "
                + "integer.",
            () => GraphReads.Integers(node, "items")
        );
        AssertRejected(
            "Item 0 of the 'items' property of the LiteralInteger node is the Int64 1, not "
                + "a string.",
            () => GraphReads.Strings(node, "items")
        );
        AssertRejected("Item 0", () => GraphReads.Booleans(node, "items"));
        AssertRejected("Item 0", () => GraphReads.Decimals(node, "items"));
        AssertRejected("Item 0", () => GraphReads.Uuids(node, "items"));
        AssertRejected("Item 0", () => GraphReads.Dates(node, "items"));
        AssertRejected("Item 0", () => GraphReads.Times(node, "items"));
        AssertRejected("Item 0", () => GraphReads.Datetimes(node, "items"));
        AssertRejected(
            "The 'value' property of the LiteralInteger node is 'five', not a JoinKind.",
            () => throw GraphReads.NotA(node, "value", "a JoinKind")
        );
    }

    [Fact]
    public void ChildrenAreReadByRelationshipType()
    {
        ReadNode node = TestNodes.With(
            "Parent",
            [("empty", Array.Empty<string>())],
            ("One", TestNodes.Of("A")),
            ("Two", TestNodes.Of("B")),
            ("Two", TestNodes.Of("C"))
        );

        Assert.Equal("A", GraphReads.Child(node, "One").Label);
        Assert.Equal("A", GraphReads.OptionalChild(node, "One")!.Label);
        Assert.Null(GraphReads.OptionalChild(node, "None"));
        Assert.Equal(["B", "C"], GraphReads.Many(node, "Two", n => n.Label));
        Assert.Empty(GraphReads.Many(node, "None", n => n.Label));
        Assert.Equal(
            ["B", "C"],
            GraphReads.ManyOptional(node, "Two", "two", n => n.Label)!
        );
        Assert.Empty(GraphReads.ManyOptional(node, "Empty", "empty", n => n.Label)!);
        Assert.Null(GraphReads.ManyOptional(node, "None", "none", n => n.Label));
        AssertRejected(
            "The Parent node has no 'None' relationship.",
            () => GraphReads.Child(node, "None")
        );
        AssertRejected(
            "The Parent node has 2 'Two' relationships, where one is expected.",
            () => GraphReads.Child(node, "Two")
        );
    }

    [Fact]
    public void MissingChildNodesAreRejected()
    {
        ReadNode node = TestNodes.With("Parent", [], ("Values", null!));

        AssertRejected(
            "The 'Values' relationship 0 of the Parent node has no node.",
            () => GraphReads.Many(node, "Values", n => n.Label)
        );
    }

    [Fact]
    public void ANodeOfAnotherRecordIsNotACase()
    {
        Assert.Equal(
            "A FieldString node cannot be read as a BooleanRow: FieldString is not one of "
                + "its cases.",
            GraphReads.NotACase(TestNodes.Of("FieldString"), "BooleanRow").Message
        );
        Assert.Equal(
            "The FieldString node has no 'field' property.",
            GraphReads.Missing(TestNodes.Of("FieldString"), "field").Message
        );
    }

    private static void AssertRejected(string message, Action read)
    {
        GraphMappingException error = Assert.Throws<GraphMappingException>(read);
        Assert.Contains(message, error.Message, StringComparison.Ordinal);
    }
}
