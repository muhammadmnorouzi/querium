using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Domain.Common.Results;

namespace Arian.Quantiq.Application.Interfaces;

public interface IExcelService
{
    MemoryStream GenerateExcelTemplate(IReadOnlyList<ColumnMetadata> columns);

    Task<ApplicationResult<DynamicTableDTO>> ExcelToDynamicData(
        MemoryStream excelFileStream,
        IReadOnlyList<ColumnMetadata> columns,
         string tableName,
        CancellationToken cancellationToken = default
    );
}