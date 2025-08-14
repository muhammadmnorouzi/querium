using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Arian.Quantiq.Application.Features.TableManagement.Notifications.TableCreated;

public class TableCreatedNotificationHandler(
    ITableDefinitionRepository tableDefinitionRepository,
    ILogger<TableCreatedNotificationHandler> logger) : INotificationHandler<TableCreatedNotification>
{
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