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

/// <summary>
/// Handles the <see cref="DownloadEmptyExcelQuery"/> to generate and return an Excel template for a table.
/// </summary>
public class DownloadEmptyExcelQueryHandler : IRequestHandler<DownloadEmptyExcelQuery, ApplicationResult<MemoryStream>>
{
    private readonly ITableDefinitionRepository tableDefinitionRepository;
    private readonly IExcelService excelService;
    private readonly ITableMetadataService tableMetadataService;
    private readonly IUserContextService userContextService;
    private readonly IValidator<DownloadEmptyExcelQuery> validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadEmptyExcelQueryHandler"/> class.
    /// </summary>
    public DownloadEmptyExcelQueryHandler(
        ITableDefinitionRepository tableDefinitionRepository,
        IExcelService excelService,
        ITableMetadataService tableMetadataService,
        IUserContextService userContextService,
        IValidator<DownloadEmptyExcelQuery> validator)
    {
        this.tableDefinitionRepository = tableDefinitionRepository;
        this.excelService = excelService;
        this.tableMetadataService = tableMetadataService;
        this.userContextService = userContextService;
        this.validator = validator;
    }

    /// <inheritdoc />
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

        IReadOnlyList<ColumnMetadata> columnsMetadata = await tableMetadataService.GetTableColumnsAsync(request.TableName, userConnectionString, cancellationToken);
        MemoryStream memoryStream = excelService.GenerateExcelTemplate(columnsMetadata);

        return (memoryStream, HttpStatusCode.OK);
    }
}