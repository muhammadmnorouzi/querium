namespace Arian.Quantiq.Application.DTOs.ExcelService;

/// <summary>
/// DTO for requesting an Excel template for a specific table.
/// </summary>
public class GetExcelTemplateDTO
{
    /// <summary>
    /// Gets or sets the name of the table for which to generate the Excel template.
    /// </summary>
    public string TableName { get; set; } = string.Empty;
}
