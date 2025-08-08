using Microsoft.AspNetCore.Identity;

namespace Arian.Quantiq.Domain.Entities.Identity;

// Redefine ApplicationUser inheriting from IdentityUser and BaseEntity.
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
