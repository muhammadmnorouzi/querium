using Arian.Querium.Abstractions.SQL;

namespace Arian.Querium.SQLite.Implementations;

/// <summary>
/// Default SQLite dialect implementation (uses standard SQLite conventions).
/// </summary>
public class SqliteDialect : ISqlDialect
{
    /// <summary>
    /// Quotes an identifier (e.g. table or column name) with double quotes.
    /// </summary>
    public string QuoteIdentifier(string identifier)
    {
        return $"\"{identifier}\"";
    }

    /// <summary>
    /// Maps a ColumnType to its SQLite type name (INTEGER, REAL, TEXT, BLOB, NUMERIC).
    /// </summary>
    public string GetColumnType(ColumnType columnType)
    {
        return columnType switch
        {
            ColumnType.Integer => "INTEGER",
            ColumnType.Real => "REAL",
            ColumnType.Text => "TEXT",
            ColumnType.Blob => "BLOB",
            ColumnType.Numeric => "NUMERIC",
            ColumnType.Boolean => "BOOLEAN", // SQLite does not have bool values, it stores them as NUMERIC
            _ => throw new ArgumentException($"Unsupported column type: {columnType}")
        };
    }
}


