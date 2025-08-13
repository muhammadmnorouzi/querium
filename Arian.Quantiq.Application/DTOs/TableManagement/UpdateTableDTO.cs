namespace Arian.Quantiq.Application.DTOs.TableManagement;

/// <summary>
/// A data transfer object (DTO) representing a complete table update request.
/// </summary>
public class UpdateTableDTO
{
    public string TableName { get; set; } = string.Empty;
    public List<UpdateColumnDTO> ColumnUpdates { get; set; } = [];
}
