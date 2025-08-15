namespace Arian.Quantiq.Application.DTOs.TableManagement;

/// <summary>
/// A data transfer object (DTO) representing a complete table update request, including the table name and column updates.
/// </summary>
public class UpdateTableDTO
{
    /// <summary>
    /// Gets or sets the name of the table to update.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of column update operations to apply to the table.
    /// </summary>
    public List<UpdateColumnDTO> ColumnUpdates { get; set; } = [];
}
