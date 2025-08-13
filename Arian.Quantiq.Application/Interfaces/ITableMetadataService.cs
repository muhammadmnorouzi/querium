using Arian.Quantiq.Application.DTOs;
using System.Data.Common;

namespace Arian.Quantiq.Application.Interfaces;

/// <summary>
/// Defines the contract for a service that retrieves database metadata.
/// </summary>
public interface ITableMetadataService
{
    /// <summary>
    /// Gets the metadata for all columns in a specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation.</param>
    /// <returns>A list of <see cref="ColumnMetadata"/> objects.</returns>
    Task<List<ColumnMetadata>> GetTableColumnsAsync(string tableName, CancellationToken cancellationToken = default);
}
