using Arian.Quantiq.Application.DTOs;

namespace Arian.Quantiq.Application.Interfaces;

public interface IExcelService
{
    MemoryStream GenerateExcelTemplate(List<ColumnMetadata> columns);
}