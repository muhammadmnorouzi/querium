using Arian.Querium.SQL.QueryBuilders;

namespace Arian.Querium.SQL.Repositories;

/// <summary>
/// An abstraction for performing data operations on dynamic SQL tables.
/// </summary>
public interface IDynamicSQLRepository
{
    /// <summary>
    /// Retrieves all rows from a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A task that returns a collection of dictionaries, each representing a row with column names and values.</returns>
    Task<IEnumerable<Dictionary<string, object>>> GetAllAsync(string tableName);

    /// <summary>
    /// Retrieves a single row by its primary key.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value.</param>
    /// <returns>A task that returns a dictionary representing the row, or null if not found.</returns>
    Task<Dictionary<string, object>?> GetByIdAsync(string tableName, string primaryKeyColumn, object id);

    /// <summary>
    /// Adds a new row to a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columns">A dictionary of column names and their values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(string tableName, Dictionary<string, object> columns);

    /// <summary>
    /// Updates an existing row in a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columns">A dictionary of column names to update and their new values.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value of the row to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(string tableName, Dictionary<string, object> columns, string primaryKeyColumn, object id);

    /// <summary>
    /// Deletes a row from a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKeyColumn">The name of the primary key column.</param>
    /// <param name="id">The primary key value of the row to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(string tableName, string primaryKeyColumn, object id);

    /// <summary>
    /// Inserts or updates multiple rows in a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="rows">A list of dictionaries, each representing a row with column names and values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpsertRowAsync(string tableName, Dictionary<string, object> row, string pkColumn);

    /// <summary>
    /// Renames an existing table.
    /// </summary>
    Task RenameTableAsync(string oldTableName, string newTableName);

    /// <summary>
    /// Deletes a table from the database.
    /// </summary>
    Task DeleteTableAsync(string tableName);

    /// <summary>
    /// Retrieves rows from a table with optional column selection, conditions, and ordering.
    /// </summary>
    Task<IEnumerable<Dictionary<string, object>>> GetAsync(
        string tableName,
        string[]? columns = null,
        (string Column, object Value, string Operator)?[]? conditions = null,
        (string Column, SortOrder Order)?[]? orderBy = null);

    /// <summary>
    /// Updates an existing row in a specified table based on conditions.
    /// </summary>
    Task UpdateAsync(
        string tableName,
        Dictionary<string, object> columns,
        (string Column, object Value, string Operator)?[]? conditions = null);

    /// <summary>
    /// Deletes rows from a specified table based on conditions.
    /// </summary>
    Task DeleteAsync(
        string tableName,
        (string Column, object Value, string Operator)?[]? conditions = null);

    /// <summary>
    /// Retrieves the column names and their data types for a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>A task that returns a dictionary where the key is the column name and the value is the column type.</returns>
    Task<Dictionary<string, ColumnType>> GetTableColumnsAsync(string tableName);
    Task CreateTableAsync(string tableName, Dictionary<string, ColumnType> columns, string primaryKeyColumn);
}