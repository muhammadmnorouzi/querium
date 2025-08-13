using Arian.Quantiq.Application.Enums;

namespace Arian.Quantiq.Application.DTOs.TableManagement;

/// <summary>
/// A DTO representing a change to a single column.
/// </summary>
public class UpdateColumnDTO
{
    public string OldName { get; set; } = string.Empty; // Required for alter/drop/rename
    public string NewName { get; set; } = string.Empty; // Required for add/rename
    public ColumnDataType? DataType { get; set; }
    public int? Length { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public bool? IsNullable { get; set; }
    public UpdateOperationType Operation { get; set; }
}
