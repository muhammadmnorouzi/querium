using Arian.Quantiq.Application.Interfaces;
using Arian.Querium.SQL.QueryBuilders;
using FluentValidation;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;

/// <summary>
/// Validator for <see cref="CreateTableCommand"/>.
/// </summary>
public class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    public CreateTableCommandValidator(IDatabaseCompiler databaseCompiler)
    {
        string validationError = string.Empty;

        _ = RuleFor(x => x.CreateTableDTO)
            .NotNull()
            .WithMessage("Table creation DTO can not be null.")

            .MustAsync(async (createTableDTO , cancellationToken) =>
            {
                validationError = await databaseCompiler.Validate(createTableDTO, cancellationToken);

                return !string.IsNullOrWhiteSpace(validationError);
            })
            .WithMessage(validationError);
    }
}
