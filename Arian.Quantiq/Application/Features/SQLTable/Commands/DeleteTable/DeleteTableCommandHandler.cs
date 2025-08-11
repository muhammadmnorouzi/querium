using Arian.Quantiq.Domain.Common.Results;
using Arian.Quantiq.Domain.Common.Results;
using Arian.Querium.SQL.Repositories;
using MediatR;
using System.Net;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.DeleteTable;

/// <summary>
/// Handles the deletion of an existing table.
/// </summary>
public class DeleteTableCommandHandler(IDynamicSQLRepository repository) : IRequestHandler<DeleteTableCommand, ApplicationResult<AppVoid>>
{

    /// <summary>
    /// Deletes the specified table from the database.
    /// </summary>
    /// <param name="request">The command containing the table name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ApplicationResult{AppVoid}"/> indicating success or failure.</returns>
    public async Task<ApplicationResult<AppVoid>> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
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