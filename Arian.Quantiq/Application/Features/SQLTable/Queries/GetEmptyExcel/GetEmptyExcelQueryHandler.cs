using Arian.Querium.Common.Results;
using Arian.Querium.SQL.Repositories;
using Mediator;
using OfficeOpenXml;
using System.Net;

namespace Arian.Quantiq.Application.Features.SQLTable.Queries.GetEmptyExcel;

/// <summary>
/// Handles the generation of an empty Excel file for a table's structure.
/// </summary>
public class GetEmptyExcelQueryHandler(IDynamicSQLRepository repository) : IQueryHandler<GetEmptyExcelQuery, ApplicationResult<MemoryStream>>
{

    /// <summary>
    /// Generates an empty Excel file with the column headers of the specified table.
    /// </summary>
    /// <param name="request">The query containing the table name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="ApplicationResult{MemoryStream}"/> containing the Excel file stream or an error.</returns>
    public async ValueTask<ApplicationResult<MemoryStream>> Handle(GetEmptyExcelQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Dictionary<string, Querium.SQL.QueryBuilders.ColumnType> columns = await repository.GetTableColumnsAsync(request.TableName);

            if (columns == null || columns.Count == 0)
            {
                ErrorContainer error = new($"Table '{request.TableName}' not found or has no columns.");
                return new ApplicationResult<MemoryStream>(error, HttpStatusCode.NotFound);
            }

            using ExcelPackage package = new();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
            for (int i = 0; i < columns.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = columns.ElementAt(i).Key;
            }

            using MemoryStream stream = new();
            package.SaveAs(stream);
            stream.Position = 0;
            return new ApplicationResult<MemoryStream>(stream, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            ErrorContainer error = new(ex.Message);
            return new ApplicationResult<MemoryStream>(error, HttpStatusCode.InternalServerError);
        }
    }
}