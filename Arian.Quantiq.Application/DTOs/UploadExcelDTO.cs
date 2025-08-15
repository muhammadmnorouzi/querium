using System.ComponentModel.DataAnnotations;

namespace Arian.Quantiq.Application.DTOs;

public partial class TableManagementController
{
    public record UploadExcelDTO
    {
        [Required]
        public string TableName { get; init; } = string.Empty;

        [Required]
        public IFormFile ExcelFile { get; set; } = null!;
    }
}