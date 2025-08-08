using FluentValidation;

namespace Arian.Quantiq.Application.Features.SQLTable.Queries.GetEmptyExcel;

/// <summary>
/// Validator for <see cref="GetEmptyExcelQuery"/>.
/// </summary>
public class GetEmptyExcelQueryValidator : AbstractValidator<GetEmptyExcelQuery>
{
    public GetEmptyExcelQueryValidator() => RuleFor(x => x.TableName)
            .NotEmpty().WithMessage("Table name is required.")
            .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$").WithMessage("Table name must be a valid SQLite identifier.");
}
