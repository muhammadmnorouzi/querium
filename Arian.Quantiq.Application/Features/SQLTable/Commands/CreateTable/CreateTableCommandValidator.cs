using Arian.Querium.SQL.QueryBuilders;
using FluentValidation;

namespace Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;

/// <summary>
/// Validator for <see cref="CreateTableCommand"/>.
/// </summary>
public class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    public CreateTableCommandValidator()
    {
        RuleFor(x => x.TableName)
            .NotEmpty().WithMessage("Table name is required.")
            .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$").WithMessage("Table name must be a valid SQLite identifier.");

        RuleFor(x => x.PrimaryKeyColumn)
            .NotEmpty().WithMessage("Primary key column is required.")
            .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$").WithMessage("Primary key column name must be a valid SQLite identifier.")
            .Must((command, primaryKey) => command.Columns.Any(c => c.Name == primaryKey))
            .WithMessage("Primary key column must exist in the columns list.")
            .Must((command, primaryKey) =>
                command.Columns.Any(c => c.Name == primaryKey && c.Type == ColumnType.Integer))
            .WithMessage("Primary key column must be of type Integer.");

        RuleFor(x => x.Columns)
            .NotEmpty().WithMessage("At least one column is required.");

        RuleForEach(x => x.Columns).ChildRules(column =>
        {
            column.RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Column name is required.")
                .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$").WithMessage("Column name must be a valid SQLite identifier.");

            column.RuleFor(c => c.Type)
                .IsInEnum().WithMessage($"Column type must be a valid SQLite data type: {string.Join(", ", Enum.GetNames<ColumnType>())}.");
        });
    }
}
