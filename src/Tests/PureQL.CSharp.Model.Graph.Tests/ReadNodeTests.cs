namespace PureQL.CSharp.Model.Graph.Tests;

public sealed record ReadNodeTests
{
    private static readonly Dictionary<string, object?> Properties = new Dictionary<
        string,
        object?
    >
    {
        ["value"] = 1L,
    };

    [Fact]
    public void ChildrenComeFromADictionary()
    {
        IReadNode child = new ReadNode("Child", Properties, Children());
        Dictionary<string, IReadOnlyList<IReadNode>> children = [];
        ReadNode node = new ReadNode("Parent", Properties, children);
        children["Values"] = [child];

        Assert.Equal("Parent", node.Label);
        Assert.Same(Properties, node.Properties);
        Assert.Same(child, Assert.Single(node.Children("Values")));
        Assert.Empty(node.Children("Other"));
    }

    [Fact]
    public void ChildrenComeFromALookup()
    {
        IReadNode first = new ReadNode("First", Properties, Children());
        IReadNode second = new ReadNode("Second", Properties, Children());
        ILookup<string, IReadNode> children = new[] { first, second }.ToLookup(_ =>
            "Values"
        );

        ReadNode node = new ReadNode("Parent", Properties, children);

        Assert.Equal([first, second], node.Children("Values"));
        Assert.Empty(node.Children("Other"));
    }

    [Fact]
    public void ChildrenComeFromAFunction()
    {
        IReadNode child = new ReadNode("Child", Properties, Children());

        ReadNode node = new ReadNode(
            "Parent",
            Properties,
            type => type == "Values" ? [child] : null
        );

        Assert.Equal([child], node.Children("Values"));
        Assert.Empty(node.Children("Other"));
    }

    [Fact]
    public void ArgumentsAreRequired()
    {
        _ = Assert.Throws<ArgumentNullException>(() =>
            new ReadNode(null!, Properties, Children())
        );
        _ = Assert.Throws<ArgumentNullException>(() =>
            new ReadNode("Node", null!, Children())
        );
        _ = Assert.Throws<ArgumentNullException>(() =>
            new ReadNode(
                "Node",
                Properties,
                (IReadOnlyDictionary<string, IReadOnlyList<IReadNode>>)null!
            )
        );
        _ = Assert.Throws<ArgumentNullException>(() =>
            new ReadNode("Node", Properties, (ILookup<string, IReadNode>)null!)
        );
        _ = Assert.Throws<ArgumentNullException>(() =>
            new ReadNode(
                "Node",
                Properties,
                (Func<string, IReadOnlyList<IReadNode>?>)null!
            )
        );
    }

    private static Dictionary<string, IReadOnlyList<IReadNode>> Children()
    {
        return [];
    }
}
