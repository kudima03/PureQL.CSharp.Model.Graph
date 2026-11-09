using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using PureQL.CSharp.Model.Fields;
using PureQL.CSharp.Model.Keys;
using PureQL.CSharp.Model.Lists;
using PureQL.CSharp.Model.Literals;
using PureQL.CSharp.Model.Parameters;
using PureQL.CSharp.Model.Types;

namespace PureQL.CSharp.Model.Graph.Tests;

/// <summary>
/// Builds instances of every model type by reflection: one per case of a union, and
/// for a record one with every optional value set (and non-default <c>over</c> /
/// <c>direction</c>) and one with none. The items of a list differ, as list literals
/// and <c>distinctBy</c> repeat no value, and the operands of an operator give it the
/// type it declares: a nullable operator gets nullable operands and a non-null one
/// non-null operands (a non-null <c>coalesce</c> one of each), of the type it declares
/// where the operand admits it. Operands are leaves, so that every record is built
/// once, as a case of the unions that hold it.
/// </summary>
internal static class ModelSamples
{
    private static readonly string[] TypeNames =
    [
        "integer",
        "decimal",
        "string",
        "boolean",
        "date",
        "time",
        "datetime",
        "uuid",
    ];

    private static readonly string[] Contexts = ["Row", "Projection", "Group", "Total"];

    public static IReadOnlyList<Type> Types { get; } =
    [
        .. typeof(PureQLQuery)
            .Assembly.GetExportedTypes()
            .Where(t =>
                ((t.IsClass && t.IsSealed) || IsUnion(t))
                && (t.Namespace != typeof(IType).Namespace)
            )
            .OrderBy(t => t.FullName, StringComparer.Ordinal),
    ];

    public static bool IsUnion(Type type)
    {
        return type.IsValueType && typeof(IUnion).IsAssignableFrom(type);
    }

    public static IEnumerable<object> Of(Type type)
    {
        if (IsUnion(type))
        {
            foreach (Type caseType in Cases(type))
            {
                yield return Wrap(type, Create(caseType, false));
            }

            yield break;
        }

        yield return Create(type, true);
        yield return Create(type, false);
    }

    /// <summary>
    /// Whether a type is a record declared nullable that takes an operand, so that its
    /// exact type follows from its operands.
    /// </summary>
    public static bool IsNullableOperator(Type type)
    {
        return !IsUnion(type)
            && (DeclaredType(type)?.Nullable ?? false)
            && Constructor(type).GetParameters().Any(p => IsOperand(p.ParameterType));
    }

    /// <summary>
    /// A record declared nullable built over non-null operands only and with no optional
    /// value set, so that nothing but its declaration makes it nullable.
    /// </summary>
    public static object OverNonNull(Type type)
    {
        ConstructorInfo constructor = Constructor(type);
        return constructor.Invoke([
            .. constructor
                .GetParameters()
                .Select(p => Argument(p, false, false, false, DeclaredType(type))),
        ]);
    }

    /// <summary>The type a record declares, or <see langword="null"/>.</summary>
    public static IType? DeclaredType(Type type)
    {
        PropertyInfo? property = type.IsClass ? type.GetProperty("Type") : null;
        return property?.PropertyType == typeof(IType)
            ? (IType?)property.GetValue(RuntimeHelpers.GetUninitializedObject(type))
            : null;
    }

    public static Type[] Cases(Type union)
    {
        return
        [
            .. union
                .GetConstructors()
                .Select(c => c.GetParameters())
                .Where(p => p.Length == 1)
                .Select(p => p[0].ParameterType),
        ];
    }

    private static object Create(Type type, bool full)
    {
        if (type == typeof(long))
        {
            return 20L;
        }

        ConstructorInfo constructor = Constructor(type);
        IType? declared = DeclaredType(type);
        bool nullable = declared?.Nullable ?? false;
        bool coalesce =
            type.Name.StartsWith("Coalesce", StringComparison.Ordinal) && !nullable;
        return constructor.Invoke([
            .. constructor
                .GetParameters()
                .Select(p => Argument(p, full, nullable, coalesce, declared)),
        ]);
    }

    /// <summary>A leaf case of an operand union, as described above.</summary>
    private static object Operand(Type union, bool nullable, IType? parent)
    {
        Type[] cases = Cases(union);
        Type[] leaves = [.. cases.Where(IsLeaf).OrderBy(LeafOrder)];
        Type[] candidates = leaves.Length > 0 ? leaves : cases;
        Type[] wanted = [.. candidates.Where(c => DeclaredType(c)?.Nullable == nullable)];
        wanted = wanted.Length > 0 ? wanted : candidates;
        string?[] names = [parent?.Name, NaturalType(union)];
        Type pick =
            names
                .Where(n => n is not null)
                .Select(n => wanted.FirstOrDefault(c => DeclaredType(c)?.Name == n))
                .FirstOrDefault(c => c is not null)
            ?? wanted[0];
        return Wrap(union, Create(pick, false));
    }

    private static bool IsLeaf(Type type)
    {
        return !typeof(IOperator).IsAssignableFrom(type);
    }

    private static int LeafOrder(Type type)
    {
        return type switch
        {
            _ when typeof(IField).IsAssignableFrom(type) => 0,
            _ when typeof(IKey).IsAssignableFrom(type) => 0,
            _ when typeof(IParameter).IsAssignableFrom(type) => 1,
            _ when typeof(ILiteral).IsAssignableFrom(type) => 2,
            _ when typeof(ISubqueryColumn).IsAssignableFrom(type) => 3,
            _ => 4,
        };
    }

