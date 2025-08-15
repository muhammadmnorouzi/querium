using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.TableManagement.Queries.DownloadEmptyExcel;

/// <summary>
/// Query to download an empty Excel template for a specific table.
/// </summary>
public class DownloadEmptyExcelQuery : IRequest<ApplicationResult<MemoryStream>>
{
    /// <summary>
    /// Gets or sets the name of the table for which to generate the Excel template.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}
