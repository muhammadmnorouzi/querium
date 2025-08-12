using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQLite.Implementations.QueryBuilders;

namespace Arian.Querium.SQLite.Tests.QueryBuilders;

public class InsertQueryBuilderTests
{
    [Fact]
    public void ShouldBuild_SimpleInsertQueryWithColumnsAndValues()
    {
        // Arrange
        string expectedSql = "INSERT INTO users (name, age) VALUES (@p0, @p1);";
        IInsertQueryBuilder builder = new SqliteInsertQueryBuilder()
            .Into("users")
            .Columns("name", "age")
            .Values("John Doe", 30);

        // Act
        string actualSql = builder.Sql;
        List<IQueryParameter> parameters = [.. builder.Parameters];

        // Assert
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(2, parameters.Count);
        Assert.Equal("@p0", parameters[0].Name);
        Assert.Equal("John Doe", parameters[0].Value);
        Assert.Equal("@p1", parameters[1].Name);
        Assert.Equal(30, parameters[1].Value);
    }

    [Fact]
    public void ShouldBuild_InsertQueryWithDifferentDataTypes()
    {
        // Arrange
        string expectedSql = "INSERT INTO products (id, name, price, is_active) VALUES (@p0, @p1, @p2, @p3);";
        IInsertQueryBuilder builder = new SqliteInsertQueryBuilder()
            .Into("products")
            .Columns("id", "name", "price", "is_active")
            .Values(1, "Laptop", 999.99, true);

        // Act
        string actualSql = builder.Sql;
        List<IQueryParameter> parameters = [.. builder.Parameters];

        // Assert
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(4, parameters.Count);
        Assert.Equal("@p0", parameters[0].Name);
        Assert.Equal(1, parameters[0].Value);
        Assert.Equal("@p1", parameters[1].Name);
        Assert.Equal("Laptop", parameters[1].Value);
        Assert.Equal("@p2", parameters[2].Name);
        Assert.Equal(999.99, parameters[2].Value);
        Assert.Equal("@p3", parameters[3].Name);
        Assert.Equal(true, parameters[3].Value);
    }

    [Fact]
    public void ShouldThrow_WhenIntoTableNotSpecified()
    {
        // Arrange
        IInsertQueryBuilder builder = new SqliteInsertQueryBuilder()
            .Columns("name")
            .Values("test");

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() => builder.Sql);
    }

    [Fact]
    public void ShouldThrow_WhenColumnCountDoesNotMatchValueCount()
    {
        // Arrange
        IInsertQueryBuilder builder = new SqliteInsertQueryBuilder()
            .Into("users")
            .Columns("name", "age")
            .Values("John Doe");

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() => builder.Sql);
    }

    [Fact]
    public void ShouldBuild_InsertQueryWithNoColumnsSpecified()
    {
        // Arrange
        string expectedSql = "INSERT INTO users VALUES (@p0, @p1, @p2);";
        IInsertQueryBuilder builder = new SqliteInsertQueryBuilder()
            .Into("users")
            .Values(1, "Jane Doe", 25);

        // Act
        string actualSql = builder.Sql;
        List<IQueryParameter> parameters = [.. builder.Parameters];

        // Assert
        Assert.Equal(expectedSql, actualSql);
        Assert.Equal(3, parameters.Count);
        Assert.Equal(1, parameters[0].Value);
        Assert.Equal("Jane Doe", parameters[1].Value);
        Assert.Equal(25, parameters[2].Value);
    }
}