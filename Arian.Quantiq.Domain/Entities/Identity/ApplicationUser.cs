using Microsoft.AspNetCore.Identity;

namespace Arian.Quantiq.Domain.Entities.Identity;

// Redefine ApplicationUser inheriting from IdentityUser and BaseEntity.
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's connection string for database access.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
