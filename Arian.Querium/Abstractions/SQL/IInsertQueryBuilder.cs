namespace Arian.Querium.Abstractions.SQL;

/// <summary>
/// A builder for creating INSERT SQL queries in a fluent manner.
/// </summary>
public interface IInsertQueryBuilder : IQuery
{
    /// <summary>
    /// Specifies the table to insert into.
    /// </summary>
    IInsertQueryBuilder Into(string table);

    /// <summary>
    /// Specifies the columns to insert.
    /// </summary>
    IInsertQueryBuilder Columns(params string[] columns);

    /// <summary>
    /// Specifies the values to insert for previously specified columns.
    /// </summary>
    IInsertQueryBuilder Values(params object[] values);
}
