namespace PureQL.CSharp.Model.Graph;

/// <summary>
/// Collects the nodes and relationships of one query as the generated writers walk it.
/// Every node gets the query's <see cref="GraphProperties.QueryId"/>; the first node
/// added is the root.
/// </summary>
internal sealed class GraphBuilder(Guid queryId)
{
    private readonly string _queryId = GraphValues.Uuid(queryId);

    private readonly List<string> _labels = [with(64)];

    private readonly List<Dictionary<string, object>> _properties = [with(64)];

    private readonly List<WrittenRelationship> _relationships = [with(64)];

    /// <summary>Adds a node of a record and returns its index.</summary>
    public int Add(string label)
    {
        _labels.Add(label);
        _properties.Add(
            new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [GraphProperties.QueryId] = _queryId,
            }
        );
        return _labels.Count - 1;
    }

    public void Set(int node, string name, string? value)
    {
        _properties[node][name] = value ?? throw Missing(node, name);
    }

    public void Set(int node, string name, long value)
    {
        _properties[node][name] = value;
    }

    public void Set(int node, string name, bool value)
    {
        _properties[node][name] = value;
    }

    public void Set(int node, string name, decimal value)
    {
        _properties[node][name] = GraphValues.Decimal(value);
    }

    public void Set(int node, string name, Guid value)
    {
        _properties[node][name] = GraphValues.Uuid(value);
    }

    public void Set(int node, string name, DateOnly value)
    {
        _properties[node][name] = GraphValues.Date(value);
    }

    public void Set(int node, string name, TimeOnly value)
    {
        _properties[node][name] = GraphValues.Time(value);
    }

    public void Set(int node, string name, DateTimeOffset value)
    {
        _properties[node][name] = GraphValues.Datetime(value);
    }

    public void Set(int node, string name, IEnumerable<string>? values)
    {
        string[] items = [.. values ?? throw Missing(node, name)];
        _properties[node][name] =
            Array.IndexOf(items, null) < 0 ? items : throw Missing(node, name + " item");
    }

    public void Set(int node, string name, IEnumerable<long>? values)
    {
        _properties[node][name] = (long[])[.. values ?? throw Missing(node, name)];
    }

    public void Set(int node, string name, IEnumerable<bool>? values)
    {
        _properties[node][name] = (bool[])[.. values ?? throw Missing(node, name)];
    }

    public void Set(int node, string name, IEnumerable<decimal>? values)
    {
        Set(node, name, values, GraphValues.Decimal);
    }

    public void Set(int node, string name, IEnumerable<Guid>? values)
    {
        Set(node, name, values, GraphValues.Uuid);
    }

    public void Set(int node, string name, IEnumerable<DateOnly>? values)
    {
        Set(node, name, values, GraphValues.Date);
    }

    public void Set(int node, string name, IEnumerable<TimeOnly>? values)
    {
        Set(node, name, values, GraphValues.Time);
    }

    public void Set(int node, string name, IEnumerable<DateTimeOffset>? values)
    {
        Set(node, name, values, GraphValues.Datetime);
    }

    /// <summary>Sets an optional value; an absent one is not stored.</summary>
    public void SetOptional(int node, string name, string? value)
    {
        if (value is not null)
        {
            Set(node, name, value);
        }
    }

    /// <summary>Sets an optional list; an absent one is not stored, an empty one is.</summary>
    public void SetOptional(int node, string name, IEnumerable<string>? values)
    {
        if (values is not null)
        {
            Set(node, name, values);
        }
    }

    /// <summary>Links a node to a child already written.</summary>
    public void Link(int from, string type, int to)
    {
        _relationships.Add(new WrittenRelationship(from, to, type, null));
    }

    /// <summary>Writes a required record child and links the node to it.</summary>
    public void Link<T>(
        int from,
        string type,
        string name,
        T? child,
        Func<GraphBuilder, T, int> write
    )
        where T : class
    {
        Link(from, type, write(this, child ?? throw Missing(from, name)));
    }

    /// <summary>Writes the items of a required list and links the node to each.</summary>
    public void Many<T>(
        int from,
        string type,
        string name,
        IEnumerable<T>? items,
        Func<GraphBuilder, T, int> write
    )
    {
        long position = 0;
        foreach (T item in items ?? throw Missing(from, name))
        {
            int to = write(this, item ?? throw Missing(from, name + " item"));
            _relationships.Add(new WrittenRelationship(from, to, type, position));
            position++;
        }
    }

    /// <summary>
    /// Writes the items of an optional list; an absent one is not stored, and an empty
    /// one is stored as an empty array property named after the list, so that the two
    /// read back apart.
    /// </summary>
    public void ManyOptional<T>(
        int from,
        string type,
        string name,
        IEnumerable<T>? items,
        Func<GraphBuilder, T, int> write
    )
    {
        if (items is null)
        {
            return;
        }

        int count = _relationships.Count;
        Many(from, type, name, items, write);
        if (_relationships.Count == count)
        {
            _properties[from][name] = Array.Empty<string>();
        }
    }

    /// <summary>The exception for a union value that holds none of its cases.</summary>
    public static GraphMappingException Empty(string union)
    {
        return new GraphMappingException($"The {union} holds no value.");
    }

    /// <summary>The graph, with the root marked as the query.</summary>
    public QueryGraph Graph()
    {
        _properties[0][GraphProperties.Id] = _queryId;
        WrittenNode[] nodes = new WrittenNode[_labels.Count];
        for (int i = 0; i < nodes.Length; i++)
        {
            string[] labels =
                i == 0
                    ? [_labels[i], GraphLabels.Node, GraphLabels.Query]
                    : [_labels[i], GraphLabels.Node];
            nodes[i] = new WrittenNode(i, labels, _properties[i]);
        }

        return new QueryGraph(nodes, [.. _relationships]);
    }

    private void Set<T>(
        int node,
        string name,
        IEnumerable<T>? values,
        Func<T, string> format
    )
    {
        _properties[node][name] = GraphValues.Strings(
            values ?? throw Missing(node, name),
            format
        );
    }

    private GraphMappingException Missing(int node, string name)
    {
        return new GraphMappingException(
            $"The {_labels[node]} has no {name}: the model leaves it missing."
        );
    }
}
