using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQLite.Implementations.QueryBuilders;
using System.Text;

namespace Arian.Querium.SQLite.Tests.QueryBuilders;

public class SelectQueryBuilderTests
{
    [Fact]
    public void ShouldBuild_SimpleSelectQuery()
    {
        string expectedQuery = new StringBuilder()
            .AppendLine("SELECT id, name")
            .AppendLine("FROM users")
            .AppendLine("WHERE id = @p0")
            .Append("ORDER BY name ASC")
            .Append(';')
            .ToString();

        ISelectQueryBuilder builder = new SqliteSelectQueryBuilder()
            .Select("id", "name")
            .From("users")
            .Where("id", 1)
            .OrderBy("name", SortOrder.Ascending);

        string query = builder.Sql;

        Assert.Equal(expectedQuery, query);
        _ = Assert.Single(builder.Parameters);
        Assert.Equal("@p0", builder.Parameters.ToArray()[0].Name);
    }

    [Fact]
    public void ShouldBuild_SelectAll_FromTable()
    {
        string expected = new StringBuilder()
            .AppendLine("SELECT *")
            .Append("FROM users;")
            .ToString();

        ISelectQueryBuilder builder = new SqliteSelectQueryBuilder()
            .From("users");

        Assert.Equal(expected, builder.Sql);
        Assert.Empty(builder.Parameters);
    }

    [Fact]
    public void ShouldBuild_SelectWithMultipleWhereAndConditions()
    {
        string expected = new StringBuilder()
            .AppendLine("SELECT id, name")
            .AppendLine("FROM users")
            .Append("WHERE name = @p0 AND age > @p1;")
            .ToString();

        ISelectQueryBuilder builder = new SqliteSelectQueryBuilder()
            .Select("id", "name")
            .From("users")
            .Where("name", "Ali")
            .Where("age", 18, ">");

        Assert.Equal(expected, builder.Sql);
        Assert.Equal(2, builder.Parameters.Count());
        Assert.Equal("@p0", builder.Parameters.ToArray()[0].Name);
        Assert.Equal("@p1", builder.Parameters.ToArray()[1].Name);
    }

    [Fact]
    public void ShouldBuild_SelectWithOrConditions()
    {
        string expected = new StringBuilder()
            .AppendLine("SELECT *")
            .AppendLine("FROM products")
            .Append("WHERE category = @p0 OR stock > @p1;")
            .ToString();

        ISelectQueryBuilder builder = new SqliteSelectQueryBuilder()
            .From("products")
            .Where("category", "electronics")
            .Or("stock", 10, ">");

        Assert.Equal(expected, builder.Sql);
        Assert.Equal(2, builder.Parameters.Count());
    }

    [Fact]
    public void ShouldBuild_SelectWithMultipleOrderBy()
    {
        string expected = new StringBuilder()
            .AppendLine("SELECT *")
            .AppendLine("FROM sales")
            .Append("ORDER BY date DESC,amount ASC")
            .Append(';')
            .ToString();

        ISelectQueryBuilder builder = new SqliteSelectQueryBuilder()
            .From("sales")
            .OrderBy("date", SortOrder.Descending)
            .OrderBy("amount", SortOrder.Ascending);

        Assert.Equal(expected, builder.Sql);
        Assert.Empty(builder.Parameters);
    }

    [Fact]
    public void ShouldThrowIfFromNotSpecified()
    {
        SqliteSelectQueryBuilder builder = new();
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => _ = builder.Sql);
        Assert.Equal("FROM table not specified.", ex.Message);
    }
}