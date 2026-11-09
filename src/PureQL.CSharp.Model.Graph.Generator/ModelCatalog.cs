using System.Reflection;
using System.Runtime.CompilerServices;

namespace PureQL.CSharp.Model.Graph.Generator;

/// <summary>How a record property is stored in the graph.</summary>
internal enum PropertyKind
{
    /// <summary>A required scalar: a node property.</summary>
    Scalar,

    /// <summary>An optional scalar: a node property when present.</summary>
    OptionalScalar,

    /// <summary>A required list of scalars: an array property.</summary>
    Scalars,

    /// <summary>An optional list of scalars: an array property when present.</summary>
    OptionalScalars,

    /// <summary>An enum, required or with a default: its name as a node property.</summary>
    Enum,

    /// <summary>A required record or union: one relationship.</summary>
    Child,

    /// <summary>An optional union: one relationship when present.</summary>
    OptionalChild,

    /// <summary>A required list of records or unions: one relationship per item.</summary>
    Many,

    /// <summary>
    /// An optional list of records or unions: one relationship per item, and an
    /// empty-array property when the list is present and empty.
    /// </summary>
    OptionalMany,

    /// <summary>
    /// A union with one scalar case: a node property for the scalar, a relationship for
    /// a record case.
    /// </summary>
    ScalarUnion,
}

/// <param name="Name">The model property, <c>Values</c>: the relationship type.</param>
/// <param name="Parameter">The constructor parameter, <c>values</c>: the property name.</param>
/// <param name="Type">The value type, or the item type of a list.</param>
/// <param name="Default">The name of an enum's default value, if it has one.</param>
internal sealed record PropertyModel(
    string Name,
    string Parameter,
    PropertyKind Kind,
    Type Type,
    string? Default
);

internal sealed record RecordModel(Type Type, IReadOnlyList<PropertyModel> Properties);

/// <param name="Scalar">The scalar case of a union that has one.</param>
internal sealed record UnionModel(Type Type, IReadOnlyList<Type> Cases, Type? Scalar);

/// <summary>
/// The model types reachable from <see cref="PureQLQuery"/> through constructor
/// parameters, read by reflection from the referenced <c>PureQL.CSharp.Model</c>
/// assembly: records (sealed classes with one constructor), flat unions (structs
/// implementing <see cref="IUnion"/>, one constructor per case) and enums. A shape the
/// graph mapping does not handle throws, so that a new model version is not mapped
/// silently wrong.
/// </summary>
internal sealed class ModelCatalog
{
    /// <summary>The scalar types and the names of their readers.</summary>
    public static readonly IReadOnlyDictionary<Type, string> Scalars = new Dictionary<
        Type,
        string
    >
    {
        [typeof(string)] = "String",
        [typeof(long)] = "Integer",
        [typeof(bool)] = "Boolean",
        [typeof(decimal)] = "Decimal",
        [typeof(Guid)] = "Uuid",
        [typeof(DateOnly)] = "Date",
        [typeof(TimeOnly)] = "Time",
        [typeof(DateTimeOffset)] = "Datetime",
    };

    /// <summary>The plural names of the readers of lists.</summary>
    public static readonly IReadOnlyDictionary<Type, string> Lists = new Dictionary<
        Type,
        string
    >
    {
        [typeof(string)] = "Strings",
        [typeof(long)] = "Integers",
        [typeof(bool)] = "Booleans",
        [typeof(decimal)] = "Decimals",
        [typeof(Guid)] = "Uuids",
        [typeof(DateOnly)] = "Dates",
        [typeof(TimeOnly)] = "Times",
        [typeof(DateTimeOffset)] = "Datetimes",
    };

    /// <summary>Property names the graph itself uses.</summary>
    private static readonly string[] Reserved = ["id", "queryId"];

    public ModelCatalog(Type root)
    {
        Root = root;
        List<RecordModel> records = [];
        List<UnionModel> unions = [];
        HashSet<Type> enums = [];
        HashSet<Type> seen = [];
        Queue<Type> pending = new Queue<Type>([root]);
        while (pending.Count > 0)
        {
            Type type = pending.Dequeue();
            if (!seen.Add(type) || Scalars.ContainsKey(type))
            {
                continue;
            }

            if (type.IsEnum)
            {
                _ = enums.Add(type);
            }
            else if (IsUnion(type))
            {
                UnionModel union = Union(type);
                unions.Add(union);
                foreach (Type item in union.Cases)
                {
                    pending.Enqueue(item);
                }
            }
            else if (type.IsClass && type.IsSealed)
            {
                RecordModel record = Record(type);
                records.Add(record);
                foreach (PropertyModel property in record.Properties)
                {
                    pending.Enqueue(property.Type);
                }
            }
            else
            {
                throw new InvalidOperationException($"{type} is not a model type.");
            }
        }

        string[] duplicates =
        [
            .. records
                .Select(r => r.Type)
                .Concat(unions.Select(u => u.Type))
                .Concat(enums)
                .GroupBy(t => t.Name, StringComparer.Ordinal)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key),
        ];
        if (duplicates.Length > 0)
        {
            throw new InvalidOperationException(
                $"Model types share a name, which labels nodes: {string.Join(", ", duplicates)}"
            );
        }

