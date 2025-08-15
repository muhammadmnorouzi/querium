using FluentValidation;

namespace Arian.Quantiq.Application.Features.TableManagement.Commands.UploadExcel;

public class UploadExcelCommandValidator : AbstractValidator<UploadExcelCommand>
{
    public UploadExcelCommandValidator()
    {
        RuleFor(x => x.TableName)
            .NotEmpty()
            .WithMessage("TableName cannot be null or empty.")
            .MaximumLength(128)
            .WithMessage("TableName cannot exceed 128 characters.")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("TableName can only contain letters, numbers, and underscores.");

        RuleFor(x => x.ExcelFileStream)
            .NotNull()
            .WithMessage("ExcelFileStream cannot be null.")
            .Must(stream => stream?.Length > 0)
            .WithMessage("ExcelFileStream cannot be empty.")
            .Must(stream => stream?.CanRead == true)
            .WithMessage("ExcelFileStream must be readable.")
            .Must(stream => stream?.CanSeek == true)
            .WithMessage("ExcelFileStream must be seekable.");
    }
}
