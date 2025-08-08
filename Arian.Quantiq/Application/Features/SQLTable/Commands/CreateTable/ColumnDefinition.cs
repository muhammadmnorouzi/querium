using Arian.Querium.SQL.QueryBuilders;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;

/// <summary>
/// Represents a column definition for a table.
/// </summary>
public class ColumnDefinition
{
    /// <summary>
    /// Gets or sets the name of the column.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data type of the column (e.g., INTEGER, TEXT).
    /// </summary>
    public ColumnType Type { get; set; }
}
