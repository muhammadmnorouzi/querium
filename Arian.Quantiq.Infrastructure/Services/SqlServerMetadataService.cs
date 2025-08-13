using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Arian.Quantiq.Infrastructure.Services;

/// <summary>
/// An implementation of <see cref="ITableMetadataService"/> for SQL Server.
/// This would query the INFORMATION_SCHEMA.COLUMNS view.
/// </summary>
public class SqlServerMetadataService() : ITableMetadataService
{
    /// <summary>
    /// Gets the metadata for all columns in a specified table by querying the database.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation.</param>
    /// <returns>A list of <see cref="ColumnMetadata"/> objects.</returns>
    public async Task<List<ColumnMetadata>> GetTableColumnsAsync(string tableName, string connectionString, CancellationToken cancellationToken = default)
    {
        IDbConnection dbConnection = new SqlConnection(connectionString);

        List<ColumnMetadata> columns = [];

        string query = @"
            SELECT 
                COLUMN_NAME, 
                DATA_TYPE,
                CHARACTER_MAXIMUM_LENGTH,
                NUMERIC_PRECISION,
                NUMERIC_SCALE,
                IS_NULLABLE
            FROM 
                INFORMATION_SCHEMA.COLUMNS
            WHERE 
                TABLE_NAME = @TableName;";

        await using (SqlCommand command = (SqlCommand)dbConnection.CreateCommand())
        {
            command.CommandText = query;
            command.Parameters.AddWithValue("@TableName", tableName);

            // Ensure the connection is open if it's not already.
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                columns.Add(new ColumnMetadata
                {
                    Name = reader["COLUMN_NAME"].ToString()!,
                    DataType = reader["DATA_TYPE"].ToString()!,
                    // These are nullable in the database, so we check for DBNull.
                    Length = reader["CHARACTER_MAXIMUM_LENGTH"] as int?,
                    Precision = reader["NUMERIC_PRECISION"] as int?,
                    Scale = reader["NUMERIC_SCALE"] as int?,
                    IsNullable = reader["IS_NULLABLE"].ToString() == "YES"
                });
            }
        }

        return columns;
    }
}