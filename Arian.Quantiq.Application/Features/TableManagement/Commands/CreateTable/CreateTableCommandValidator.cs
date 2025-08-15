using Arian.Quantiq.Application.Interfaces;
using FluentValidation;

namespace Arian.Quantiq.Application.Features.TableManagement.Commands.CreateTable;

/// <summary>
/// Validator for <see cref="CreateTableCommand"/>. Validates the table creation request using the provided database compiler.
/// </summary>
public class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    private readonly IDatabaseCompiler _databaseCompiler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableCommandValidator"/> class.
    /// </summary>
    /// <param name="databaseCompiler">The database compiler used for additional validation.</param>
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
                string validationError = await _databaseCompiler.Validate(createTableDTO, cancellationToken);

                // If a validation error message exists, add it to the context.
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    context.AddFailure(validationError);
                }
            });
    }
}