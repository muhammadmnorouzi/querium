using Arian.Querium.Abstractions.SQL;
using Arian.Querium.SQLite.Implementations;

namespace Arian.Querium.SQLite.Tests;

public class CreateTableQueryBuilderTests
{
    private readonly ISqlDialect _dialect = new SqliteDialect();

    [Fact]
    public void ShouldBuild_SimpleCreateTableQuery()
    {
        // Arrange
        string expected = "CREATE TABLE users (id INTEGER, name TEXT);";

        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("users")
            .Column("id", ColumnType.Integer)
            .Column("name", ColumnType.Text);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
    }

    [Fact]
    public void ShouldBuild_CreateTableWithIfNotExists()
    {
        // Arrange
        string expected = "CREATE TABLE IF NOT EXISTS users (id INTEGER);";

        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("users")
            .IfNotExists()
            .Column("id", ColumnType.Integer);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
    }

    [Fact]
    public void ShouldBuild_CreateTableWithPrimaryKeyAndNotNull()
    {
        // Arrange
        string expected = "CREATE TABLE products (id INTEGER PRIMARY KEY, name TEXT NOT NULL);";

        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("products")
            .Column("id", ColumnType.Integer, isPrimaryKey: true)
            .Column("name", ColumnType.Text, isNullable: false);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
    }

    [Fact]
    public void ShouldBuild_CreateTableWithAutoIncrement()
    {
        // Arrange
        string expected = "CREATE TABLE logs (id INTEGER PRIMARY KEY AUTOINCREMENT, message TEXT);";

        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("logs")
            .Column("id", ColumnType.Integer, isPrimaryKey: true, autoIncrement: true)
            .Column("message", ColumnType.Text);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
    }

    [Fact]
    public void ShouldBuild_CreateTableWithDefaultValues()
    {
        // Arrange
        string expected = "CREATE TABLE settings (id INTEGER, value INTEGER DEFAULT 1, created_at TEXT DEFAULT '2025-01-01 10:00:00', is_active BOOLEAN DEFAULT 1);";
        DateTime testDate = new DateTime(2025, 1, 1, 10, 0, 0);

        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("settings")
            .Column("id", ColumnType.Integer)
            .Column("value", ColumnType.Integer, defaultValue: 1)
            .Column("created_at", ColumnType.Text, defaultValue: testDate)
            .Column("is_active", ColumnType.Boolean, defaultValue: true);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
    }

    [Fact]
    public void ShouldThrow_WhenTableNameNotSpecified()
    {
        // Arrange
        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .Column("id", ColumnType.Integer);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.Sql);
    }

    [Fact]
    public void ShouldThrow_WhenNoColumnsSpecified()
    {
        // Arrange
        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("empty_table");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.Sql);
    }

    [Fact]
    public void ShouldThrow_WhenColumnNameIsEmpty()
    {
        // Arrange
        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("test_table");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.Column("", ColumnType.Text));
    }

    [Fact]
    public void ShouldCorrectlyFormat_BooleanDefaultValue()
    {
        // Arrange
        string expected = "CREATE TABLE booleans (is_active BOOLEAN DEFAULT 0);";

        ICreateTableQueryBuilder builder = new SqliteCreateTableQueryBuilder()
            .CreateTable("booleans")
            .Column("is_active", ColumnType.Boolean, defaultValue: false);

        // Act & Assert
        Assert.Equal(expected, builder.Sql);
    }
}