using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Arian.Quantiq.Infrastructure.Persistence.EFCore;

public class EFTableDefinitionRepository(ApplicationDbContext dbContext) : ITableDefinitionRepository
{
    public async Task<TableDefinition> Add(TableDefinition tableDefinitionToAdd, CancellationToken cancellationToken = default)
    {
        EntityEntry<TableDefinition> entityEntry = await dbContext.AddAsync(tableDefinitionToAdd, cancellationToken);

        return entityEntry.Entity;
    }

    public async Task<TableDefinition?> Get(string tableName, string currentUserId, CancellationToken cancellationToken = default)
    {
        TableDefinition? tableDefinition = await dbContext.TableDefinitions.Where(
            item => item.TableName == tableName &&
            item.CreatedByUserId == Guid.Parse(currentUserId)).FirstOrDefaultAsync(cancellationToken);

        return tableDefinition;
    }

    public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}