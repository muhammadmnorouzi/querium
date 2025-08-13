using FluentValidation;

namespace Arian.Quantiq.Application.Features.TableManagement.Queries.DownloadEmptyExcel;

public class DownloadEmptyExcelQueryValidator : AbstractValidator<DownloadEmptyExcelQuery>
{
    public DownloadEmptyExcelQueryValidator()
    {
        RuleFor(item => item.TableName)
            .NotEmpty().WithMessage("Table name can not be empty.")
            .MaximumLength(255).WithMessage("Table name can have at most 255 characters.");
    }
}
