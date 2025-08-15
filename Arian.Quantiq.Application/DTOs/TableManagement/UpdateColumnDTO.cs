using Arian.Quantiq.Application.Enums;

namespace Arian.Quantiq.Application.DTOs.TableManagement;

/// <summary>
/// A DTO representing a change to a single column in a table schema update operation.
/// </summary>
public class UpdateColumnDTO
{
    /// <summary>
    /// Gets or sets the current name of the column. Required for alter, drop, or rename operations.
    /// </summary>
    public string OldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new name of the column. Required for add or rename operations.
    /// </summary>
    public string NewName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new data type for the column, if applicable.
    /// </summary>
    public ColumnDataType? DataType { get; set; }

    /// <summary>
    /// Gets or sets the new length for the column, if applicable.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the new precision for the column, if applicable.
    /// </summary>
    public int? Precision { get; set; }

    /// <summary>
    /// Gets or sets the new scale for the column, if applicable.
    /// </summary>
    public int? Scale { get; set; }

    /// <summary>
    /// Gets or sets whether the column should allow null values after the update.
    /// </summary>
    public bool? IsNullable { get; set; }

    /// <summary>
    /// Gets or sets the type of update operation to perform on the column.
    /// </summary>
    public UpdateOperationType Operation { get; set; }
}
