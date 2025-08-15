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
    public async Task<IReadOnlyList<ColumnMetadata>> GetTableColumnsAsync(string tableName, string connectionString, CancellationToken cancellationToken = default)
    {
        using SqlConnection dbConnection = new SqlConnection(connectionString);
        List<ColumnMetadata> columns = new();

        string query = @"
            SELECT 
                c.COLUMN_NAME, 
                c.DATA_TYPE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.NUMERIC_PRECISION,
                c.NUMERIC_SCALE,
                c.IS_NULLABLE,
                sc.is_identity,
                CASE WHEN pk.CONSTRAINT_TYPE = 'PRIMARY KEY' THEN 1 ELSE 0 END AS is_primary_key
            FROM 
                INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN sys.columns sc 
                ON OBJECT_ID(c.TABLE_NAME) = sc.object_id 
                AND c.COLUMN_NAME = sc.name
            LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                ON c.TABLE_NAME = kcu.TABLE_NAME
                AND c.COLUMN_NAME = kcu.COLUMN_NAME
            LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk
                ON kcu.CONSTRAINT_NAME = pk.CONSTRAINT_NAME
                AND pk.CONSTRAINT_TYPE = 'PRIMARY KEY'
            WHERE 
                c.TABLE_NAME = @TableName
                AND c.TABLE_SCHEMA = 'dbo';";

        await using (SqlCommand command = new(query, dbConnection))
        {
            command.Parameters.AddWithValue("@TableName", tableName);

            if (dbConnection.State != ConnectionState.Open)
            {
                await dbConnection.OpenAsync(cancellationToken);
            }

            await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                columns.Add(new ColumnMetadata
                {
                    Name = reader["COLUMN_NAME"].ToString()!,
                    DataType = reader["DATA_TYPE"].ToString()!,
                    Length = reader["CHARACTER_MAXIMUM_LENGTH"] as int?,
                    Precision = reader["NUMERIC_PRECISION"] as int?,
                    Scale = reader["NUMERIC_SCALE"] as int?,
                    IsNullable = reader["IS_NULLABLE"].ToString() == "YES",
                    IsAutoIncrementing = reader.GetBoolean(reader.GetOrdinal("is_identity")),
                    IsPrimaryKey = reader.GetInt32(reader.GetOrdinal("is_primary_key")) == 1
                });
            }
        }

        return columns;
    }
}