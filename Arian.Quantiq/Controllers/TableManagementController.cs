using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.DTOs.ExcelService;
using Arian.Quantiq.Application.DTOs.TableManagement;
using Arian.Quantiq.Application.Features.TableManagement.Commands.CreateTable;
using Arian.Quantiq.Application.Features.TableManagement.Commands.UploadExcel;
using Arian.Quantiq.Application.Features.TableManagement.Queries.DownloadEmptyExcel;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Arian.Quantiq.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class TableManagementController(IMediator mediator) : Controller
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApplicationResult<AppVoid>), (int)HttpStatusCode.OK)]
    public async Task<ApplicationResult<AppVoid>> Create(
        CreateTableDTO input,
        CancellationToken cancellationToken)
    {
        ApplicationResult<AppVoid> applicationResult = await mediator.Send(
            new CreateTableCommand()
            {
                CreateTableDTO = input
            }
            , cancellationToken);

        return applicationResult;
    }

    /// <summary>
    /// Generates and downloads an Excel template based on provided column metadata.
    /// </summary>
    /// <param name="columns">List of column metadata defining the Excel template structure.</param>
    /// <returns>A downloadable Excel file.</returns>
    /// <response code="200">Returns the Excel file as a download.</response>
    /// <response code="400">If the provided column metadata is invalid.</response>
    [HttpPost("download-excel-template")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorContainer), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DownloadExcelTemplate([FromBody] GetExcelTemplateDTO input, CancellationToken cancellationToken)
    {

        ApplicationResult<MemoryStream> result = await mediator.Send(new DownloadEmptyExcelQuery()
        {
            TableName = input.TableName
        }, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }


        // Set the file name and content type
        string fileName = $"{input.TableName}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        // Return the file for download
        return File(result.Data!.ToArray(), contentType, fileName);
    }

    [HttpPost("upload-excel")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApplicationResult<DynamicTableDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResult<DynamicTableDTO>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResult<DynamicTableDTO>), StatusCodes.Status500InternalServerError)]
    public async Task<ApplicationResult<DynamicTableDTO>> UploadExcel([FromForm] UploadExcelDTO input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.TableName))
        {
            return (new ErrorContainer("TableName cannot be null or empty."), HttpStatusCode.BadGateway);
        }

        if (input.ExcelFile == null || input.ExcelFile.Length == 0)
        {
            return (new ErrorContainer("Excel file is required and cannot be empty."), HttpStatusCode.BadGateway);
        }

        using MemoryStream stream = new();
        await input.ExcelFile.CopyToAsync(stream, cancellationToken);

        UploadExcelCommand command = new()
        {
            TableName = input.TableName,
            ExcelFileStream = stream
        };

        ApplicationResult<DynamicTableDTO> result = await mediator.Send(command, cancellationToken);

        return result;

    }
}