using Arian.Querium.Common.Results;
using Arian.Querium.SQL.Repositories;
using Mediator;
using System.Net;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.DeleteTable;

/// <summary>
/// Handles the deletion of an existing table.
/// </summary>
public class DeleteTableCommandHandler(IDynamicSQLRepository repository) : ICommandHandler<DeleteTableCommand, ApplicationResult<AppVoid>>
{

    /// <summary>
    /// Deletes the specified table from the database.
    /// </summary>
    /// <param name="request">The command containing the table name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ApplicationResult{AppVoid}"/> indicating success or failure.</returns>
    public async ValueTask<ApplicationResult<AppVoid>> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await repository.DeleteTableAsync(request.TableName);
            return new ApplicationResult<AppVoid>(AppVoid.Instance, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            ErrorContainer error = new(ex.Message);
            return new ApplicationResult<AppVoid>(error, HttpStatusCode.InternalServerError);
        }
    }
}