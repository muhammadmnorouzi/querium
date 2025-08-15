using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Enums;
using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Common.Results;
using SpreadsheetLight;
using System.Globalization;
using System.Net;

namespace Arian.Quantiq.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private const string WorksheetName = "Data Input";

    #region GenerateExcelTemplate
    public MemoryStream GenerateExcelTemplate(IReadOnlyList<ColumnMetadata> columns)
    {
        if (columns == null)
        {
            throw new ArgumentNullException(nameof(columns), "Column metadata cannot be null.");
        }

        if (columns.Count == 0)
        {
            throw new ArgumentException("Column metadata cannot be empty.", nameof(columns));
        }

        using SLDocument document = new();
        document.SelectWorksheet("Sheet1");
        document.RenameWorksheet("Sheet1", WorksheetName);

        for (int i = 0; i < columns.Count; i++)
        {
            ColumnMetadata column = columns[i];
            if (string.IsNullOrWhiteSpace(column.Name))
            {
                throw new ArgumentException($"Column name at index {i} cannot be null or empty.", nameof(columns));
            }

            string cell = $"{GetExcelColumnName(i + 1)}1";
            document.SetCellValue(cell, column.Name);

            SLStyle headerStyle = document.CreateStyle();
            headerStyle.Font.Bold = true;
            headerStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.Transparent);
            headerStyle.Protection.Locked = true;
            document.SetCellStyle(cell, headerStyle);
        }

        for (int i = 0; i < columns.Count; i++)
        {
            ColumnMetadata column = columns[i];
            string columnName = GetExcelColumnName(i + 1);
            string dataRange = $"{columnName}2:{columnName}1048576";

            SLStyle columnStyle = document.CreateStyle();
            columnStyle.Protection.Locked = false;
            document.SetColumnStyle(i + 1, columnStyle);

            string normalizedDataType = NormalizeDataType(column.DataType);
            if (!Enum.TryParse(normalizedDataType, true, out ColumnDataType dataType))
            {
                throw new ArgumentException($"Invalid data type '{column.DataType}' for column '{column.Name}'.", nameof(columns));
            }

            SLDataValidation validation = document.CreateDataValidation(dataRange);

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
                    validation.AllowDate(SLDataValidationSingleOperandValues.GreaterThanOrEqual, new DateTime(1753, 1, 1), true);
                    document.AddDataValidation(validation);
                    break;

                case ColumnDataType.Boolean:
                    validation.AllowList("TRUE,FALSE", true, true);
                    document.AddDataValidation(validation);
                    break;

                case ColumnDataType.String:
                    if (column.Length.HasValue && column.Length > 0)
                    {
                        SLDataValidation textValidation = document.CreateDataValidation(dataRange);
                        textValidation.AllowCustom($"LEN({columnName}2)<={column.Length}", true);
                        textValidation.ShowErrorMessage = true;
                        textValidation.SetErrorAlert("Invalid Input", $"Text length must be less than or equal to {column.Length} characters.");
                        document.AddDataValidation(textValidation);
                    }
                    break;
            }
        }

        document.ProtectWorksheet(new SLSheetProtection()
        {
            AllowDeleteColumns = false,
            AllowDeleteRows = true,
            AllowSort = true,
            AllowInsertRows = true,
            AllowInsertColumns = false,
            AllowSelectLockedCells = false,
            AllowSelectUnlockedCells = true,
            AllowFormatColumns = true
        });

        MemoryStream stream = new();
        document.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }

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
            _ => dataType
        };
    }

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
    #endregion

    public async Task<ApplicationResult<DynamicTableDTO>> ExcelToDynamicData(
            MemoryStream excelFileStream,
            IReadOnlyList<ColumnMetadata> columnsMetadata,
            string tableName,
            CancellationToken cancellationToken = default)
    {
        var errorContainer = new ErrorContainer();

        if (excelFileStream == null)
        {
            errorContainer.AddError("Excel file stream cannot be null.");
            return (errorContainer, HttpStatusCode.BadRequest);
        }

        if (excelFileStream.Length == 0)
        {
            errorContainer.AddError("Excel file stream cannot be empty.");
            return (errorContainer, HttpStatusCode.BadRequest);
        }

        if (!excelFileStream.CanRead)
        {
            errorContainer.AddError("Excel file stream must be readable.");
            return (errorContainer, HttpStatusCode.BadRequest);
        }

        if (!excelFileStream.CanSeek)
        {
            errorContainer.AddError("Excel file stream must be seekable.");
            return (errorContainer, HttpStatusCode.BadRequest);
        }

        if (columnsMetadata == null || !columnsMetadata.Any())
        {
            errorContainer.AddError("Column metadata cannot be null or empty.");
            return (errorContainer, HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(tableName))
        {
            errorContainer.AddError("Table name cannot be null or empty.");
            return (errorContainer, HttpStatusCode.BadRequest);
        }

        try
        {
            excelFileStream.Position = 0;
            using var document = new SLDocument(excelFileStream, WorksheetName);

            var headers = new List<string>();
            int colIndex = 1;
            while (!string.IsNullOrWhiteSpace(document.GetCellValueAsString(1, colIndex)))
            {
                headers.Add(document.GetCellValueAsString(1, colIndex));
                colIndex++;
            }

            if (!headers.Any())
            {
                errorContainer.AddError("No headers found in the Excel file.");
                return (errorContainer, HttpStatusCode.BadRequest);
            }

            var metadataColumnNames = columnsMetadata.Select(c => c.Name).ToList();
            var invalidHeaders = headers.Except(metadataColumnNames, StringComparer.OrdinalIgnoreCase).ToList();
            if (invalidHeaders.Any())
            {
                errorContainer.AddError($"Invalid headers found: {string.Join(", ", invalidHeaders)}. Ignoring invalid columns.");
                headers.RemoveAll(h => invalidHeaders.Contains(h, StringComparer.OrdinalIgnoreCase));
            }

            var missingRequiredColumns = columnsMetadata
                .Where(c => !c.IsNullable && !headers.Contains(c.Name, StringComparer.OrdinalIgnoreCase))
                .Select(c => c.Name)
                .ToList();
            if (missingRequiredColumns.Any())
            {
                errorContainer.AddError($"Missing required columns: {string.Join(", ", missingRequiredColumns)}.");
            }

            var rows = new Dictionary<string, IList<object?>>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers)
            {
                rows[header] = [];
            }

            int rowIndex = 2;
            bool hasData = false;
            while (true)
            {
                bool rowHasData = false;
                foreach (var header in headers)
                {
                    var cellValue = document.GetCellValueAsString(rowIndex, headers.IndexOf(header) + 1);
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        rowHasData = true;
                        hasData = true;
                        break;
                    }
                }

                if (!rowHasData)
                {
                    break;
                }

                for (int i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];
                    var columnMetadata = columnsMetadata.FirstOrDefault(c => c.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                    if (columnMetadata == null)
                    {
                        continue;
                    }

                    var cellValue = document.GetCellValueAsString(rowIndex, i + 1);
                    object? value = null;

                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        value = ParseCellValue(cellValue, columnMetadata, header, rowIndex, errorContainer);
                    }

                    if (value == null && !columnMetadata.IsNullable)
                    {
                        errorContainer.AddError($"Value for non-nullable column '{header}' at row {rowIndex} cannot be null.");
                        value = GetDefaultValue(columnMetadata.DataType);
                    }

                    rows[header].Add(value);
                }

                rowIndex++;
            }

            if (!hasData)
            {
                errorContainer.AddError("No valid data rows found in the Excel file.");
                return (errorContainer, HttpStatusCode.BadRequest);
            }

            int rowCount = rows.First().Value.Count;
            var inconsistentColumns = rows.Where(r => r.Value.Count != rowCount).Select(r => r.Key).ToList();
            if (inconsistentColumns.Any())
            {
                errorContainer.AddError($"Inconsistent row sizes for columns: {string.Join(", ", inconsistentColumns)}. Truncating to shortest row count.");
                rowCount = rows.Min(r => r.Value.Count);
                foreach (var row in rows)
                {
                    while (row.Value.Count > rowCount)
                    {
                        row.Value.RemoveAt(row.Value.Count - 1);
                    }
                }
            }

            var result = new DynamicTableDTO
            {
                TableName = tableName,
                Rows = rows
            };

            return (result, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            errorContainer.AddError($"Error processing Excel file: {ex.Message}");
            return (errorContainer, HttpStatusCode.BadRequest);
        }

        await Task.CompletedTask;
    }

    private object? ParseCellValue(string cellValue, ColumnMetadata columnMetadata, string columnName, int rowIndex, ErrorContainer errorContainer)
    {
        string normalizedDataType = NormalizeDataType(columnMetadata.DataType);
        if (!Enum.TryParse<ColumnDataType>(normalizedDataType, true, out var dataType))
        {
            errorContainer.AddError($"Unsupported data type '{columnMetadata.DataType}' for column '{columnName}' at row {rowIndex}.");
            return null;
        }

        switch (dataType)
        {
            case ColumnDataType.Integer:
                if (int.TryParse(cellValue, out int intValue))
                {
                    return intValue;
                }
                errorContainer.AddError($"Expected integer for column '{columnName}' at row {rowIndex}, got '{cellValue}'.");
                return null;

            case ColumnDataType.Decimal:
                if (decimal.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue) && IsValidDecimal(decimalValue, columnMetadata.Precision, columnMetadata.Scale))
                {
                    return decimalValue;
                }
                errorContainer.AddError($"Expected decimal with precision {columnMetadata.Precision} and scale {columnMetadata.Scale} for column '{columnName}' at row {rowIndex}, got '{cellValue}'.");
                return null;

            case ColumnDataType.DateTime:
                // Try parsing as Excel serial date (numeric)
                if (double.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double serialDate))
                {
                    try
                    {
                        DateTime dateValue1 = DateTime.FromOADate(serialDate);
                        if (dateValue1 >= new DateTime(1753, 1, 1) && dateValue1 <= new DateTime(9999, 12, 31, 23, 59, 59))
                        {
                            return dateValue1;
                        }
                        errorContainer.AddError($"Date '{cellValue}' for column '{columnName}' at row {rowIndex} is outside SQL Server's valid range (1753-01-01 to 9999-12-31).");
                        return null;
                    }
                    catch (ArgumentException)
                    {
                        errorContainer.AddError($"Invalid Excel serial date '{cellValue}' for column '{columnName}' at row {rowIndex}.");
                        return null;
                    }
                }
                // Try parsing as string date
                string[] dateFormats = { "yyyy/MM/dd", "yyyy-MM-dd", "MM/dd/yyyy", "yyyy/M/d", "M/d/yyyy" };
                if (DateTime.TryParseExact(cellValue, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    if (dateValue >= new DateTime(1753, 1, 1) && dateValue <= new DateTime(9999, 12, 31, 23, 59, 59))
                    {
                        return dateValue;
                    }
                    errorContainer.AddError($"Date '{cellValue}' for column '{columnName}' at row {rowIndex} is outside SQL Server's valid range (1753-01-01 to 9999-12-31).");
                }
                else
                {
                    errorContainer.AddError($"Expected datetime for column '{columnName}' at row {rowIndex}, got '{cellValue}'.");
                }
                return null;

            case ColumnDataType.Boolean:
                if (bool.TryParse(cellValue, out bool boolValue))
                {
                    return boolValue;
                }
                // Handle Excel 1/0 for booleans
                if (cellValue == "1") return true;
                if (cellValue == "0") return false;
                errorContainer.AddError($"Expected boolean for column '{columnName}' at row {rowIndex}, got '{cellValue}'.");
                return null;

            case ColumnDataType.String:
                if (IsValidString(cellValue, columnMetadata.Length))
                {
                    return cellValue;
                }
                errorContainer.AddError($"String length exceeds {columnMetadata.Length} for column '{columnName}' at row {rowIndex}, got '{cellValue}'.");
                return null;

            default:
                errorContainer.AddError($"Unsupported data type '{columnMetadata.DataType}' for column '{columnName}' at row {rowIndex}.");
                return null;
        }
    }

    private static bool IsValidDecimal(decimal value, int? precision, int? scale)
    {
        if (precision == null || scale == null)
        {
            return true;
        }

        string decStr = value.ToString(CultureInfo.InvariantCulture);
        string[] parts = decStr.Split('.');
        int totalDigits = parts[0].TrimStart('-').Length + (parts.Length > 1 ? parts[1].Length : 0);
        int decimalPlaces = parts.Length > 1 ? parts[1].Length : 0;

        return totalDigits <= precision && decimalPlaces <= scale;
    }

    private static bool IsValidString(string value, int? length)
    {
        return length == null || value.Length <= length;
    }

    private static object? GetDefaultValue(string dataType)
    {
        return NormalizeDataType(dataType) switch
        {
            "Integer" => 0,
            "Decimal" => 0m,
            "DateTime" => new DateTime(1753, 1, 1),
            "Boolean" => false,
            "String" => "",
            _ => null
        };
    }
}