    /// <summary>
    /// The type a position is named after: <c>decimal</c> for
    /// <c>DecimalNullableRow</c>, <c>DeclaresDecimalRow</c> and
    /// <c>DecimalOrDecimalNullableRow</c>.
    /// </summary>
    private static string? NaturalType(Type union)
    {
        string name = union.Name;
        string? context = Contexts.FirstOrDefault(c =>
            name.EndsWith(c, StringComparison.Ordinal)
        );
        if (context is null)
        {
            return null;
        }

        name = name[..^context.Length];
        name = name.StartsWith("Declares", StringComparison.Ordinal)
            ? name["Declares".Length..]
            : name;
        int or = name.IndexOf("Or", StringComparison.Ordinal);
        name =
            (or > 0) && (name.Length > (or + 2)) && char.IsUpper(name[or + 2])
                ? name[..or]
                : name;
        name = name.EndsWith("Nullable", StringComparison.Ordinal)
            ? name[..^"Nullable".Length]
            : name;
        string type = char.ToLowerInvariant(name[0]) + name[1..];
        return TypeNames.Contains(type) ? type : null;
    }

    private static ConstructorInfo Constructor(Type type)
    {
        return type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .First();
    }

    private static bool IsOperand(Type type)
    {
        Type item =
            type.IsGenericType
            && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ? type.GetGenericArguments()[0]
                : type;
        return IsUnion(item);
    }

    private static object? Argument(
        ParameterInfo parameter,
        bool full,
        bool nullable,
        bool coalesce,
        IType? declared
    )
    {
        Type type = parameter.ParameterType;
        return parameter.HasDefaultValue && !full && (type.IsClass || IsNullable(type))
            ? null
            : Value(
                Nullable.GetUnderlyingType(type) ?? type,
                full,
                0,
                nullable,
                coalesce,
                declared
            );
    }

    private static bool IsNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) is not null;
    }

    private static object Value(
        Type type,
        bool full,
        int variant,
        bool nullable,
        bool coalesce,
        IType? declared
    )
    {
        if (
            type.IsGenericType
            && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        )
        {
            Type item = type.GetGenericArguments()[0];
            Array items = Array.CreateInstance(item, 2);
            for (int i = 0; i < 2; i++)
            {
                // A non-null coalesce has exactly one non-null operand.
                bool wanted = nullable || (coalesce && (i == 0));
                items.SetValue(Value(item, false, i, wanted, false, declared), i);
            }

            return items;
        }

        return type switch
        {
            _ when type == typeof(PaginationSkip) || type == typeof(PaginationTake) =>
                Wrap(type, full ? new ParamInteger("page") : 20L),
            _ when IsUnion(type) => Operand(type, nullable, declared),
            _ when type == typeof(string) => $"name{variant}",
            _ when type == typeof(long) => -42L + variant,
            _ when type == typeof(decimal) => 12.50m + variant,
            _ when type == typeof(bool) => full ^ (variant == 1),
            _ when type == typeof(DateOnly) => new DateOnly(2024, 2, 29).AddDays(variant),
            _ when type == typeof(TimeOnly) => new TimeOnly(23, 59, 59).Add(
                TimeSpan.FromTicks((1234560 + variant) * 10)
            ),
            _ when type == typeof(DateTimeOffset) => new DateTimeOffset(
                2024,
                1,
                31,
                18,
                30,
                variant,
                TimeSpan.Zero
            ),
            _ when type == typeof(Guid) => Guid.Parse(
                $"3f2a6c1e-8b4d-4e2a-9c1f-1a2b3c4d5e6{variant}"
            ),
            _ when type == typeof(AggregateOver) => full
                ? AggregateOver.All
                : AggregateOver.Group,
            _ when type == typeof(SortDirection) => full
                ? SortDirection.Desc
                : SortDirection.Asc,
            _ when type == typeof(JoinKind) => JoinKind.Right,
            _ => Create(type, full),
        };
    }

    private static object Wrap(Type union, object value)
    {
        return union.GetConstructor([value.GetType()])!.Invoke([value]);
    }
}

/// <summary>
/// Compares two model graphs by runtime type and value, including union cases.
/// </summary>
internal static class ModelAssert
{
    public static void Equivalent(object? expected, object? actual, string path = "$")
    {
        if (expected is null || actual is null)
        {
            Assert.True(expected is null && actual is null, $"{path}: null mismatch");
            return;
        }

        Assert.True(
            expected.GetType() == actual.GetType(),
            $"{path}: expected {expected.GetType().Name}, got {actual.GetType().Name}"
        );
        Type type = expected.GetType();
        if (expected is IUnion union)
        {
            Equivalent(union.Value, ((IUnion)actual).Value, $"{path}<{type.Name}>");
        }
        else if (
            type.IsPrimitive
            || type.IsEnum
            || type.IsValueType
            || expected is string
        )
        {
            Assert.True(Equals(expected, actual), $"{path}: {expected} != {actual}");
        }
        else if (expected is IEnumerable items)
        {
            object[] left = [.. items.Cast<object>()];
            object[] right = [.. ((IEnumerable)actual).Cast<object>()];
            Assert.True(left.Length == right.Length, $"{path}: length mismatch");
            for (int i = 0; i < left.Length; i++)
            {
                Equivalent(left[i], right[i], $"{path}[{i}]");
            }
        }
        else
        {
            foreach (PropertyInfo property in type.GetProperties())
            {
                Equivalent(
                    property.GetValue(expected),
                    property.GetValue(actual),
                    $"{path}.{property.Name}"
                );
            }
        }
    }
}
