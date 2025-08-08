using Arian.Querium.SQL.QueryBuilders;

namespace Arian.Querium.SQL.Repositories;

/// <summary>
/// An abstraction for a dynamic repository that operates on column names and values.
/// </summary>
public interface IDynamicSqlRepository
{
    Task CreateTableAsync(string tableName, Dictionary<string, ColumnType> columns);
    Task RenameTableAsync(string oldTableName, string newTableName);
    Task DeleteTableAsync(string tableName);

    /// <summary>
    /// Retrieves all rows from a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    Task<IEnumerable<Dictionary<string, object>>> GetAllAsync(string tableName);

    /// <summary>
    /// Retrieves a single row by its primary key.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value.</param>
    Task<Dictionary<string, object>?> GetByIdAsync(string tableName, string primaryKeyColumn, object id);

    /// <summary>
    /// Adds a new row to a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columns">A dictionary of column names and their values.</param>
    Task AddAsync(string tableName, Dictionary<string, object> columns);

    /// <summary>
    /// Updates an existing row in a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columns">A dictionary of column names to update and their new values.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value of the row to update.</param>
    Task UpdateAsync(string tableName, Dictionary<string, object> columns, string primaryKeyColumn, object id);

    /// <summary>
    /// Deletes a row from a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value of the row to delete.</param>
    Task DeleteAsync(string tableName, string primaryKeyColumn, object id);
}
