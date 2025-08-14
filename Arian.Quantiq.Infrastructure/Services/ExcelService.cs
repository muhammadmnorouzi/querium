using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Enums;
using Arian.Quantiq.Application.Interfaces;
using SpreadsheetLight;

namespace Arian.Quantiq.Infrastructure.Services
{
    /// <summary>
    /// Service for generating Excel documents with structured data validation.
    /// This implementation uses the SpreadsheetLight library (version 3.5.0).
    /// Headers are locked, data cells are editable, and no password is used.
    /// </summary>
    public class ExcelService : IExcelService
    {
        /// <summary>
        /// Generates a structured Excel template based on a list of column metadata.
        /// The template includes locked column headers and data validation for data types.
        /// </summary>
        /// <param name="columns">A list of <see cref="ColumnMetadata"/> objects defining the columns.</param>
        /// <returns>A MemoryStream containing the Excel workbook file.</returns>
        public MemoryStream GenerateExcelTemplate(List<ColumnMetadata> columns)
        {
            using var document = new SLDocument();
            document.SelectWorksheet("Sheet1"); // Default worksheet
            document.RenameWorksheet("Sheet1", "Data Input");

            // Define the header row
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var cell = $"{GetExcelColumnName(i + 1)}1"; // Header row (e.g., A1, B1, ...)
                document.SetCellValue(cell, column.Name);

                // Apply header style
                var headerStyle = document.CreateStyle();
                headerStyle.Font.Bold = true;
                headerStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.Transparent);
                headerStyle.Protection.Locked = true; // Lock header cells
                document.SetCellStyle(cell, headerStyle);
            }

            // Apply data validation and formatting to data cells
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var columnName = GetExcelColumnName(i + 1);
                var dataRange = $"{columnName}2:{columnName}1048576"; // Excel's max rows

                // Unlock data cells
                var columnStyle = document.CreateStyle();
                columnStyle.Protection.Locked = false; // Unlock data cells
                document.SetColumnStyle(i + 1, columnStyle);

                // Parse data type
                if (Enum.TryParse<ColumnDataType>(column.DataType, true, out var dataType))
                {
                    var validation = document.CreateDataValidation(dataRange);

                    switch (dataType)
                    {
                        case ColumnDataType.Integer:
                            validation.AllowWholeNumber(SLDataValidationSingleOperandValues.GreaterThanOrEqual, 0, true);
                            document.AddDataValidation(validation);
                            break;

                        case ColumnDataType.Decimal:
                            validation.AllowDecimal(SLDataValidationSingleOperandValues.GreaterThanOrEqual, 0.0f, true);
                            document.AddDataValidation(validation);
                            break;

                        case ColumnDataType.DateTime:
                            validation.AllowDate(SLDataValidationSingleOperandValues.GreaterThanOrEqual, DateTime.Today, true);
                            document.AddDataValidation(validation);
                            break;

                        case ColumnDataType.Boolean:
                            validation.AllowList("TRUE,FALSE", true, true); // In-cell dropdown, allow blank
                            document.AddDataValidation(validation);
                            break;

                        case ColumnDataType.String:
                            if (column.Length.HasValue && column.Length > 0)
                            {
                                var textValidation = document.CreateDataValidation(dataRange);
                                textValidation.AllowCustom($"LEN({columnName}2)<={column.Length}", true);
                                document.AddDataValidation(textValidation);
                            }
                            break;
                    }
                }
            }

            // Enable worksheet protection without a password
            document.ProtectWorksheet(new SLSheetProtection()
            {
                AllowDeleteColumns = false,
                AllowDeleteRows = true,
                AllowSort = true,
                AllowInsertRows = true,
                AllowInsertColumns = false,
                AllowSelectLockedCells = false,
                AllowSelectUnlockedCells = true,
            });

            // Save to stream
            var stream = new MemoryStream();
            document.SaveAs(stream);
            stream.Position = 0; // Reset position for reading

            return stream;
        }

        /// <summary>
        /// Converts a column index (1-based) to Excel column name (A, B, ..., AA, AB, etc.)
        /// </summary>
        private static string GetExcelColumnName(int columnNumber)
        {
            string columnName = string.Empty;
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
    }
}