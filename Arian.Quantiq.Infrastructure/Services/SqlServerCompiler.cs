using Arian.Quantiq.Application.DTOs;
using Arian.Quantiq.Application.Enums;
using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Common.Results;
using System.Net;
using System.Text;

namespace Arian.Quantiq.Infrastructure.Services;

/// <summary>
/// An example implementation of <see cref="IDatabaseCompiler"/> for SQL Server.
/// This class knows how to translate the abstract model into SQL Server-specific syntax.
/// </summary>
public class SqlServerCompiler : IDatabaseCompiler
{
    public async Task<ApplicationResult<string>> Compile(CreateTableDTO model, CancellationToken cancellationToken)
    {
        string errorMessage = await Validate(model , cancellationToken);

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
    /// Validates the abstract table creation model for common errors.
    /// </summary>
    /// <param name="input">The model to validate.</param>
    public async Task<string> Validate(CreateTableDTO input, CancellationToken _)
    {
        if (string.IsNullOrWhiteSpace(input.TableName))
        {
            return "Table name cannot be null or whitespace.";
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

        await Task.CompletedTask;

        return string.Empty;
    }
}
