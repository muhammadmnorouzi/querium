using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.DTOs.TableManagement;
using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Common.Results;
using Arian.Quantiq.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Net;

namespace Arian.Quantiq.Infrastructure.Services;

/// <summary>
/// An implementation of <see cref="ISQLTableManager"/> that creates SQL tables using a
/// <see cref="TableCreationService"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqlServerTableManager"/> class.
/// </remarks>
/// <param name="tableCreationService">The service used to execute the actual table creation logic.</param>
public class SqlServerTableManager(
    IDatabaseCompiler databaseCompiler,
    IUserContextService userContextService,
    ITableMetadataService tableMetadataService) : ISQLTableManager
{
    private readonly IDatabaseCompiler databaseCompiler = databaseCompiler;
    private readonly IUserContextService userContextService = userContextService;
    private readonly ITableMetadataService tableMetadataService = tableMetadataService;

    /// <summary>
    /// Creates a new SQL table based on the provided DTO.
    /// </summary>
    /// <param name="input">The DTO defining the table structure.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous create operation. The task result contains an
    /// <see cref="ApplicationResult{T}"/> indicating the success or failure of the operation.
    /// </returns>
    public async Task<ApplicationResult<AppVoid>> CreateTable(
        CreateTableDTO input,
        CancellationToken cancellationToken = default)
    {
        // Step 1: Compile the DTO into a database-specific SQL query.
        ApplicationResult<string> result = await databaseCompiler.Compile(input, cancellationToken);

        if (result.IsFailure)
        {
            return (result.Error!, result.HttpStatusCode);
        }

        // Step 2: Get the user-specific connection string.
        string? userConnectionString = await userContextService.GetUserConnectionString();
        string tableCreationQuery = result.Data!;

        // Step 3: Execute the SQL query against the database.
        try
        {
            await using (SqlConnection connection = new(userConnectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using SqlCommand command = new(tableCreationQuery, connection);
                // Ensure the command timeout is sufficient for a schema change.
                command.CommandTimeout = 60;
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            return (AppVoid.Instance, HttpStatusCode.OK);
        }
        catch (OperationCanceledException)
        {
            return (new ErrorContainer(["Table creation operation was canceled."]), HttpStatusCode.RequestTimeout);
        }
        catch (SqlException ex)
        {
            return (new ErrorContainer([$"A database error occurred: {ex.Message}"]), HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            return (new ErrorContainer([$"An unexpected error occurred during table creation: {ex.Message}"]), HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ApplicationResult<DynamicTableDTO>> InsertDynamicData(
         DynamicTableDTO dynamicDataDTO,
         string connectionString,
         CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve column metadata
            IReadOnlyList<ColumnMetadata> columns = await tableMetadataService.GetTableColumnsAsync(dynamicDataDTO.TableName, connectionString, cancellationToken);

            // Query database to identify identity columns
            List<string> identityColumns = [];
            using (SqlConnection connection1 = new(connectionString))
            {
                await connection1.OpenAsync(cancellationToken);
                string query = @"
                    SELECT c.name
                    FROM sys.columns c
                    INNER JOIN sys.tables t ON c.object_id = t.object_id
                    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                    WHERE t.name = @tableName AND s.name = 'dbo' AND c.is_identity = 1";
                using (SqlCommand command = new(query, connection1))
                {
                    command.Parameters.AddWithValue("@tableName", dynamicDataDTO.TableName);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            identityColumns.Add(reader.GetString(0));
                        }
                    }
                }
            }

            // Build parameterized INSERT query, excluding identity columns
            List<string> columnNames = dynamicDataDTO.Rows.Keys
                .Where(c => !identityColumns.Any(ic => ic.Equals(c, StringComparison.OrdinalIgnoreCase)))
                .Select(c => $"[{c}]")
                .ToList();

            if (!columnNames.Any())
            {
                return (new ErrorContainer(["No valid non-identity columns provided for insertion."]), HttpStatusCode.BadRequest);
            }

            string insertQuery = $"INSERT INTO [{dynamicDataDTO.TableName}] ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", columnNames.Select((_, i) => $"@p{i}"))})";

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync(cancellationToken);

            foreach (object?[] row in ZipRows(dynamicDataDTO.Rows, columnNames))
            {
                using SqlCommand command = new(insertQuery, connection);
                for (int i = 0; i < columnNames.Count; i++)
                {
                    command.Parameters.AddWithValue($"@p{i}", row[i] ?? DBNull.Value);
                }

                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            return (dynamicDataDTO, HttpStatusCode.OK);
        }
        catch (SqlException ex)
        {
            return (new ErrorContainer([$"Database error: {ex.Message}"]), HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            return (new ErrorContainer([$"An error occurred while inserting data: {ex.Message}"]), HttpStatusCode.BadRequest);
        }
    }

    private static IEnumerable<object?[]> ZipRows(Dictionary<string, IList<object?>> rows, List<string> columnNames)
    {
        int rowCount = rows.Any() ? rows.First().Value.Count : 0;

        for (int i = 0; i < rowCount; i++)
        {
            object?[] row = new object?[columnNames.Count];
            for (int j = 0; j < columnNames.Count; j++)
            {
                string columnName = columnNames[j].Trim('[', ']');
                row[j] = rows.ContainsKey(columnName) ? rows[columnName][i] : DBNull.Value;
            }
            yield return row;
        }
    }
}