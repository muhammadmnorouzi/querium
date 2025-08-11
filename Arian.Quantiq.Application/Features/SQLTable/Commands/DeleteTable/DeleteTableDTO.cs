namespace Arian.Quantiq.Application.Features.SQLTable.Commands.DeleteTable;

/// <summary>
/// Represents a DTO to request to delete an existing table.
/// </summary>
public class DeleteTableDTO
{
    /// <summary>
    /// Gets or sets the name of the table to delete.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}
