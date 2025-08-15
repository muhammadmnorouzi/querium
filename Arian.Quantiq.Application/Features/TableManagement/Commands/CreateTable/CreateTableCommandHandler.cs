using Arian.Quantiq.Application.Extensions;
using Arian.Quantiq.Application.Features.TableManagement.Notifications.TableCreated;
using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Common.Results;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Net;

namespace Arian.Quantiq.Application.Features.TableManagement.Commands.CreateTable;

/// <summary>
/// Handles the <see cref="CreateTableCommand"/> to create a new table in the database.
/// </summary>
public class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, ApplicationResult<AppVoid>>
{
    private readonly ISQLTableManager tableManager;
    private readonly IValidator<CreateTableCommand> validator;
    private readonly IMediator mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableCommandHandler"/> class.
    /// </summary>
    /// <param name="tableManager">The table manager.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="mediator">The mediator.</param>
    public CreateTableCommandHandler(
        ISQLTableManager tableManager,
        IValidator<CreateTableCommand> validator,
        IMediator mediator)
    {
        this.tableManager = tableManager;
        this.validator = validator;
        this.mediator = mediator;
    }

    /// <summary>
    /// Creates a new table with the specified name, columns, and primary key.
    /// </summary>
    /// <param name="request">The command containing table name, columns, and primary key column.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ApplicationResult{AppVoid}"/> indicating success or failure.</returns>
    public async Task<ApplicationResult<AppVoid>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return (validationResult.ToErrorContainer(), HttpStatusCode.BadRequest);
        }

        try
        {
            ApplicationResult<AppVoid> applicationResult = await tableManager.CreateTable(request.CreateTableDTO, cancellationToken);

            if (applicationResult.IsSuccess)
            {
                await mediator.Publish(new TableCreatedNotification() { TableName = request.CreateTableDTO.TableName }, cancellationToken);
            }

            return applicationResult;
        }

        catch (Exception ex)
        {
            return (new ErrorContainer([ex.Message]), HttpStatusCode.InternalServerError);
        }
    }
}