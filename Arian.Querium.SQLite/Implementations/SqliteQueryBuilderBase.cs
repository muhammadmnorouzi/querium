using Arian.Querium.Abstractions.SQL;
using System.Data;
using System.Text;

namespace Arian.Querium.SQLite.Implementations;

/// <summary>
/// Base class for SQLite query builders, providing common SQL assembly and parameter management.
/// </summary>
public abstract class SqliteQueryBuilderBase : IQuery
{
    protected readonly StringBuilder _sqlBuilder = new();
    protected readonly List<IQueryParameter> _parameters = [];
    private int _paramIndex = 0;
    protected readonly ISqlDialect _dialect = new SqliteDialect();

    /// <summary>
    /// Gets the assembled SQL query text.
    /// </summary>
    public virtual string Sql => _sqlBuilder.ToString().Trim();

    /// <summary>
    /// Gets the list of parameters for this query.
    /// </summary>
    public IEnumerable<IQueryParameter> Parameters => _parameters;

    /// <summary>
    /// Creates an IDbCommand with the current SQL and parameters on the provided connection.
    /// </summary>
    public IDbCommand ToCommand(IDbConnection connection)
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = Sql;
        foreach (IQueryParameter param in _parameters)
        {
            IDbDataParameter dbParam = cmd.CreateParameter();
            dbParam.ParameterName = param.Name;
            dbParam.Value = param.Value ?? DBNull.Value;
            _ = cmd.Parameters.Add(dbParam);
        }
        return cmd;
    }

    /// <summary>
    /// Adds a new parameter for the given value and returns its generated name.
    /// </summary>
    protected string AddParameter(object value)
    {
        string paramName = $"@p{_paramIndex++}";
        _parameters.Add(new QueryParameter(paramName, value ?? DBNull.Value));
        return paramName;
    }
}


