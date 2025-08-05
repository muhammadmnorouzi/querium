using Arian.Querium.Abstractions.SQL;

namespace Arian.Querium.SQLite.Implementations;

/// <summary>
/// A query parameter (name and value) used in parameterized queries.
/// </summary>
public class QueryParameter : IQueryParameter
{
    /// <summary>
    /// The name of the parameter (including prefix).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The value of the parameter.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Constructs a QueryParameter with the given name and value.
    /// </summary>
    public QueryParameter(string name, object value)
    {
        Name = name;
        Value = value;
    }
}


