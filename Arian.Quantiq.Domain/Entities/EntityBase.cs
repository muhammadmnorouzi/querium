namespace Arian.Quantiq.Domain.Entities;

public class EntityBase
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;
    public required Guid CreatedByUserId { get; set; }

}