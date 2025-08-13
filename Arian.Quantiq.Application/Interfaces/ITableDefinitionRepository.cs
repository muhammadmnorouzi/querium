using Arian.Quantiq.Domain.Entities;

namespace Arian.Quantiq.Application.Interfaces;

public interface ITableDefinitionRepository
{
    Task<TableDefinition> Add(TableDefinition tableDefinitionToAdd, CancellationToken cancellationToken = default);
    Task<TableDefinition?> Get(string tableName, string currentUserId, CancellationToken cancellationToken = default);
    Task<bool> SaveChanges(CancellationToken cancellationToken = default);
}
