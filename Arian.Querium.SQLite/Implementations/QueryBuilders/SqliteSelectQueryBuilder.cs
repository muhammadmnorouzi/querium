using Arian.Querium.SQL.QueryBuilders;

namespace Arian.Querium.SQLite.Implementations.QueryBuilders;

/// <summary>
/// SQLite implementation of ISelectQueryBuilder.
/// </summary>
public class SqliteSelectQueryBuilder : SqliteQueryBuilderBase, ISelectQueryBuilder
{
    private readonly List<string> _columns = [];
    private string _fromTable = string.Empty;
    private readonly List<string> _whereConditions = [];
    private readonly List<string> _orderByColumns = [];

    /// <summary>
    /// Specifies the columns to select. If not called, defaults to '*'.
    /// </summary>
    public ISelectQueryBuilder Select(params string[] columns)
    {
        _columns.AddRange(columns);
        return this;
    }

    /// <summary>
    /// Specifies the table to select from.
    /// </summary>
    public ISelectQueryBuilder From(string table)
    {
        _fromTable = table;
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition (AND) for column {op} value.
    /// </summary>
    public ISelectQueryBuilder Where(string column, object value, string op = "=")
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
    public ISelectQueryBuilder Or(string column, object value, string op = "=")
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
    /// Adds an ORDER BY clause for the given column and sort order.
    /// </summary>
    public ISelectQueryBuilder OrderBy(string column, SortOrder sortOrder = SortOrder.Ascending)
    {
        string order = sortOrder == SortOrder.Ascending ? "ASC" : "DESC";
        _orderByColumns.Add($"{column} {order}");
        return this;
    }

    /// <summary>
    /// Builds the final SQL string with clauses on separate lines.
    /// </summary>
    private void BuildSql()
    {
        bool newLineAdded;

        _ = _sqlBuilder.Clear();
        // SELECT clause
        if (_columns.Count > 0)
        {
            _ = _sqlBuilder.Append("SELECT ");
            _ = _sqlBuilder.Append(string.Join(", ", _columns));
        }
        else
        {
            _ = _sqlBuilder.Append("SELECT *");
        }
        _ = _sqlBuilder.AppendLine();

        // FROM clause
        if (string.IsNullOrEmpty(_fromTable))
        {
            throw new InvalidOperationException("FROM table not specified.");
        }

        _ = _sqlBuilder.Append($"FROM {_fromTable}");

        // WHERE clause
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

        // ORDER BY clause
        if (_orderByColumns.Count > 0)
        {
            _ = _sqlBuilder.AppendLine();
            _ = _sqlBuilder.Append("ORDER BY ");
            for (int i = 0; i < _orderByColumns.Count; i++)
            {
                string suffix = i < _orderByColumns.Count - 1 ? "," : "";
                _ = _sqlBuilder.Append($"{_orderByColumns[i]}{suffix}");
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