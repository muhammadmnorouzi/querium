using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.DTOs.TableManagement;
using Arian.Quantiq.Domain.Common.Results;

namespace Arian.Quantiq.Application.Interfaces;

/// <summary>
/// Defines the contract for managing SQL table creation and modification.
/// </summary>
public interface ISQLTableManager
{
    /// <summary>
    /// Creates a new SQL table based on the provided data transfer object.
    /// </summary>
    /// <param name="input">The data transfer object containing the table's name and column definitions.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous create operation. The task result contains an
    /// <see cref="ApplicationResult{T}"/> indicating the success or failure of the operation.
    /// </returns>
    Task<ApplicationResult<AppVoid>> CreateTable(CreateTableDTO input, CancellationToken cancellationToken = default);
    Task<ApplicationResult<DynamicTableDTO>> InsertDynamicData(DynamicTableDTO dynamicDataDTO, string connectionString, CancellationToken cancellationToken);
}