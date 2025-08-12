using Arian.Quantiq.Domain.Common.Results;
using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQL.Repositories;
using MediatR;
using OfficeOpenXml;
using System.Net;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.UploadExcel;


/// <summary>
/// Handles the upload of an Excel file to update or insert rows in a table.
/// </summary>
public class UploadExcelCommandHandler(IDynamicSQLRepository repository) : IRequestHandler<UploadExcelCommand, ApplicationResult<AppVoid>>
{
    // A placeholder for the primary key column name.
    // In a real application, this should be determined dynamically (e.g., from a configuration or a schema query).
    private const string PrimaryKeyColumnName = "Id";

    /// <summary>
    /// Processes an uploaded Excel file to update or insert rows in the specified table.
    /// </summary>
    /// <param name="request">The command containing the table name and Excel file stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ApplicationResult{AppVoid}"/> indicating success or failure.</returns>
    public async Task<ApplicationResult<AppVoid>> Handle(UploadExcelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Dictionary<string, ColumnType> columns = await repository.GetTableColumnsAsync(request.TableName);
            if (columns == null || columns.Count == 0)
            {
                ErrorContainer error = new($"Table '{request.TableName}' not found or has no columns.");
                return new ApplicationResult<AppVoid>(error, HttpStatusCode.NotFound);
            }

            using ExcelPackage package = new(request.FileStream);
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            List<string> columnNamesInDb = columns.Keys.ToList(); // Corrected to use Keys for column names

            // Read header row to map column indices
            Dictionary<int, string> headerMap = [];
            for (int col = 1; col <= columnNamesInDb.Count; col++)
            {
                string? header = worksheet.Cells[1, col].Text?.Trim();
                if (!string.IsNullOrEmpty(header) && columnNamesInDb.Contains(header))
                {
                    headerMap[col] = header;
                }
            }

            if (headerMap.Count == 0)
            {
                ErrorContainer error = new("No valid column headers found in the Excel file.");
                return new ApplicationResult<AppVoid>(error, HttpStatusCode.BadRequest);
            }

            List<Dictionary<string, object>> rows = [];

            // Read data rows starting from row 2
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                Dictionary<string, object> rowData = [];
                foreach (KeyValuePair<int, string> col in headerMap)
                {
                    object value = worksheet.Cells[row, col.Key].Value;
                    string columnName = col.Value;
                    ColumnType columnType = columns[columnName]; // Access the dictionary by key

                    // Convert value based on column type
                    object convertedValue = columnType switch
                    {
                        ColumnType.Integer => Convert.ToInt64(value ?? 0),
                        ColumnType.Real => Convert.ToDouble(value ?? 0.0),
                        ColumnType.Text => value?.ToString() ?? string.Empty,
                        ColumnType.Blob => value, // Handle as binary if needed
                        ColumnType.Numeric => Convert.ToDecimal(value ?? 0),
                        ColumnType.Boolean => value != null && Convert.ToBoolean(value),
                        _ => value
                    };
                    rowData[columnName] = convertedValue;
                }
                if (rowData.Count != 0)
                {
                    rows.Add(rowData);
                }
            }

            if (rows.Count == 0)
            {
                ErrorContainer error = new("No valid data rows found in the Excel file.");
                return new ApplicationResult<AppVoid>(error, HttpStatusCode.BadRequest);
            }

            foreach (Dictionary<string, object> row in rows)
            {
                await repository.UpsertRowAsync(request.TableName, row, PrimaryKeyColumnName);
            }

            return new ApplicationResult<AppVoid>(AppVoid.Instance, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            ErrorContainer error = new(ex.Message);
            return new ApplicationResult<AppVoid>(error, HttpStatusCode.InternalServerError);
        }
    }
}