using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Arian.Quantiq.Application.DTOs;

/// <summary>
/// Represents the data required to upload an Excel file for a table.
/// </summary>
public record UploadExcelDTO
{
    /// <summary>
    /// Gets the name of the table to which the Excel data will be uploaded.
    /// </summary>
    [Required]
    public string TableName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Excel file to be uploaded.
    /// </summary>
    [Required]
    public IFormFile ExcelFile { get; set; } = null!;
}
