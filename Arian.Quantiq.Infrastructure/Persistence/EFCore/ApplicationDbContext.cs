using Arian.Quantiq.Domain.Entities;
using Arian.Quantiq.Domain.Entities.Identity;
using Arian.Quantiq.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Arian.Quantiq.Infrastructure.Persistence.EFCore;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await UpdateAuditingFieldsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateAuditingFieldsAsync(CancellationToken cancellationToken)
    {
        var entries = ChangeTracker.Entries<EntityBase>()
            .Where(e => e.State == EntityState.Added);

        string currentUserId = await serviceProvider.GetRequiredService<IUserContextService>().GetUserIdAsync();

        Guid? userId = Guid.TryParse(currentUserId, out Guid guidUserId)
            ? guidUserId
            : null;

        foreach (var entry in entries)
        {
            entry.Entity.CreatedByUserId = guidUserId;
        }
    }

    public virtual DbSet<TableDefinition> TableDefinitions { get; protected set; }
}