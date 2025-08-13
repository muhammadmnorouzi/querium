using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Enums;
using Arian.Quantiq.Application.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Arian.Quantiq.Infrastructure.Services;


/// <summary>
/// Service for generating Excel documents with structured data validation.
/// This implementation uses the EPPlus library.
/// </summary>
public class ExcelService : IExcelService
{
    public ExcelService()
    {
        // Set EPPlus license context (required for both commercial and non-commercial use)
#warning this should be fixed in case of production use
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Adjust based on your license
    }

    /// <summary>
    /// Generates a structured Excel template based on a list of column metadata.
    /// The template includes locked column headers and data validation for data types.
    /// </summary>
    /// <param name="columns">A list of <see cref="ColumnMetadata"/> objects defining the columns.</param>
    /// <returns>A MemoryStream containing the Excel workbook file.</returns>
    public MemoryStream GenerateExcelTemplate(List<ColumnMetadata> columns)
    {
        using ExcelPackage package = new();
        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Data Input");

        // Define the header row
        for (int i = 0; i < columns.Count; i++)
        {
            ColumnMetadata column = columns[i];
            ExcelRange cell = worksheet.Cells[1, i + 1];
            cell.Value = column.Name;
            cell.Style.Font.Bold = true;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            cell.Style.Locked = true; // Use Locked property directly
        }

        // Apply data validation and formatting to data cells
        for (int i = 0; i < columns.Count; i++)
        {
            ColumnMetadata column = columns[i];
            ExcelColumn col = worksheet.Column(i + 1);
            col.Style.Locked = false; // Unlock data cells

            // Parse data type
            if (Enum.TryParse<ColumnDataType>(column.DataType, true, out ColumnDataType dataType))
            {
                // Define data validation range (rows 2 to 1048576, Excel's max rows)
                ExcelRange dataRange = worksheet.Cells[2, i + 1, 1048576, i + 1];

                switch (dataType)
                {
                    case ColumnDataType.Integer:
                        OfficeOpenXml.DataValidation.Contracts.IExcelDataValidationInt integerValidation = dataRange.DataValidation.AddIntegerDataValidation();
                        integerValidation.AllowBlank = true;
                        integerValidation.ShowErrorMessage = true;
                        integerValidation.ErrorTitle = "Invalid Input";
                        integerValidation.Error = "Please enter a valid integer";
                        integerValidation.Formula.Value = int.MinValue;
                        integerValidation.Formula2.Value = int.MaxValue;
                        break;

                    case ColumnDataType.Decimal:
                        OfficeOpenXml.DataValidation.Contracts.IExcelDataValidationDecimal decimalValidation = dataRange.DataValidation.AddDecimalDataValidation();
                        decimalValidation.AllowBlank = true;
                        decimalValidation.ShowErrorMessage = true;
                        decimalValidation.ErrorTitle = "Invalid Input";
                        decimalValidation.Error = "Please enter a valid decimal number";
                        decimalValidation.Formula.Value = (double)decimal.MinValue;
                        decimalValidation.Formula2.Value = (double)decimal.MaxValue;
                        break;

                    case ColumnDataType.DateTime:
                        OfficeOpenXml.DataValidation.Contracts.IExcelDataValidationDateTime dateValidation = dataRange.DataValidation.AddDateTimeDataValidation();
                        dateValidation.AllowBlank = true;
                        dateValidation.ShowErrorMessage = true;
                        dateValidation.ErrorTitle = "Invalid Input";
                        dateValidation.Error = "Please enter a valid date";
                        dateValidation.Formula.Value = DateTime.MinValue;
                        dateValidation.Formula2.Value = DateTime.MaxValue;
                        break;

                    case ColumnDataType.Boolean:
                        OfficeOpenXml.DataValidation.Contracts.IExcelDataValidationList boolValidation = dataRange.DataValidation.AddListDataValidation();
                        boolValidation.AllowBlank = true;
                        boolValidation.ShowErrorMessage = true;
                        boolValidation.ErrorTitle = "Invalid Input";
                        boolValidation.Error = "Please select TRUE or FALSE";
                        boolValidation.Formula.Values.Add("TRUE");
                        boolValidation.Formula.Values.Add("FALSE");
                        boolValidation.HideDropDown = false;
                        break;

                    case ColumnDataType.String:
                        if (column.Length.HasValue && column.Length > 0)
                        {
                            OfficeOpenXml.DataValidation.Contracts.IExcelDataValidationInt textValidation = dataRange.DataValidation.AddTextLengthDataValidation();
                            textValidation.AllowBlank = true;
                            textValidation.ShowErrorMessage = true;
                            textValidation.ErrorTitle = "Invalid Input";
                            textValidation.Error = $"Text length must be between 0 and {column.Length} characters";
                            textValidation.Formula.Value = 0;
                            textValidation.Formula2.Value = column.Length.Value;
                        }
                        break;
                }
            }
        }

        // Enable worksheet protection
        worksheet.Protection.IsProtected = true;
        worksheet.Protection.AllowSelectLockedCells = true;
        worksheet.Protection.AllowSelectUnlockedCells = true;
        worksheet.Protection.SetPassword("password");

        // Save to stream
        MemoryStream stream = new();
        package.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }
}