using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.TableManagement.Queries.DownloadEmptyExcel;

public class DownloadEmptyExcelQuery : IRequest<ApplicationResult<MemoryStream>>
{
    public string TableName { get; set; } = string.Empty;
}
