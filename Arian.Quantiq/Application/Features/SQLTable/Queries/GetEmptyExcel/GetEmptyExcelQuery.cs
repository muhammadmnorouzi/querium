using Arian.Quantiq.Domain.Common.Results;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.SQLTable.Queries.GetEmptyExcel;

/// <summary>
/// Represents a query to download an empty Excel file for a selected table.
/// </summary>
public class GetEmptyExcelQuery : IRequest<ApplicationResult<MemoryStream>>
{
    /// <summary>
    /// Gets or sets the name of the table for which to generate the Excel file.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}

/// <summary>
/// Represents a DTO to request to download an empty Excel file for a selected table.
/// </summary>
public class GetEmptyExcelDTO
{
    /// <summary>
    /// Gets or sets the name of the table for which to generate the Excel file.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}