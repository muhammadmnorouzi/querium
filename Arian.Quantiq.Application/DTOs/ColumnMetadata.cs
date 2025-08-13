namespace Arian.Quantiq.Application.DTOs;

/// <summary>
/// Represents the metadata of an existing database column.
/// This is populated by querying the database.
/// </summary>
public class ColumnMetadata
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty; // Stored as a string to be database-agnostic
    public int? Length { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public bool IsNullable { get; set; }
}