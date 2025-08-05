using Arian.Querium.Abstractions.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _ => throw new ArgumentException($"Unsupported column type: {columnType}")
        };
    }
}


