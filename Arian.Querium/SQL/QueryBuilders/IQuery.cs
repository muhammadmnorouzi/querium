using System.Data;

namespace Arian.Querium.SQL.QueryBuilders;

/// <summary>
/// Represents a SQL query with its text and parameters, ready for execution.
/// </summary>
public interface IQuery
{
    /// <summary>
    /// Gets the SQL text of the query.
    /// </summary>
    string Sql { get; }

    /// <summary>
    /// Gets the parameters (name and value) for the query.
    /// </summary>
    IEnumerable<IQueryParameter> Parameters { get; }

    /// <summary>
    /// Creates and returns an <see cref="IDbCommand"/> for this query on the given connection, 
    /// with parameters applied.
    /// </summary>
    IDbCommand ToCommand(IDbConnection connection);
}