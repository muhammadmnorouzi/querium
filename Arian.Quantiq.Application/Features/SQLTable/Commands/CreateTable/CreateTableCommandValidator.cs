using Arian.Quantiq.Application.Interfaces;
using FluentValidation;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;

/// <summary>
/// Validator for <see cref="CreateTableCommand"/>.
/// </summary>
public class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    private readonly IDatabaseCompiler _databaseCompiler;

    public CreateTableCommandValidator(IDatabaseCompiler databaseCompiler)
    {
        // Store the compiler in a private field.
        _databaseCompiler = databaseCompiler;

        RuleFor(x => x.CreateTableDTO)
            .NotNull()
            .WithMessage("Table creation DTO cannot be null.")
            .CustomAsync(async (createTableDTO, context, cancellationToken) =>
            {
                // Call the validation logic and get the error message.
                var validationError = await _databaseCompiler.Validate(createTableDTO, cancellationToken);

                // If a validation error message exists, add it to the context.
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    context.AddFailure(validationError);
                }
            });
    }
}