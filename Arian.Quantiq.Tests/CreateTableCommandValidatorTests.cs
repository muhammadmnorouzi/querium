using Arian.Quantiq.Application.Features.SQLTable.Commands.CreateTable;
using Arian.Querium.SQL.QueryBuilders;
using FluentValidation.TestHelper;
using System.Net;

namespace Arian.Quantiq.Tests;

[Collection("Database collection")]
public class CreateTableCommandValidatorTests
{
    private readonly CreateTableCommandValidator _validator = new();

    [Fact]
    public void ShouldFail_WhenTableNameIsEmpty()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "",
            PrimaryKeyColumn = "Id",
            Columns =
            [
                new() { Name = "Id", Type = ColumnType.Integer }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TableName)
              .WithErrorMessage("Table name is required.");
    }

    [Fact]
    public void ShouldFail_WhenTableNameIsInvalid()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "1_invalid_table_name",
            PrimaryKeyColumn = "Id",
            Columns =
            [
                new() { Name = "Id", Type = ColumnType.Integer }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TableName)
              .WithErrorMessage("Table name must be a valid SQLite identifier.");
    }

    [Fact]
    public void ShouldFail_WhenPrimaryKeyColumnIsEmpty()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "",
            Columns =
            [
                new() { Name = "Id", Type = ColumnType.Integer }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PrimaryKeyColumn)
              .WithErrorMessage("Primary key column is required.");
    }

    [Fact]
    public void ShouldFail_WhenPrimaryKeyColumnIsInvalid()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "invalid-id",
            Columns =
            [
                new() { Name = "invalid-id", Type = ColumnType.Integer }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PrimaryKeyColumn)
              .WithErrorMessage("Primary key column name must be a valid SQLite identifier.");
    }

    [Fact]
    public void ShouldFail_WhenPrimaryKeyColumnIsNotInteger()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "Id",
            Columns =
            [
                new() { Name = "Id", Type = ColumnType.Text } // Primary key is not an integer
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PrimaryKeyColumn)
              .WithErrorMessage("Primary key column must be of type Integer.");
    }

    [Fact]
    public void ShouldFail_WhenColumnsListIsEmpty()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "Id",
            Columns = new List<ColumnDefinition>()
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Columns)
              .WithErrorMessage("At least one column is required.");
    }

    [Fact]
    public void ShouldFail_WhenColumnNameIsEmpty()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "Id",
            Columns =
            [
                new() { Name = "", Type = ColumnType.Integer }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Columns[0].Name")
              .WithErrorMessage("Column name is required.");
    }

    [Fact]
    public void ShouldFail_WhenColumnNameIsInvalid()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "Id",
            Columns =
            [
                new() { Name = "invalid-name", Type = ColumnType.Integer }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Columns[0].Name")
              .WithErrorMessage("Column name must be a valid SQLite identifier.");
    }

    [Fact]
    public void ShouldFail_WhenColumnTypeIsInvalid()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "Id",
            Columns =
            [
                new() { Name = "Id", Type = (ColumnType)999 }, // Invalid enum value
                new() { Name = "Name", Type = ColumnType.Text }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Columns[0].Type")
              .WithErrorMessage("Column type must be a valid SQLite data type: Integer, Real, Text, Blob, Numeric, Boolean.");
    }

    [Fact]
    public void ShouldPass_WhenColumnTypeIsValid()
    {
        // Arrange
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "Id",
            Columns =
            [
                new() { Name = "Id", Type = ColumnType.Integer },
                new() { Name = "Name", Type = ColumnType.Text }
            ]
        };

        // Act
        TestValidationResult<CreateTableCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateTableCommandValidator_ShouldFail_WhenPrimaryKeyColumnMissing()
    {
        CreateTableCommandValidator validator = new();
        CreateTableCommand command = new()
        {
            TableName = "users",
            PrimaryKeyColumn = "UserId",
            Columns = new List<ColumnDefinition>
        {
            new() { Name = "Name", Type = ColumnType.Text }
        }
        };

        FluentValidation.Results.ValidationResult result = await validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Primary key column must exist in the columns list."));
    }
}
