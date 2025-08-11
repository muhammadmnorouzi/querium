using Arian.Querium.Common.Results;
using MediatR;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.UploadExcel;

/// <summary>
/// Represents a command to upload an Excel file to update the dynamic database.
/// </summary>
public class UploadExcelCommand : IRequest<ApplicationResult<AppVoid>>
{
    /// <summary>
    /// Gets or sets the name of the table to update.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stream of the uploaded Excel file.
    /// </summary>
    public Stream FileStream { get; set; } = null!;
}
