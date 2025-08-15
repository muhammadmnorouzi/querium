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

public class UploadExcelCommandHandler(
    IValidator<UploadExcelCommand> validator,
    IExcelService excelService,
    ITableMetadataService tableMetadataService,
    IUserContextService userContextService,
    ISQLTableManager sqlTableManager) : IRequestHandler<UploadExcelCommand, ApplicationResult<DynamicTableDTO>>
{

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
