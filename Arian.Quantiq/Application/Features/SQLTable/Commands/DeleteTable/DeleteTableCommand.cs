using Arian.Querium.Common.Results;
using Mediator;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.DeleteTable;

/// <summary>
/// Represents a command to delete an existing table.
/// </summary>
public class DeleteTableCommand : ICommand<ApplicationResult<AppVoid>>
{
    /// <summary>
    /// Gets or sets the name of the table to delete.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}
