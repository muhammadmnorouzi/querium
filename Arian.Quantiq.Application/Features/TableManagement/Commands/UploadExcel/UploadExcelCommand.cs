using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.TableManagement.Commands.UploadExcel;

/// <summary>
/// Command to upload an Excel file and import its data into a dynamic table.
/// </summary>
public class UploadExcelCommand : IRequest<ApplicationResult<DynamicTableDTO>>
{
    /// <summary>
    /// Gets or sets the name of the table to import data into.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Excel file stream containing the data to import.
    /// </summary>
    public MemoryStream ExcelFileStream { get; set; } = null!;
}
