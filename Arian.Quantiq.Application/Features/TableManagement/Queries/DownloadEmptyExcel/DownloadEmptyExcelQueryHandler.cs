using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Extensions;
using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Common.Results;
using Arian.Quantiq.Domain.Entities;
using Arian.Quantiq.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Net;

namespace Arian.Quantiq.Application.Features.TableManagement.Queries.DownloadEmptyExcel;

public class DownloadEmptyExcelQueryHandler(
    ITableDefinitionRepository tableDefinitionRepository,
    IExcelService excelService,
    ITableMetadataService tableMetadataService,
    IUserContextService userContextService,
    IValidator<DownloadEmptyExcelQuery> validator) : IRequestHandler<DownloadEmptyExcelQuery, ApplicationResult<MemoryStream>>
{
    public async Task<ApplicationResult<MemoryStream>> Handle(DownloadEmptyExcelQuery request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return (validationResult.ToErrorContainer(), HttpStatusCode.BadRequest);
        }

        string currentUserId = await userContextService.GetUserIdAsync();

        TableDefinition? tableDefinition = await tableDefinitionRepository.Get(
            tableName: request.TableName,
            currentUserId,
            cancellationToken);

        if (tableDefinition == null)
        {
            return (new ErrorContainer([$"Table {request.TableName} does not exist."]), HttpStatusCode.NotFound);
        }

        string? userConnectionString = await userContextService.GetUserConnectionString();


        if (string.IsNullOrWhiteSpace(userConnectionString))
        {
            return (new ErrorContainer([$"User connection string is not set."]), HttpStatusCode.NotFound);
        }

        List<ColumnMetadata> columnsMetadata = await tableMetadataService.GetTableColumnsAsync(request.TableName, userConnectionString, cancellationToken);
        MemoryStream memoryStream = excelService.GenerateExcelTemplate(columnsMetadata);

        return (memoryStream, HttpStatusCode.OK);
    }
}