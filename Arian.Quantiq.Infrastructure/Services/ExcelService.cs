using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Enums;
using Arian.Quantiq.Application.Interfaces;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;

namespace Arian.Quantiq.Infrastructure.Services
{
    /// <summary>
    /// Service for generating Excel documents with structured data validation using SpreadsheetLight (version 3.5.0).
    /// Headers are locked and unchangeable, data cells are editable, and columns are resizable.
    /// </summary>
    public class ExcelService : IExcelService
    {
        private const string WorksheetName = "Data Input";
        private const string ProtectionPassword = "securepassword123"; // Configurable password

        /// <summary>
        /// Generates a structured Excel template based on a list of column metadata.
        /// The template includes locked column headers, editable data cells, and resizable columns.
        /// </summary>
        /// <param name="columns">A list of <see cref="ColumnMetadata"/> objects defining the columns.</param>
        /// <returns>A MemoryStream containing the Excel workbook file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if columns is null.</exception>
        /// <exception cref="ArgumentException">Thrown if columns is empty or contains invalid data.</exception>
        public MemoryStream GenerateExcelTemplate(List<ColumnMetadata> columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns), "Column metadata cannot be null.");
            }
            if (columns.Count == 0)
            {
                throw new ArgumentException("Column metadata cannot be empty.", nameof(columns));
            }

            using var document = new SLDocument();
            document.SelectWorksheet("Sheet1");
            document.RenameWorksheet("Sheet1", WorksheetName);

            // Define the header row
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                if (string.IsNullOrWhiteSpace(column.Name))
                {
                    throw new ArgumentException($"Column name at index {i} cannot be null or empty.", nameof(columns));
                }

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

                // Normalize data type
                var normalizedDataType = NormalizeDataType(column.DataType);
                if (!Enum.TryParse<ColumnDataType>(normalizedDataType, true, out var dataType))
                {
                    throw new ArgumentException($"Invalid data type '{column.DataType}' for column '{column.Name}'.", nameof(columns));
                }

                var validation = document.CreateDataValidation(dataRange);

                switch (dataType)
                {
                    case ColumnDataType.Integer:
                        validation.AllowWholeNumber(SLDataValidationSingleOperandValues.GreaterThanOrEqual, int.MinValue, true);
                        document.AddDataValidation(validation);
                        break;

                    case ColumnDataType.Decimal:
                        validation.AllowDecimal(SLDataValidationSingleOperandValues.GreaterThanOrEqual, (double)decimal.MinValue, true);
                        document.AddDataValidation(validation);
                        break;

                    case ColumnDataType.DateTime:
                        validation.AllowDate(SLDataValidationSingleOperandValues.GreaterThanOrEqual, DateTime.MinValue, true);
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
                            textValidation.ShowErrorMessage = true;
                            textValidation.SetErrorAlert("Invalid Input", $"Text length must be less than or equal to {column.Length} characters.");
                            document.AddDataValidation(textValidation);
                        }
                        break;
                }
            }

            // Enable worksheet protection with column resizing allowed
            document.ProtectWorksheet(new SLSheetProtection()
            {
                AllowDeleteColumns = false,
                AllowDeleteRows = true,
                AllowSort = true,
                AllowInsertRows = true,
                AllowInsertColumns = false,
                AllowSelectLockedCells = false,
                AllowSelectUnlockedCells = true,
                AllowFormatColumns = true // Allow column resizing
            });

            // Save to stream
            var stream = new MemoryStream();
            document.SaveAs(stream);
            stream.Position = 0; // Reset position for reading

            return stream;
        }

        /// <summary>
        /// Normalizes common data type aliases to match ColumnDataType enum values.
        /// </summary>
        private static string NormalizeDataType(string dataType)
        {
            if (string.IsNullOrWhiteSpace(dataType))
            {
                return dataType;
            }

            return dataType.ToLowerInvariant() switch
            {
                "int" => "Integer",
                "integer" => "Integer",
                "decimal" => "Decimal",
                "float" => "Decimal",
                "double" => "Decimal",
                "datetime" => "DateTime",
                "date" => "DateTime",
                "bit" => "Boolean",
                "bool" => "Boolean",
                "boolean" => "Boolean",
                "string" => "String",
                "text" => "String",
                "nvarchar" => "String",
                "varchar" => "String",
                _ => dataType // Return original if no match
            };
        }

        /// <summary>
        /// Converts a column index (1-based) to Excel column name (A, B, ..., AA, AB, etc.)
        /// </summary>
        private static string GetExcelColumnName(int columnNumber)
        {
            if (columnNumber < 1)
            {
                throw new ArgumentException("Column number must be greater than or equal to 1.", nameof(columnNumber));
            }

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