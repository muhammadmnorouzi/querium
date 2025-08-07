using Arian.Querium.SQL.QueryBuilders;

namespace Arian.Querium.SQLite.Implementations.QueryBuilders;

/// <summary>
/// SQLite implementation of IInsertQueryBuilder.
/// </summary>
public class SqliteInsertQueryBuilder : SqliteQueryBuilderBase, IInsertQueryBuilder
{
    private string _table = string.Empty;
    private readonly List<string> _columns = [];
    private readonly List<object> _values = [];

    /// <summary>
    /// Specifies the table to insert into.
    /// </summary>
    public IInsertQueryBuilder Into(string table)
    {
        _table = table;
        return this;
    }

    /// <summary>
    /// Specifies the columns to insert.
    /// </summary>
    public IInsertQueryBuilder Columns(params string[] columns)
    {
        _columns.AddRange(columns);
        return this;
    }

    /// <summary>
    /// Specifies the values to insert for the previously specified columns.
    /// </summary>
    public IInsertQueryBuilder Values(params object[] values)
    {
        _values.AddRange(values);
        return this;
    }

    /// <summary>
    /// Builds the final SQL string.
    /// </summary>
    private void BuildSql()
    {
        if (string.IsNullOrEmpty(_table))
        {
            throw new InvalidOperationException("INTO table not specified.");
        }

        if (_columns.Count != 0 && _columns.Count != _values.Count)
        {
            throw new InvalidOperationException("Number of columns and values must match.");
        }

        _ = _sqlBuilder.Clear();

        _ = _sqlBuilder.Append($"INSERT INTO {_table}");
        if (_columns.Count > 0)
        {
            _ = _sqlBuilder.Append(" (");
            _ = _sqlBuilder.Append(string.Join(", ", _columns));
            _ = _sqlBuilder.Append(')');
        }
        _ = _sqlBuilder.Append(" VALUES (");

        for (int i = 0; i < _values.Count; i++)
        {
            string paramName = AddParameter(_values[i]);
            _ = _sqlBuilder.Append(paramName);
            if (i < _values.Count - 1)
            {
                _ = _sqlBuilder.Append(", ");
            }
        }
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