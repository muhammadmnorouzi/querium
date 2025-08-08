using FluentValidation;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.UploadExcel;

/// <summary>
/// Validator for <see cref="UploadExcelCommand"/>.
/// </summary>
public class UploadExcelCommandValidator : AbstractValidator<UploadExcelCommand>
{
    public UploadExcelCommandValidator()
    {
        RuleFor(x => x.TableName)
            .NotEmpty().WithMessage("Table name is required.")
            .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$").WithMessage("Table name must be a valid SQLite identifier.");

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File stream is required.");
    }
}