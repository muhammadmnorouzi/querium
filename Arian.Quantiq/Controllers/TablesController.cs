using Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;
using Arian.Quantiq.Application.Features.SQLTable.Commands.UploadExcel;
using Arian.Quantiq.Application.Features.SQLTable.Queries.GetEmptyExcel;
using Arian.Quantiq.Domain.Common.Results;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Arian.Quantiq.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TablesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationResult<AppVoid>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApplicationResult<AppVoid>), (int)HttpStatusCode.OK)]
    public async Task<ApplicationResult<AppVoid>> CreateTable([FromBody] CreateTableCommand command)
    {
        ApplicationResult<AppVoid> result = await _mediator.Send(command);

        return result;
    }


    /// <summary>
    /// Downloads an empty Excel file with the table's column structure.
    /// </summary>
    /// <param name="query">The query containing the table name.</param>
    /// <returns>A file containing the empty Excel template.</returns>
    [HttpGet("download-excel")]
    public async Task<IActionResult> DownloadEmptyExcel([FromQuery] GetEmptyExcelQuery query)
    {
        ApplicationResult<MemoryStream> result = await _mediator.Send(query);
        if (result.IsSuccess)
        {
            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{query.TableName}.xlsx");
        }
        return BadRequest(result.Error.Messages);
    }

    /// <summary>
    /// Uploads an Excel file to update or insert rows in a table.
    /// </summary>
    /// <param name="tableName">The name of the table to update.</param>
    /// <param name="file">The uploaded Excel file.</param>
    /// <returns>An IActionResult indicating success or failure.</returns>
    [HttpPost("upload-excel")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApplicationResult<AppVoid>), (int)HttpStatusCode.OK)]
    public async Task<ApplicationResult<AppVoid>> UploadExcel([FromQuery] string tableName, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return (new ErrorContainer("No file uploaded."),HttpStatusCode.BadRequest);
        }

        using Stream stream = file.OpenReadStream();

        UploadExcelCommand command = new()
        {
            TableName = tableName,
            FileStream = stream
        };

        ApplicationResult<AppVoid> result = await _mediator.Send(command);

        return result;
    }
}
