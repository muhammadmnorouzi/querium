namespace Arian.Querium.Abstractions.SQL;

/// <summary>
/// A builder for creating SELECT SQL queries in a fluent manner.
/// </summary>
public interface ISelectQueryBuilder : IQuery
{
    /// <summary>
    /// Specifies the columns to select. If not called, defaults to *.
    /// </summary>
    ISelectQueryBuilder Select(params string[] columns);

    /// <summary>
    /// Specifies the table to select from.
    /// </summary>
    ISelectQueryBuilder From(string table);

    /// <summary>
    /// Adds a WHERE clause condition for the given column with an operator and parameter value.
    /// </summary>
    ISelectQueryBuilder Where(string column, object value, string op = "=");

    /// <summary>
    /// Adds an OR condition to the WHERE clause for the given column.
    /// </summary>
    ISelectQueryBuilder Or(string column, object value, string op = "=");

    /// <summary>
    /// Adds an ORDER BY clause for the given column and sort order.
    /// </summary>
    ISelectQueryBuilder OrderBy(string column, SortOrder sortOrder = SortOrder.Ascending);
}