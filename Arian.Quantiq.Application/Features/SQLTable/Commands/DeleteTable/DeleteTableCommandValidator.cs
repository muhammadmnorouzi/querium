using FluentValidation;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.DeleteTable;

/// <summary>
/// Validator for <see cref="DeleteTableCommand"/>.
/// </summary>
public class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
{
    public DeleteTableCommandValidator() => RuleFor(x => x.TableName)
            .NotEmpty().WithMessage("Table name is required.")
            .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$").WithMessage("Table name must be a valid SQLite identifier.");
}
