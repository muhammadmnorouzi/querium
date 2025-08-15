using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Extensions;
using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Common.Results;
using Arian.Quantiq.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Net;

namespace Arian.Quantiq.Application.Features.TableManagement.Commands.UploadExcel;

/// <summary>
/// Handles the <see cref="UploadExcelCommand"/> to import Excel data into a dynamic table.
/// </summary>
public class UploadExcelCommandHandler : IRequestHandler<UploadExcelCommand, ApplicationResult<DynamicTableDTO>>
{
    private readonly IValidator<UploadExcelCommand> validator;
    private readonly IExcelService excelService;
    private readonly ITableMetadataService tableMetadataService;
    private readonly IUserContextService userContextService;
    private readonly ISQLTableManager sqlTableManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadExcelCommandHandler"/> class.
    /// </summary>
    public UploadExcelCommandHandler(
        IValidator<UploadExcelCommand> validator,
        IExcelService excelService,
        ITableMetadataService tableMetadataService,
        IUserContextService userContextService,
        ISQLTableManager sqlTableManager)
    {
        this.validator = validator;
        this.excelService = excelService;
        this.tableMetadataService = tableMetadataService;
        this.userContextService = userContextService;
        this.sqlTableManager = sqlTableManager;
    }

    /// <inheritdoc />
    public async Task<ApplicationResult<DynamicTableDTO>> Handle(UploadExcelCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return (validationResult.ToErrorContainer(), HttpStatusCode.BadRequest);
        }

        string? connectionString = await userContextService.GetUserConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return (new ErrorContainer(["Connection string "]), HttpStatusCode.BadRequest);
        }

        IReadOnlyList<ColumnMetadata> columnsMetadata = await tableMetadataService.GetTableColumnsAsync(
            request.TableName,
            connectionString,
            cancellationToken);

        ApplicationResult<DynamicTableDTO> applicationResult = await excelService.ExcelToDynamicData(
            request.ExcelFileStream,
            columnsMetadata,
            request.TableName,
            cancellationToken);

        if (applicationResult.IsFailure)
        {
            return (applicationResult.Error!, applicationResult.HttpStatusCode);
        }

        ApplicationResult<DynamicTableDTO> insertionResult = await sqlTableManager.InsertDynamicData(
            applicationResult.Data!,
            connectionString,
            cancellationToken);

        return insertionResult;
    }
}
