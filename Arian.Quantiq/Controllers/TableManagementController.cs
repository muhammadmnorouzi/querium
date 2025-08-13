using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;
using Arian.Quantiq.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Arian.Quantiq.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TableManagementController(IMediator mediator) : Controller
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
}