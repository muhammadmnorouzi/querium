namespace Arian.Quantiq.Application.DTOs;

/// <summary>
/// Represents the metadata of an existing database column, as retrieved from the database schema.
/// </summary>
public class ColumnMetadata
{
    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the data type of the column as a string (database-agnostic).
    /// </summary>
    public string DataType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the length of the column, if applicable.
    /// </summary>
    public int? Length { get; init; }

    /// <summary>
    /// Gets the precision of the column, if applicable.
    /// </summary>
    public int? Precision { get; init; }

    /// <summary>
    /// Gets the scale of the column, if applicable.
    /// </summary>
    public int? Scale { get; init; }

    /// <summary>
    /// Gets a value indicating whether the column allows null values.
    /// </summary>
    public bool IsNullable { get; init; }

    /// <summary>
    /// Gets a value indicating whether the column is auto-incrementing.
    /// </summary>
    public bool IsAutoIncrementing { get; init; }

    /// <summary>
    /// Gets a value indicating whether the column is a primary key.
    /// </summary>
    public bool IsPrimaryKey { get; init; }
}