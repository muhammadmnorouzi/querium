using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.TableManagement.Commands.UploadExcel;

public class UploadExcelCommand : IRequest<ApplicationResult<DynamicTableDTO>>
{
    public string TableName { get; set; } = string.Empty;
    public MemoryStream ExcelFileStream { get; set; } = null!;
}
