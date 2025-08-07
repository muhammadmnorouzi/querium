namespace Arian.Querium.Abstractions.SQL;

/// <summary>
/// Represents a parameter for a SQL query (name and value).
/// </summary>
public interface IQueryParameter
{
    /// <summary>
    /// The name of the parameter (including the parameter prefix, e.g. "@p0").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The value of the parameter.
    /// </summary>
    object Value { get; }
}