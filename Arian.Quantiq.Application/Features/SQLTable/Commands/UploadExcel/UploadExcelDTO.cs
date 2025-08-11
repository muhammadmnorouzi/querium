namespace Arian.Quantiq.Application.Features.SQLTable.Commands.UploadExcel;

/// <summary>
/// Represents a DTO to upload an Excel file to update the dynamic database.
/// </summary>
public class UploadExcelDTO
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