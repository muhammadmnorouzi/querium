using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQL.Repositories;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Arian.Querium.SQLite.Implementations.Repositories;

/// <summary>
/// A dynamic repository implementation for SQLite databases.
/// </summary>
public class SqliteDynamicRepository(
    string connectionString,
    IQueryBuilderFactory queryBuilderFactory) : IDynamicSqlRepository
{
    private readonly string _connectionString = connectionString;
    private readonly IQueryBuilderFactory _queryBuilderFactory = queryBuilderFactory;

    /// <summary>
    /// Executes a query and returns the results as a collection of dictionaries.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    private async Task<IEnumerable<Dictionary<string, object>>> ExecuteQueryAsync(IQuery query)
    {
        var results = new List<Dictionary<string, object>>();
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using IDbCommand cmd = query.ToCommand(connection);
        using IDataReader reader = await ((SqliteCommand)cmd).ExecuteReaderAsync();

        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            results.Add(row);
        }
        return results;
    }

    /// <summary>
    /// Executes a non-query command (INSERT, UPDATE, DELETE).
    /// </summary>
    /// <param name="query">The query to execute.</param>
    private async Task ExecuteNonQueryAsync(IQuery query)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using IDbCommand cmd = query.ToCommand(connection);
        _ = await ((SqliteCommand)cmd).ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Retrieves all rows from a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    public async Task<IEnumerable<Dictionary<string, object>>> GetAllAsync(string tableName)
    {
        ISelectQueryBuilder queryBuilder = _queryBuilderFactory.Select().From(tableName);
        return await ExecuteQueryAsync(queryBuilder);
    }

    /// <summary>
    /// Retrieves a single row by its primary key.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value.</param>
    public async Task<Dictionary<string, object>?> GetByIdAsync(string tableName, string primaryKeyColumn, object id)
    {
        ISelectQueryBuilder queryBuilder = _queryBuilderFactory.Select()
            .From(tableName)
            .Where(primaryKeyColumn, id);

        return (await ExecuteQueryAsync(queryBuilder)).FirstOrDefault();
    }

    /// <summary>
    /// Adds a new row to a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columns">A dictionary of column names and their values.</param>
    public async Task AddAsync(string tableName, Dictionary<string, object> columns)
    {
        IInsertQueryBuilder queryBuilder = _queryBuilderFactory.Insert()
            .Into(tableName)
            .Columns(columns.Keys.ToArray())
            .Values(columns.Values.ToArray());

        await ExecuteNonQueryAsync(queryBuilder);
    }

    /// <summary>
    /// Updates an existing row in a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columns">A dictionary of column names to update and their new values.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value of the row to update.</param>
    public async Task UpdateAsync(string tableName, Dictionary<string, object> columns, string primaryKeyColumn, object id)
    {
        IUpdateQueryBuilder queryBuilder = _queryBuilderFactory.Update().Update(tableName);

        foreach (var column in columns)
        {
            queryBuilder.Set(column.Key, column.Value);
        }

        queryBuilder.Where(primaryKeyColumn, id);

        await ExecuteNonQueryAsync(queryBuilder);
    }

    /// <summary>
    /// Deletes a row from a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value of the row to delete.</param>
    public async Task DeleteAsync(string tableName, string primaryKeyColumn, object id)
    {
        IDeleteQueryBuilder queryBuilder = _queryBuilderFactory.Delete()
            .Delete(tableName)
            .Where(primaryKeyColumn, id);

        await ExecuteNonQueryAsync(queryBuilder);
    }
}
