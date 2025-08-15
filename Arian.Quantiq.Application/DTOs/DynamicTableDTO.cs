namespace Arian.Quantiq.Application.DTOs;

public class DynamicTableDTO
{
    public string TableName { get; init; } = string.Empty;
    public Dictionary<string, IList<object?>> Rows { get; init; } = [];

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
