using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.DTOs.TableManagement;
using Arian.Quantiq.Application.Enums;
using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Common.Results;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Arian.Quantiq.Infrastructure.Services;

/// <summary>
/// An example implementation of <see cref="IDatabaseCompiler"/> for SQL Server.
/// This class knows how to translate the abstract model into SQL Server-specific syntax.
/// </summary>
public class SqlServerCompiler : IDatabaseCompiler
{
    private static readonly Regex __sqlIdentRegex = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    public async Task<ApplicationResult<string>> Compile(CreateTableDTO model, CancellationToken cancellationToken)
    {
        string errorMessage = await Validate(model, cancellationToken);

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            return (errorMessage, HttpStatusCode.BadRequest);
        }

        StringBuilder builder = new();
        _ = builder.AppendLine($"CREATE TABLE [{model.TableName}] (");

        List<string> columnDefinitions = [];

        foreach (CreateColumnDTO column in model.Columns)
        {
            StringBuilder columnBuilder = new();
            _ = columnBuilder.Append($"    [{column.Name}] ");

            // Translate abstract data type to SQL Server-specific type
            switch (column.DataType)
            {
                case ColumnDataType.String:
                    _ = columnBuilder.Append($"NVARCHAR({(column.Length.HasValue ? column.Length.Value.ToString() : "MAX")})");
                    break;
                case ColumnDataType.Integer:
                    _ = columnBuilder.Append("INT");
                    break;
                case ColumnDataType.Decimal:
                    _ = columnBuilder.Append($"DECIMAL({column.Precision ?? 18}, {column.Scale ?? 2})");
                    break;
                case ColumnDataType.DateTime:
                    _ = columnBuilder.Append("DATETIME");
                    break;
                case ColumnDataType.Boolean:
                    _ = columnBuilder.Append("BIT");
                    break;
            }

            // Append column constraints
            if (column.IsAutoIncrementing)
            {
                _ = columnBuilder.Append(" IDENTITY(1,1)");
            }

            if (column.IsNullable)
            {
                _ = columnBuilder.Append(" NULL");
            }
            else
            {
                _ = columnBuilder.Append(" NOT NULL");
            }

            if (column.IsPrimaryKey)
            {
                _ = columnBuilder.Append(" PRIMARY KEY");
            }

            columnDefinitions.Add(columnBuilder.ToString());
        }

        _ = builder.AppendLine(string.Join(",\n", columnDefinitions));
        _ = builder.AppendLine(");");

        await Task.CompletedTask;
        return (builder.ToString(), HttpStatusCode.OK);
    }

    /// <summary>
    /// Validates a table update request for SQL Server.
    /// This method is the core of the validation logic.
    /// </summary>
    public async Task<string> ValidateUpdate(UpdateTableDTO updateDto, List<ColumnMetadata> existingColumns)
    {
        if (string.IsNullOrWhiteSpace(updateDto.TableName))
        {
            return "Table name cannot be null or whitespace.";
        }

        if (updateDto.ColumnUpdates == null || updateDto.ColumnUpdates.Count == 0)
        {
            return "No column updates were provided.";
        }

        // Create a dictionary for efficient lookup of existing columns.
        Dictionary<string, ColumnMetadata> existingColumnsDict = existingColumns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        // Track columns that are being dropped or renamed
        HashSet<string> droppedColumns = new(StringComparer.OrdinalIgnoreCase);

        foreach (UpdateColumnDTO update in updateDto.ColumnUpdates)
        {
            if (update.Operation == UpdateOperationType.Add)
            {
                // Check if the column to be added already exists.
                if (existingColumnsDict.ContainsKey(update.NewName))
                {
                    return $"Column '{update.NewName}' already exists.";
                }
            }
            else if (update.Operation == UpdateOperationType.Drop)
            {
                // Check if the column to be dropped actually exists.
                if (!existingColumnsDict.ContainsKey(update.OldName))
                {
                    return $"Column '{update.OldName}' does not exist.";
                }
                // Check for dependencies or constraints on the column (not implemented here but essential).
#warning TODO
                droppedColumns.Add(update.OldName);
            }
            else if (update.Operation == UpdateOperationType.Alter || update.Operation == UpdateOperationType.Rename)
            {
                // Check if the column to be altered/renamed exists.
                if (!existingColumnsDict.ContainsKey(update.OldName))
                {
                    return $"Column '{update.OldName}' does not exist.";
                }

                // If renaming, ensure the new name is not a duplicate.
                if (update.Operation == UpdateOperationType.Rename && existingColumnsDict.ContainsKey(update.NewName))
                {
                    return $"A column with the new name '{update.NewName}' already exists.";
                }
            }
        }

        await Task.CompletedTask;
        return string.Empty; // Return empty string if validation passes.
    }

    public Task<bool> IsValidSqlIdentifier(string sqlIdentifier)
    {
        if (string.IsNullOrWhiteSpace(sqlIdentifier) || sqlIdentifier.Length > 128)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(__sqlIdentRegex.IsMatch(sqlIdentifier));
    }

    /// <summary>
    /// Validates the abstract table creation model for common errors.
    /// </summary>
    /// <param name="input">The model to validate.</param>
    public async Task<string> Validate(CreateTableDTO input, CancellationToken _)
    {
        if (string.IsNullOrWhiteSpace(input.TableName))
        {
            return "Table name cannot be null or whitespace.";
        }

        if (!await IsValidSqlIdentifier(input.TableName))
        {
            return $"Table name {input.TableName} is not a valid SQL identifier.";
        }

        if (input.Columns == null || input.Columns.Count == 0)
        {
            return "Table must have at least one column.";
        }

        HashSet<string> columnNames = [];
        int primaryKeyCount = 0;

        foreach (CreateColumnDTO column in input.Columns)
        {
            if (string.IsNullOrWhiteSpace(column.Name))
            {
                return "Column name cannot be null or whitespace.";
            }

            if (!await IsValidSqlIdentifier(column.Name))
            {
                return $"Column name {column.Name} is not a valid SQL identifier.";
            }

            if (!columnNames.Add(column.Name.ToLower()))
            {
                return $"Duplicate column name found: '{column.Name}'.";
            }

            if (column.IsPrimaryKey)
            {
                primaryKeyCount++;
            }

            if (column.IsAutoIncrementing && column.DataType != ColumnDataType.Integer)
            {
                return $"Auto-incrementing column '{column.Name}' must be of type 'Integer'.";
            }
        }

        if (primaryKeyCount > 1)
        {
            return "A table can have at most one primary key.";
        }

        return string.Empty;
    }
}