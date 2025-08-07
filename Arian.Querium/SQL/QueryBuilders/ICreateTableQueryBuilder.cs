namespace Arian.Querium.SQL.QueryBuilders;

/// <summary>
/// A builder for creating CREATE TABLE SQL queries in a fluent manner.
/// </summary>
public interface ICreateTableQueryBuilder : IQuery
{
    /// <summary>
    /// Specifies the name of the table to create.
    /// </summary>
    ICreateTableQueryBuilder CreateTable(string table);

    /// <summary>
    /// Specifies that the table should only be created if it does not already exist.
    /// </summary>
    ICreateTableQueryBuilder IfNotExists();

    /// <summary>
    /// Specifies a column definition with name, type, nullability, primary key, default value, and auto-increment option.
    /// </summary>
    ICreateTableQueryBuilder Column(string name, ColumnType type, bool isNullable = true, bool isPrimaryKey = false, object? defaultValue = null, bool autoIncrement = false);
}