using Arian.Querium.Abstractions.SQL;

namespace Arian.Querium.SQLite.Implementations;

/// <summary>
/// SQLite implementation of IUpdateQueryBuilder.
/// </summary>
public class SqliteUpdateQueryBuilder : SqliteQueryBuilderBase, IUpdateQueryBuilder
{
    private string _table = string.Empty;
    private readonly List<string> _setClauses = [];
    private readonly List<string> _whereConditions = [];

    /// <summary>
    /// Specifies the table to update.
    /// </summary>
    public IUpdateQueryBuilder Update(string table)
    {
        _table = table;
        return this;
    }

    /// <summary>
    /// Adds a SET clause assignment.
    /// </summary>
    public IUpdateQueryBuilder Set(string column, object value)
    {
        string paramName = AddParameter(value);
        _setClauses.Add($"{column} = {paramName}");
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition (AND) for column {op} value.
    /// </summary>
    public IUpdateQueryBuilder Where(string column, object value, string op = "=")
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
    public IUpdateQueryBuilder Or(string column, object value, string op = "=")
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
            throw new InvalidOperationException("UPDATE table not specified.");
        }

        if (_setClauses.Count == 0)
        {
            throw new InvalidOperationException("No SET clauses specified.");
        }

        _ = _sqlBuilder.Clear();

        _ = _sqlBuilder.AppendLine($"UPDATE {_table}");
        _ = _sqlBuilder.Append("SET ");
        _ = _sqlBuilder.AppendLine(string.Join(", ", _setClauses));

        if (_whereConditions.Count > 0)
        {
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