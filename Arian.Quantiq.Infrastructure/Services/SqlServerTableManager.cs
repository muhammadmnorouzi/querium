using Arian.Quantiq.Application.DTOs;
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
    IUserContextService userContextService) : ISQLTableManager
{
    private readonly IDatabaseCompiler databaseCompiler = databaseCompiler;
    private readonly IUserContextService userContextService = userContextService;

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
        string userConnectionString = await userContextService.GetUserConnectionString();
        string tableCreationQuery = result.Data!;


        // Step 3: Execute the SQL query against the database.
        try
        {
            await using (var connection = new SqlConnection(userConnectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await using var command = new SqlCommand(tableCreationQuery, connection);
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
}
