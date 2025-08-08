namespace Arian.Quantiq.Domain.Entities;

public class TableDefinition
{
    public int Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
