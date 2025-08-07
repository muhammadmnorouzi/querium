namespace Arian.Querium.SQL.QueryBuilders;

/// <summary>
/// A builder for creating DELETE SQL queries in a fluent manner.
/// </summary>
public interface IDeleteQueryBuilder : IQuery
{
    /// <summary>
    /// Specifies the table to delete from.
    /// </summary>
    IDeleteQueryBuilder Delete(string table);

    /// <summary>
    /// Adds a WHERE clause condition for the given column with an operator and parameter value.
    /// </summary>
    IDeleteQueryBuilder Where(string column, object value, string op = "=");

    /// <summary>
    /// Adds an OR condition to the WHERE clause for the given column, operator, and parameter value.
    /// </summary>
    IDeleteQueryBuilder Or(string column, object value, string op = "=");
}