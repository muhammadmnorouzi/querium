using MediatR;

namespace Arian.Quantiq.Application.Features.TableManagement.Notifications.TableCreated;

/// <summary>
/// Notification that is published when a new table is created.
/// </summary>
public class TableCreatedNotification : INotification
{
    /// <summary>
    /// Gets or sets the name of the table that was created.
    /// </summary>
    public required string TableName { get; set; }
}