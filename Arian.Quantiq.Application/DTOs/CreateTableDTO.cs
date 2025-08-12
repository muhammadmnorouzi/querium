namespace Arian.Quantiq.Application.DTOs;

/// <summary>
/// Represents the complete abstract structure for a new table to be created.
/// </summary>
public class CreateTableDTO
{
    /// <summary>
    /// Gets or sets the name of the table to be created.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a list of abstract column definitions for the new table.
    /// </summary>
    public List<CreateColumnDTO> Columns { get; set; } = [];
}
