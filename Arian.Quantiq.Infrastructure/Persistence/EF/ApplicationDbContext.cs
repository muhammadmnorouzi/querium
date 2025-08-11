using Arian.Quantiq.Domain.Entities;
using Arian.Quantiq.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Arian.Quantiq.Infrastructure.Persistence.EF;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser , IdentityRole<Guid>, Guid>(options)
{

    public virtual DbSet<TableDefinition> TableDefinitions { get; protected set; }
}