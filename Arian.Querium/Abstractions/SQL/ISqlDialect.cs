namespace Arian.Querium.Abstractions.SQL;

/// <summary>
/// Represents a SQL dialect interface (quoting, type mapping) for SQLite.
/// </summary>
public interface ISqlDialect
{
    /// <summary>
    /// Quotes an identifier (such as table or column name) according to the dialect rules.
    /// </summary>
    string QuoteIdentifier(string identifier);

    /// <summary>
    /// Gets the SQL type name for a given ColumnType.
    /// </summary>
    string GetColumnType(ColumnType columnType);
}
