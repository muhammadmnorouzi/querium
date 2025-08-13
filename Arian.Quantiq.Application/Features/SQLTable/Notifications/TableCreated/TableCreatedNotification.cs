using MediatR;

namespace Arian.Quantiq.Application.Features.SQLTable.Notifications.TableCreated;

public class TableCreatedNotification : INotification
{
    public required string TableName { get; set; }
}