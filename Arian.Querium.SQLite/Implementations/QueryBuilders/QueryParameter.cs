using Arian.Querium.SQL.QueryBuilders;

namespace Arian.Querium.SQLite.Implementations.QueryBuilders;

/// <summary>
/// A query parameter (name and value) used in parameterized queries.
/// </summary>
/// <remarks>
/// Constructs a QueryParameter with the given name and value.
/// </remarks>
public class QueryParameter(string name, object value) : IQueryParameter
{
    /// <summary>
    /// The name of the parameter (including prefix).
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The value of the parameter.
    /// </summary>
    public object Value { get; } = value;
}