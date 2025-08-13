using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Entities;
using Arian.Quantiq.Domain.Interfaces;
using MediatR;

namespace Arian.Quantiq.Application.Features.SQLTable.Notifications.TableCreated;

public class TableCreatedNotificationHandler(
    IUserContextService userContextService,
    ITableDefinitionRepository tableDefinitionRepository) : INotificationHandler<TableCreatedNotification>
{
    public async Task Handle(TableCreatedNotification notification, CancellationToken cancellationToken)
    {
        string userId = await userContextService.GetUserIdAsync();
        Guid userIdGuid = Guid.Parse(userId);

        TableDefinition tableDefinitionToAdd = new()
        {
            TableName = notification.TableName,
            CreatedByUserId = userIdGuid,
        };

        await tableDefinitionRepository.Add(tableDefinitionToAdd, cancellationToken);
        bool changesSaved = await tableDefinitionRepository.SaveChanges(cancellationToken);
    }
}