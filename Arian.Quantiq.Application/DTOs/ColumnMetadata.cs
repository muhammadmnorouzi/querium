namespace Arian.Quantiq.Application.DTOs;

/// <summary>
/// Represents the metadata of an existing database column.
/// This is populated by querying the database.
/// </summary>
public class ColumnMetadata
{
    public string Name { get; init; } = string.Empty;
    public string DataType { get; init; } = string.Empty; // Stored as a string to be database-agnostic
    public int? Length { get; init; }
    public int? Precision { get; init; }
    public int? Scale { get; init; }
    public bool IsNullable { get; init; }
}