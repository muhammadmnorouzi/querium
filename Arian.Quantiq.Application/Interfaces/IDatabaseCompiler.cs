using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.DTOs.TableManagement;
using Arian.Quantiq.Domain.Common.Results;

namespace Arian.Quantiq.Application.Interfaces;

/// <summary>
/// Defines the contract for compiling an abstract table model into database-specific SQL.
/// </summary>
public interface IDatabaseCompiler
{
    /// <summary>
    /// Compiles an abstract table creation model into a database-specific SQL CREATE TABLE statement.
    /// </summary>
    /// <param name="model">The abstract model of the table to create.</param>
    /// <returns>The fully formatted SQL string.</returns>
    Task<ApplicationResult<string>> Compile(CreateTableDTO model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a table update request against the current database schema.
    /// </summary>
    /// <param name="updateDto">The user's requested changes.</param>
    /// <param name="existingColumns">The metadata of the existing columns in the table.</param>
    /// <returns>A string with an error message, or null if validation passes.</returns>
    Task<string> ValidateUpdate(UpdateTableDTO updateDto, List<ColumnMetadata> existingColumns);

    Task<string> Validate(CreateTableDTO model, CancellationToken cancellationToken = default);
    Task<bool> IsValidSqlIdentifier(string sqlIdentifier);
}