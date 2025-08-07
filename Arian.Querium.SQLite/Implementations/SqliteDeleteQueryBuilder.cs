using Arian.Querium.Abstractions.SQL;

namespace Arian.Querium.SQLite.Implementations;

/// <summary>
/// SQLite implementation of IDeleteQueryBuilder.
/// </summary>
public class SqliteDeleteQueryBuilder : SqliteQueryBuilderBase, IDeleteQueryBuilder
{
    private string _table = string.Empty;
    private readonly List<string> _whereConditions = [];

    /// <summary>
    /// Specifies the table to delete from.
    /// </summary>
    public IDeleteQueryBuilder Delete(string table)
    {
        _table = table;
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition (AND) for column {op} value.
    /// </summary>
    public IDeleteQueryBuilder Where(string column, object value, string op = "=")
    {
        string paramName = AddParameter(value);
        string condition = $"{column} {op} {paramName}";
        if (_whereConditions.Count == 0)
        {
            _whereConditions.Add(condition);
        }
        else
        {
            _whereConditions.Add("AND " + condition);
        }

        return this;
    }

    /// <summary>
    /// Adds a WHERE condition (OR) for column {op} value.
    /// </summary>
    public IDeleteQueryBuilder Or(string column, object value, string op = "=")
    {
        string paramName = AddParameter(value);
        string condition = $"{column} {op} {paramName}";
        if (_whereConditions.Count == 0)
        {
            _whereConditions.Add(condition);
        }
        else
        {
            _whereConditions.Add("OR " + condition);
        }

        return this;
    }

    /// <summary>
    /// Builds the final SQL string.
    /// </summary>
    private void BuildSql()
    {
        if (string.IsNullOrEmpty(_table))
        {
            throw new InvalidOperationException("DELETE table not specified.");
        }

        _ = _sqlBuilder.Clear();

        _ = _sqlBuilder.Append($"DELETE FROM {_table}");

        if (_whereConditions.Count > 0)
        {
            _ = _sqlBuilder.AppendLine();
            _ = _sqlBuilder.Append("WHERE ");
            _ = _sqlBuilder.Append(_whereConditions[0]);
            for (int i = 1; i < _whereConditions.Count; i++)
            {
                _ = _sqlBuilder.Append($" {_whereConditions[i]}");
            }
        }
        _ = _sqlBuilder.Append(';');
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