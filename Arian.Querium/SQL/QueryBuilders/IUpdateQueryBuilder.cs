namespace Arian.Querium.SQL.QueryBuilders;

/// <summary>
/// A builder for creating UPDATE SQL queries in a fluent manner.
/// </summary>
public interface IUpdateQueryBuilder : IQuery
{
    /// <summary>
    /// Specifies the table to update.
    /// </summary>
    IUpdateQueryBuilder Update(string table);

    /// <summary>
    /// Adds a column = value assignment to the SET clause.
    /// </summary>
    IUpdateQueryBuilder Set(string column, object value);

    /// <summary>
    /// Adds a WHERE clause condition for the given column with an operator and parameter value.
    /// </summary>
    IUpdateQueryBuilder Where(string column, object value, string op = "=");

    /// <summary>
    /// Adds an OR condition to the WHERE clause for the given column, operator, and parameter value.
    /// </summary>
    IUpdateQueryBuilder Or(string column, object value, string op = "=");
}