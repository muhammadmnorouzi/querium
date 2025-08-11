using Arian.Querium.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;

/// <summary>
/// Represents a command to create a new table in the database.
/// </summary>
public class CreateTableCommand : IRequest<ApplicationResult<AppVoid>>
{
    /// <summary>
    /// Gets or sets the name of the table to create.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of columns for the table.
    /// </summary>
    public List<ColumnDefinition> Columns { get; set; } = [];

    /// <summary>
    /// Gets or sets the name of the primary key column.
    /// </summary>
    public string PrimaryKeyColumn { get; set; } = string.Empty;
}