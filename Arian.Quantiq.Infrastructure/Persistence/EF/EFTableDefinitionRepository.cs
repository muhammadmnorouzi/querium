using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Arian.Quantiq.Infrastructure.Persistence.EF;

public class EFTableDefinitionRepository(ApplicationDbContext dbContext) : ITableDefinitionRepository
{
    public async Task<TableDefinition> Add(TableDefinition tableDefinitionToAdd, CancellationToken cancellationToken = default)
    {
        EntityEntry<TableDefinition> entityEntry = await dbContext.AddAsync(tableDefinitionToAdd, cancellationToken);

        return entityEntry.Entity;
    }

    public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}