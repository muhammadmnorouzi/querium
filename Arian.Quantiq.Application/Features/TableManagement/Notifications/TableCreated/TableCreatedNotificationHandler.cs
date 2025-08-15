using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Arian.Quantiq.Application.Features.TableManagement.Notifications.TableCreated;

/// <summary>
/// Handles the <see cref="TableCreatedNotification"/> to persist the table definition and log errors if saving fails.
/// </summary>
public class TableCreatedNotificationHandler : INotificationHandler<TableCreatedNotification>
{
    private readonly ITableDefinitionRepository tableDefinitionRepository;
    private readonly ILogger<TableCreatedNotificationHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCreatedNotificationHandler"/> class.
    /// </summary>
    /// <param name="tableDefinitionRepository">The repository to manage table definitions.</param>
    /// <param name="logger">The logger instance for logging errors.</param>
    public TableCreatedNotificationHandler(
        ITableDefinitionRepository tableDefinitionRepository,
        ILogger<TableCreatedNotificationHandler> logger)
    {
        this.tableDefinitionRepository = tableDefinitionRepository;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(TableCreatedNotification notification, CancellationToken cancellationToken)
    {
        TableDefinition tableDefinitionToAdd = new()
        {
            TableName = notification.TableName,
        };

        await tableDefinitionRepository.Add(tableDefinitionToAdd, cancellationToken);
        bool changesSaved = await tableDefinitionRepository.SaveChanges(cancellationToken);

        if (!changesSaved)
        {
            logger.LogError("Failed to save changes for table definition: {TableName}", notification.TableName);
        }
    }
}