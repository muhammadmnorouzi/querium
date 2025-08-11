using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.DeleteTable;

/// <summary>
/// Represents a command to delete an existing table.
/// </summary>
public class DeleteTableCommand : IRequest<ApplicationResult<AppVoid>>
{
    /// <summary>
    /// Gets or sets the name of the table to delete.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}
