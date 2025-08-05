using Arian.Querium.Abstractions.SQL;
using Arian.Querium.SQLite.Implementations;
using System.Text;

namespace Arian.Querium.SQLite.Tests;

public class DeleteQueryBuilderTests
{
    [Fact]
    public void ShouldBuild_SimpleDeleteQueryWithoutWhere()
    {
        // Arrange
        string expected = new StringBuilder()
            .Append("DELETE FROM users;")
            .ToString();

        IDeleteQueryBuilder builder = new SqliteDeleteQueryBuilder()
            .Delete("users");

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Empty(builder.Parameters);
    }

    [Fact]
    public void ShouldBuild_DeleteQueryWithSingleWhereCondition()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("DELETE FROM users")
            .Append("WHERE id = @p0")
            .Append(';')
            .ToString();

        IDeleteQueryBuilder builder = new SqliteDeleteQueryBuilder()
            .Delete("users")
            .Where("id", 1);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        IQueryParameter parameter = Assert.Single(builder.Parameters);
        Assert.Equal("@p0", parameter.Name);
        Assert.Equal(1, parameter.Value);
    }

    [Fact]
    public void ShouldBuild_DeleteQueryWithMultipleAndWhereConditions()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("DELETE FROM logs")
            .Append("WHERE log_level = @p0 AND created_at < @p1;")
            .ToString();

        IDeleteQueryBuilder builder = new SqliteDeleteQueryBuilder()
            .Delete("logs")
            .Where("log_level", "DEBUG")
            .Where("created_at", "2025-08-01", "<");

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(2, builder.Parameters.Count());
    }

    [Fact]
    public void ShouldBuild_DeleteQueryWithOrConditions()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("DELETE FROM emails")
            .Append("WHERE status = @p0 OR status = @p1;")
            .ToString();

        IDeleteQueryBuilder builder = new SqliteDeleteQueryBuilder()
            .Delete("emails")
            .Where("status", "draft")
            .Or("status", "pending");

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(2, builder.Parameters.Count());
    }

    [Fact]
    public void ShouldBuild_DeleteQueryWithMixedAndOrConditions()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("DELETE FROM orders")
            .Append("WHERE is_paid = @p0 AND total_amount < @p1 OR customer_id = @p2;")
            .ToString();

        IDeleteQueryBuilder builder = new SqliteDeleteQueryBuilder()
            .Delete("orders")
            .Where("is_paid", false)
            .Where("total_amount", 100.0, "<")
            .Or("customer_id", 42);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(3, builder.Parameters.Count());
    }

    [Fact]
    public void ShouldThrow_WhenTableNameIsNotSpecified()
    {
        // Arrange
        SqliteDeleteQueryBuilder builder = new SqliteDeleteQueryBuilder();

        // Act & Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => builder.Sql);
        Assert.Equal("DELETE table not specified.", exception.Message);
    }
}