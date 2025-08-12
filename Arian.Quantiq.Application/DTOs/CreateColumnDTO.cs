using Arian.Quantiq.Application.Enums;

namespace Arian.Quantiq.Application.DTOs;

/// <summary>
/// Represents a single column definition using abstract data types and properties.
/// This model can be used to generate create table statements for any supported database.
/// </summary>
public class CreateColumnDTO
{
    /// <summary>
    /// Gets or sets the name of the column.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the abstract data type for the column.
    /// </summary>
    public ColumnDataType DataType { get; set; }

    /// <summary>
    /// Gets or sets the optional length for string types.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the optional precision for decimal types.
    /// </summary>
    public int? Precision { get; set; }

    /// <summary>
    /// Gets or sets the optional scale for decimal types.
    /// </summary>
    public int? Scale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column is a primary key.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column is an auto-incrementing identity column.
    /// </summary>
    public bool IsAutoIncrementing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column can contain null values.
    /// </summary>
    public bool IsNullable { get; set; } = true;
}
