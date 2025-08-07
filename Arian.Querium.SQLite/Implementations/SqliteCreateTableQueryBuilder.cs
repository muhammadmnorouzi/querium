using Arian.Querium.Abstractions.SQL;

namespace Arian.Querium.SQLite.Implementations;

/// <summary>
/// SQLite implementation of ICreateTableQueryBuilder.
/// </summary>
public class SqliteCreateTableQueryBuilder : SqliteQueryBuilderBase, ICreateTableQueryBuilder
{
    private string _table = string.Empty;
    private bool _ifNotExists = false;
    private readonly List<string> _columnDefs = [];

    /// <summary>
    /// Specifies the table to create.
    /// </summary>
    public ICreateTableQueryBuilder CreateTable(string table)
    {
        _table = table;
        return this;
    }

    /// <summary>
    /// Specifies that the table should be created only if it does not already exist.
    /// </summary>
    public ICreateTableQueryBuilder IfNotExists()
    {
        _ifNotExists = true;
        return this;
    }

    /// <summary>
    /// Adds a column definition with type and constraints.
    /// </summary>
    public ICreateTableQueryBuilder Column(string name, ColumnType type, bool isNullable = true, bool isPrimaryKey = false, object? defaultValue = null, bool autoIncrement = false)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Column name must be provided.");
        }

        string colDef = $"{name} {_dialect.GetColumnType(type)}";
        if (isPrimaryKey)
        {
            colDef += " PRIMARY KEY";
            if (autoIncrement)
            {
                colDef += " AUTOINCREMENT";
            }
        }
        if (!isNullable)
        {
            colDef += " NOT NULL";
        }

        if (defaultValue != null)
        {
            colDef += $" DEFAULT {FormatLiteral(defaultValue)}";
        }
        _columnDefs.Add(colDef);
        return this;
    }

    private string FormatLiteral(object value)
    {
        if (value is string)
        {
            return $"'{value}'";
        }

        if (value is bool)
        {
            return (bool)value ? "1" : "0";
        }

        if (value is DateTime dt)
        {
            return $"'{dt:yyyy-MM-dd HH:mm:ss}'";
        }

        return value.ToString();
    }

    /// <summary>
    /// Builds the final SQL string.
    /// </summary>
    private void BuildSql()
    {
        if (string.IsNullOrEmpty(_table))
        {
            throw new InvalidOperationException("Table name not specified for CREATE TABLE.");
        }

        if (_columnDefs.Count == 0)
        {
            throw new InvalidOperationException("No columns specified for CREATE TABLE.");
        }

        _ = _sqlBuilder.Clear();
        _ = _sqlBuilder.Append("CREATE TABLE ");
        if (_ifNotExists)
        {
            _ = _sqlBuilder.Append("IF NOT EXISTS ");
        }

        _ = _sqlBuilder.Append($"{_table} (");
        _ = _sqlBuilder.Append(string.Join(", ", _columnDefs));
        _ = _sqlBuilder.Append(");");
    }

    /// <summary>
    /// Gets the SQL text of the query (building it if needed).
    /// </summary>
    public override string Sql
    {
        get
        {
            BuildSql();
            return base.Sql;
        }
    }
}