using System.Reflection;

namespace PureQL.CSharp.Model.Graph.Tests;

/// <summary>
/// Every variant of every record and union the mapping has a writer for (built by
/// <see cref="ModelSamples"/>) is written as a graph and read back by its generated
/// writer and reader, and must come back as the same model.
/// </summary>
public sealed record ModelRoundTripTests
{
    private static readonly Guid Id = Guid.Parse("8f2a5c1e-3b7d-4e9a-a1c2-6d4b8e0f2a37");

    [Fact]
    public void EveryReachableTypeIsMapped()
    {
        Assert.Equal(731, MappedTypes().Count(t => !ModelSamples.IsUnion(t)));
        Assert.Equal(113, MappedTypes().Count(ModelSamples.IsUnion));
    }

    [Theory]
    [MemberData(nameof(ModelTypes))]
    public void EveryVariantRoundTripsToTheSameModel(string typeName)
    {
        Type type = MappedTypes().Single(t => t.FullName == typeName);
        foreach (object value in ModelSamples.Of(type))
        {
            object back = RoundTrip(type, value);

            ModelAssert.Equivalent(value, back);
            Assert.Equal(
                PureQLJson.Serialize(value, type),
                PureQLJson.Serialize(back, type)
            );
        }
    }

    public static TheoryData<string> ModelTypes()
    {
        TheoryData<string> names = [];
        foreach (Type type in MappedTypes())
        {
            names.Add(type.FullName!);
        }

        return names;
    }

    /// <summary>Writes a value as a graph and reads its root back.</summary>
    internal static object RoundTrip(Type type, object value)
    {
        GraphBuilder builder = new GraphBuilder(Id);
        int root = (int)Writer(type).Invoke(null, [builder, value])!;
        Assert.Equal(0, root);
        IReadNode node = StoredGraphs.Root(builder.Graph());
        try
        {
            return Reader(type).Invoke(null, [node])!;
        }
        catch (TargetInvocationException error) when (error.InnerException is not null)
        {
            throw error.InnerException;
        }
    }

    /// <summary>The model types that have a generated writer of their own.</summary>
    internal static IEnumerable<Type> MappedTypes()
    {
        return ModelSamples.Types.Where(t =>
            Mapping(t, "Write")?.GetParameters() is [{ }, { } value]
            && (value.ParameterType == t)
        );
    }

    internal static MethodInfo Writer(Type type)
    {
        return Mapping(type, "Write")!;
    }

    internal static MethodInfo Reader(Type type)
    {
        return Mapping(type, "Read")!;
    }

    private static MethodInfo? Mapping(Type type, string prefix)
    {
        return typeof(GraphBuilder)
            .Assembly.GetType("PureQL.CSharp.Model.Graph.GraphMapping")!
            .GetMethod(prefix + type.Name, BindingFlags.Static | BindingFlags.NonPublic);
    }
}
