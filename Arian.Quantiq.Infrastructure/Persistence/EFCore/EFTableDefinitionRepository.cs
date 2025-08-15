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
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
        }

        if (Guid.TryParse(currentUserId, out Guid userId))
        {
            return await dbContext.TableDefinitions
                .FirstOrDefaultAsync(t => t.TableName == tableName && userId == t.CreatedByUserId, cancellationToken);

        }
        else
        {
            throw new ArgumentException("Current user Id is not valid Guid.", nameof(tableName));
        }

    }

    public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}