        Records = [.. records.OrderBy(r => r.Type.Name, StringComparer.Ordinal)];
        Unions = [.. unions.OrderBy(u => u.Type.Name, StringComparer.Ordinal)];
        Enums = [.. enums.OrderBy(e => e.Name, StringComparer.Ordinal)];
        foreach (UnionModel union in Unions.Where(u => u.Scalar is not null))
        {
            if (union.Type == root)
            {
                throw new InvalidOperationException(
                    $"The root {root} has a scalar case."
                );
            }
        }
    }

    public Type Root { get; }

    public IReadOnlyList<RecordModel> Records { get; }

    public IReadOnlyList<UnionModel> Unions { get; }

    public IReadOnlyList<Type> Enums { get; }

    public static bool IsUnion(Type type)
    {
        return type.IsValueType && typeof(IUnion).IsAssignableFrom(type);
    }

    private static UnionModel Union(Type type)
    {
        Type[] cases =
        [
            .. type.GetConstructors()
                .Select(c => c.GetParameters())
                .Where(p => p.Length == 1)
                .Select(p => p[0].ParameterType),
        ];
        if (cases.Length == 0)
        {
            throw new InvalidOperationException($"The union {type} has no cases.");
        }

        Type[] nested = [.. cases.Where(IsUnion)];
        if (nested.Length > 0)
        {
            throw new InvalidOperationException(
                $"The union {type} nests {string.Join(", ", nested.Select(n => n.Name))}."
            );
        }

        Type[] scalars = [.. cases.Where(Scalars.ContainsKey)];
        Type[] others = [.. cases.Where(c => !Scalars.ContainsKey(c))];
        return (scalars.Length > 1) || others.Any(c => !(c.IsClass && c.IsSealed))
            ? throw new InvalidOperationException(
                $"The union {type} has cases the graph cannot tell apart."
            )
            : new UnionModel(type, cases, scalars.SingleOrDefault());
    }

    private static RecordModel Record(Type type)
    {
        ConstructorInfo[] constructors = type.GetConstructors();
        if (constructors.Length != 1)
        {
            throw new InvalidOperationException($"{type} has no single constructor.");
        }

        List<PropertyModel> properties = [];
        foreach (ParameterInfo parameter in constructors[0].GetParameters())
        {
            string name = parameter.Name!;
            if (Reserved.Contains(name))
            {
                throw new InvalidOperationException($"{type}.{name} is reserved.");
            }

            PropertyInfo property =
                type.GetProperty(char.ToUpperInvariant(name[0]) + name[1..])
                ?? throw new InvalidOperationException($"{type} has no property {name}.");
            properties.Add(Property(type, parameter, property.Name));
        }

        return new RecordModel(type, properties);
    }

    private static PropertyModel Property(
        Type record,
        ParameterInfo parameter,
        string name
    )
    {
        Type type =
            Nullable.GetUnderlyingType(parameter.ParameterType)
            ?? parameter.ParameterType;
        bool optional = parameter.HasDefaultValue && (parameter.DefaultValue is null);
        Type? item =
            type.IsGenericType
            && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ? type.GetGenericArguments()[0]
                : null;
        string parameterName = parameter.Name!;
        string? defaultName = null;
        PropertyKind kind;
        if (item is not null)
        {
            bool scalar = Scalars.ContainsKey(item);
            if (!scalar && !IsChild(item))
            {
                throw Unsupported(record, parameter);
            }

            type = item;
            kind = (scalar, optional) switch
            {
                (true, false) => PropertyKind.Scalars,
                (true, true) when item == typeof(string) => PropertyKind.OptionalScalars,
                (false, false) => PropertyKind.Many,
                (false, true) => PropertyKind.OptionalMany,
                _ => throw Unsupported(record, parameter),
            };
        }
        else if (type.IsEnum)
        {
            kind = PropertyKind.Enum;
            defaultName = parameter.HasDefaultValue
                ? Enum.GetName(type, parameter.DefaultValue!)
                    ?? throw Unsupported(record, parameter)
                : null;
        }
        else
        {
            kind = Single(record, parameter, type, optional);
        }

        return new PropertyModel(name, parameterName, kind, type, defaultName);
    }

    /// <summary>The kind of a property that is not a list nor an enum.</summary>
    private static PropertyKind Single(
        Type record,
        ParameterInfo parameter,
        Type type,
        bool optional
    )
    {
        bool scalar = Scalars.ContainsKey(type);
        bool scalarUnion = IsUnion(type) && (Union(type).Scalar is not null);
        return (scalar, scalarUnion, IsChild(type), optional, IsUnion(type)) switch
        {
            (true, _, _, false, _) => PropertyKind.Scalar,
            (true, _, _, true, _) when type == typeof(string) =>
                PropertyKind.OptionalScalar,
            (_, true, _, false, _) => PropertyKind.ScalarUnion,
            (_, _, true, false, _) => PropertyKind.Child,
            (_, _, true, true, true) => PropertyKind.OptionalChild,
            _ => throw Unsupported(record, parameter),
        };
    }

    /// <summary>A record, or a union of records only.</summary>
    private static bool IsChild(Type type)
    {
        return (type.IsClass && type.IsSealed && (type != typeof(string)))
            || (IsUnion(type) && (Union(type).Scalar is null));
    }

    private static InvalidOperationException Unsupported(
        Type record,
        ParameterInfo parameter
    )
    {
        return new InvalidOperationException(
            $"{record}.{parameter.Name} ({parameter.ParameterType}) has a shape the graph "
                + "mapping does not handle."
        );
    }
}
