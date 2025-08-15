namespace Arian.Quantiq.Application.DTOs;

/// <summary>
/// Represents a dynamic table with its name and row data.
/// </summary>
public class DynamicTableDTO
{
    /// <summary>
    /// Gets the name of the dynamic table.
    /// </summary>
    public string TableName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the rows of the table, where each key is a column name and the value is a list of cell values for that column.
    /// </summary>
    public Dictionary<string, IList<object?>> Rows { get; init; } = [];

    /// <summary>
    /// Validates that all columns have the same number of rows.
    /// </summary>
    /// <returns>True if all columns have the same number of rows; otherwise, false.</returns>
    public bool ValidateRowSize()
    {
        if (Rows.Count == 0)
        {
            return false;
        }
        int rowSize = Rows.First().Value.Count;

        return Rows.All(column => column.Value.Count == rowSize);
    }
}
