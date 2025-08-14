using Arian.Quantiq.Domain.Entities.Identity;

namespace Arian.Quantiq.Domain.Entities;

public class EntityBase
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
}