using Arian.Querium.Abstractions.SQL;
using Arian.Querium.SQLite.Implementations;
using System.Text;

namespace Arian.Querium.SQLite.Tests;

public class UpdateQueryBuilderTests
{
    [Fact]
    public void ShouldBuild_SimpleUpdateQuery()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("UPDATE users")
            .AppendLine("SET name = @p0, email = @p1")
            .Append(';')
            .ToString();

        IUpdateQueryBuilder builder = new SqliteUpdateQueryBuilder()
            .Update("users")
            .Set("name", "John Doe")
            .Set("email", "john.doe@example.com");

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(2, builder.Parameters.Count());
        Assert.Equal("@p0", builder.Parameters.ToArray()[0].Name);
        Assert.Equal("John Doe", builder.Parameters.ToArray()[0].Value);
        Assert.Equal("@p1", builder.Parameters.ToArray()[1].Name);
        Assert.Equal("john.doe@example.com", builder.Parameters.ToArray()[1].Value);
    }

    [Fact]
    public void ShouldBuild_UpdateWithWhereCondition()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("UPDATE users")
            .AppendLine("SET name = @p0")
            .Append("WHERE id = @p1;")
            .ToString();

        IUpdateQueryBuilder builder = new SqliteUpdateQueryBuilder()
            .Update("users")
            .Set("name", "Jane Doe")
            .Where("id", 1);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(2, builder.Parameters.Count());
        Assert.Equal("@p0", builder.Parameters.ToArray()[0].Name);
        Assert.Equal("Jane Doe", builder.Parameters.ToArray()[0].Value);
        Assert.Equal("@p1", builder.Parameters.ToArray()[1].Name);
        Assert.Equal(1, builder.Parameters.ToArray()[1].Value);
    }

    [Fact]
    public void ShouldBuild_UpdateWithMultipleAndWhereConditions()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("UPDATE products")
            .AppendLine("SET price = @p0")
            .Append("WHERE category = @p1 AND in_stock = @p2;")
            .ToString();

        IUpdateQueryBuilder builder = new SqliteUpdateQueryBuilder()
            .Update("products")
            .Set("price", 99.99)
            .Where("category", "Electronics")
            .Where("in_stock", true);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(3, builder.Parameters.Count());
    }

    [Fact]
    public void ShouldBuild_UpdateWithOrWhereConditions()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("UPDATE users")
            .AppendLine("SET status = @p0")
            .Append("WHERE role = @p1 OR role = @p2;")
            .ToString();

        IUpdateQueryBuilder builder = new SqliteUpdateQueryBuilder()
            .Update("users")
            .Set("status", "inactive")
            .Where("role", "guest")
            .Or("role", "pending");

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(3, builder.Parameters.Count());
    }

    [Fact]
    public void ShouldBuild_UpdateWithMixedWhereConditions()
    {
        // Arrange
        string expected = new StringBuilder()
            .AppendLine("UPDATE orders")
            .AppendLine("SET is_paid = @p0")
            .Append("WHERE status = @p1 AND total_amount > @p2 OR customer_id = @p3;")
            .ToString();

        IUpdateQueryBuilder builder = new SqliteUpdateQueryBuilder()
            .Update("orders")
            .Set("is_paid", true)
            .Where("status", "completed")
            .Where("total_amount", 100, ">")
            .Or("customer_id", 55);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
        Assert.Equal(4, builder.Parameters.Count());
    }

    [Fact]
    public void ShouldThrow_WhenUpdateTableNotSpecified()
    {
        // Arrange
        IUpdateQueryBuilder builder = new SqliteUpdateQueryBuilder()
            .Set("name", "test");

        // Act & Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => _ = builder.Sql);
        Assert.Equal("UPDATE table not specified.", ex.Message);
    }

    [Fact]
    public void ShouldThrow_WhenNoSetClausesSpecified()
    {
        // Arrange
        IUpdateQueryBuilder builder = new SqliteUpdateQueryBuilder()
            .Update("users")
            .Where("id", 1);

        // Act & Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => _ = builder.Sql);
        Assert.Equal("No SET clauses specified.", ex.Message);
    }
}
