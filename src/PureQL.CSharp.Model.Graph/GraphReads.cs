using System.Collections;

namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// What the generated readers ask of a node: its properties as model values and its
/// children by relationship type, with a <see cref="GraphMappingException"/> naming the
/// node and the property or relationship for anything missing or unexpected. A
/// property holding <see langword="null"/> is absent.
/// </summary>
internal static class GraphReads
{
    private delegate bool TryRead<T>(object value, out T result);

    public static string String(IReadNode node, string name)
    {
        return Value<string>(node, name, GraphValues.TryString, "a string");
    }

    public static string? OptionalString(IReadNode node, string name)
    {
        return Has(node, name) ? String(node, name) : null;
    }

    public static long Integer(IReadNode node, string name)
    {
        return Value<long>(node, name, GraphValues.TryInteger, "an integer");
    }

    public static bool Boolean(IReadNode node, string name)
    {
        return Value<bool>(node, name, GraphValues.TryBoolean, "a boolean");
    }

    public static decimal Decimal(IReadNode node, string name)
    {
        return Value<decimal>(node, name, GraphValues.TryDecimal, "a decimal");
    }

    public static Guid Uuid(IReadNode node, string name)
    {
        return Value<Guid>(node, name, GraphValues.TryUuid, "a uuid");
    }

    public static DateOnly Date(IReadNode node, string name)
    {
        return Value<DateOnly>(node, name, GraphValues.TryDate, "a date");
    }

    public static TimeOnly Time(IReadNode node, string name)
    {
        return Value<TimeOnly>(node, name, GraphValues.TryTime, "a time");
    }

    public static DateTimeOffset Datetime(IReadNode node, string name)
    {
        return Value<DateTimeOffset>(node, name, GraphValues.TryDatetime, "a datetime");
    }

    public static string[] Strings(IReadNode node, string name)
    {
        return List<string>(node, name, GraphValues.TryString, "a string");
    }

    public static string[]? OptionalStrings(IReadNode node, string name)
    {
        return Has(node, name) ? Strings(node, name) : null;
    }

    public static long[] Integers(IReadNode node, string name)
    {
        return List<long>(node, name, GraphValues.TryInteger, "an integer");
    }

    public static bool[] Booleans(IReadNode node, string name)
    {
        return List<bool>(node, name, GraphValues.TryBoolean, "a boolean");
    }

    public static decimal[] Decimals(IReadNode node, string name)
    {
        return List<decimal>(node, name, GraphValues.TryDecimal, "a decimal");
    }

    public static Guid[] Uuids(IReadNode node, string name)
    {
        return List<Guid>(node, name, GraphValues.TryUuid, "a uuid");
    }

    public static DateOnly[] Dates(IReadNode node, string name)
    {
        return List<DateOnly>(node, name, GraphValues.TryDate, "a date");
    }

    public static TimeOnly[] Times(IReadNode node, string name)
    {
        return List<TimeOnly>(node, name, GraphValues.TryTime, "a time");
    }

    public static DateTimeOffset[] Datetimes(IReadNode node, string name)
    {
        return List<DateTimeOffset>(node, name, GraphValues.TryDatetime, "a datetime");
    }

    /// <summary>The exception for a property whose value is none of a type's.</summary>
    public static GraphMappingException NotA(IReadNode node, string name, string expected)
    {
        object? value = node.Properties.GetValueOrDefault(name);
        return new GraphMappingException(
            $"The '{name}' property of the {node.Label} node is {Describe(value)}, "
                + $"not {expected}."
        );
    }

    /// <summary>The exception for a required property that is absent.</summary>
    public static GraphMappingException Missing(IReadNode node, string name)
    {
        return new GraphMappingException(
            $"The {node.Label} node has no '{name}' property."
        );
    }

    /// <summary>The single child of a relationship type.</summary>
    public static IReadNode Child(IReadNode node, string type)
    {
        return OptionalChild(node, type)
            ?? throw new GraphMappingException(
                $"The {node.Label} node has no '{type}' relationship."
            );
    }

    /// <summary>The single child of a relationship type, or <see langword="null"/>.</summary>
    public static IReadNode? OptionalChild(IReadNode node, string type)
    {
        IReadOnlyList<IReadNode> children = Children(node, type);
        return children.Count switch
        {
            0 => null,
            1 => children[0],
            _ => throw new GraphMappingException(
                $"The {node.Label} node has {children.Count} '{type}' relationships, "
                    + "where one is expected."
            ),
        };
    }

    /// <summary>The children of a list, in position order.</summary>
    public static T[] Many<T>(IReadNode node, string type, Func<IReadNode, T> read)
    {
        IReadOnlyList<IReadNode> children = Children(node, type);
        T[] items = new T[children.Count];
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = read(children[i]);
        }

        return items;
    }

    /// <summary>
    /// The children of an optional list: absent when there are none and the node has no
    /// empty-list property named after the list.
    /// </summary>
    public static T[]? ManyOptional<T>(
        IReadNode node,
        string type,
        string name,
        Func<IReadNode, T> read
    )
    {
        T[] items = Many(node, type, read);
        return (items.Length > 0) || Has(node, name) ? items : null;
    }

    /// <summary>The exception for a node that is none of a union's cases.</summary>
    public static GraphMappingException NotACase(IReadNode node, string union)
    {
        return new GraphMappingException(
            $"A {node.Label} node cannot be read as a {union}: "
                + $"{node.Label} is not one of its cases."
        );
    }

    private static bool Has(IReadNode node, string name)
    {
        return node.Properties.GetValueOrDefault(name) is not null;
    }

    private static IReadOnlyList<IReadNode> Children(IReadNode node, string type)
    {
        IReadOnlyList<IReadNode> children = node.Children(type);
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] is null)
            {
                throw new GraphMappingException(
                    $"The '{type}' relationship {i} of the {node.Label} node "
                        + "has no node."
                );
            }
        }

        return children;
    }

    private static T Value<T>(
        IReadNode node,
        string name,
        TryRead<T> read,
        string expected
    )
    {
        object value =
            node.Properties.GetValueOrDefault(name) ?? throw Missing(node, name);
        return read(value, out T result) ? result : throw NotA(node, name, expected);
    }

    private static T[] List<T>(
        IReadNode node,
        string name,
        TryRead<T> read,
        string expected
    )
    {
        object value =
            node.Properties.GetValueOrDefault(name) ?? throw Missing(node, name);
        if (!GraphValues.TryList(value, out IEnumerable items))
        {
            throw NotA(node, name, "a list");
        }

        List<T> results = [];
        foreach (object? item in items)
        {
            if ((item is null) || !read(item, out T result))
            {
                throw new GraphMappingException(
                    $"Item {results.Count} of the '{name}' property of the "
                        + $"{node.Label} node is {Describe(item)}, not {expected}."
                );
            }

            results.Add(result);
        }

        return [.. results];
    }

    private static string Describe(object? value)
    {
        return value switch
        {
            null => "null",
            string text => $"'{text}'",
            _ => $"the {value.GetType().Name} {value}",
        };
    }
}
