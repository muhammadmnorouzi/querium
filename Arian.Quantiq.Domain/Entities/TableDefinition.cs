namespace Arian.Quantiq.Domain.Entities;

public class TableDefinition : EntityBase
{
    public string TableName { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
}
