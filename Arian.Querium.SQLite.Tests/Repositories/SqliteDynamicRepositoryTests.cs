using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQL.Repositories;
using Arian.Querium.SQLite.Implementations.QueryBuilders;
using Arian.Querium.SQLite.Implementations.Repositories;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Data;

namespace Arian.Querium.SQLite.Tests.Repositories;

// This collection definition is optional but helps Xunit run tests in parallel.
[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

// A fixture to handle database setup and teardown for the entire test collection.
public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }
    public IQueryBuilderFactory QueryBuilderFactory { get; }
    private readonly SqliteConnection _connection;

    public DatabaseFixture()
    {
        // Initialize SQLitePCL provider before any database connections are made.
        Batteries.Init();

        // Use a shared in-memory database for all tests in the collection.
        ConnectionString = "Data Source=:memory:;Mode=Memory;Cache=Shared";
        QueryBuilderFactory = new SqliteQueryBuilderFactory();

        // Open and keep the connection alive for the duration of the test run.
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();

        // Initialize the database with a test table.
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        // Use the persistent connection to execute commands.
        // The 'using' block is no longer needed here as the connection is managed by the fixture.
        ICreateTableQueryBuilder createTableQuery = QueryBuilderFactory.CreateTable()
            .CreateTable("users")
            .IfNotExists()
            .Column("Id", ColumnType.Integer, isPrimaryKey: true, autoIncrement: true)
            .Column("Name", ColumnType.Text, isNullable: false)
            .Column("Age", ColumnType.Integer, isNullable: false);

        using IDbCommand createTableCommand = createTableQuery.ToCommand(_connection);
        createTableCommand.ExecuteNonQuery();
    }

    public void Dispose()
    {
        // Close the persistent connection at the end of the test run.
        _connection.Close();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}

// All tests for this class will use the same DatabaseFixture, but we'll reset the state for each test.
[Collection("Database collection")]
public class SqliteDynamicRepositoryTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private readonly IDynamicSqlRepository _repository;

    public SqliteDynamicRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        // The repository instance is created once per test class instance.
        _repository = new SqliteDynamicRepository(_fixture.ConnectionString, _fixture.QueryBuilderFactory);
    }

    /// <summary>
    /// Clears the test table before each test method runs.
    /// </summary>
    public async Task InitializeAsync()
    {
        IDeleteQueryBuilder deleteQuery = _fixture.QueryBuilderFactory.Delete()
            .Delete("users");

        await using var connection = new SqliteConnection(_fixture.ConnectionString);
        await connection.OpenAsync();
        using IDbCommand cmd = deleteQuery.ToCommand(connection);
        await ((SqliteCommand)cmd).ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Disposed of after each test. Not needed here.
    /// </summary>
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ShouldAdd_AndRetrieve_A_New_Row()
    {
        // Arrange
        string tableName = "users";
        var newUserData = new Dictionary<string, object>
        {
            { "Name", "John Doe" },
            { "Age", 30 }
        };

        // Act
        await _repository.AddAsync(tableName, newUserData);

        // Assert
        // Retrieve all and verify the new entry exists.
        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(tableName);
        Dictionary<string, object> addedUser = Assert.Single(allUsers);
        Assert.Equal("John Doe", addedUser["Name"]);
        Assert.Equal(30L, addedUser["Age"]); // SQLite Integer type often returns long.
    }

    [Fact]
    public async Task ShouldGet_A_Row_By_Id()
    {
        // Arrange
        string tableName = "users";
        string primaryKeyColumn = "Id";
        await _repository.AddAsync(tableName, new Dictionary<string, object> { { "Name", "Jane Smith" }, { "Age", 25 } });

        // Retrieve the added user's Id. We assume Id is the first column and is autoincrementing.
        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(tableName);
        long id = (long)allUsers.First()["Id"];

        // Act
        Dictionary<string, object>? retrievedUser = await _repository.GetByIdAsync(tableName, primaryKeyColumn, id);

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal("Jane Smith", retrievedUser["Name"]);
    }

    [Fact]
    public async Task ShouldUpdate_An_Existing_Row()
    {
        // Arrange
        string tableName = "users";
        string primaryKeyColumn = "Id";
        await _repository.AddAsync(tableName, new Dictionary<string, object> { { "Name", "Old Name" }, { "Age", 99 } });

        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(tableName);
        long idToUpdate = (long)allUsers.First()["Id"];

        var updatedData = new Dictionary<string, object>
        {
            { "Name", "New Name" },
            { "Age", 35 }
        };

        // Act
        await _repository.UpdateAsync(tableName, updatedData, primaryKeyColumn, idToUpdate);

        // Assert
        Dictionary<string, object>? updatedUser = await _repository.GetByIdAsync(tableName, primaryKeyColumn, idToUpdate);
        Assert.NotNull(updatedUser);
        Assert.Equal("New Name", updatedUser["Name"]);
        Assert.Equal(35L, updatedUser["Age"]);
    }

    [Fact]
    public async Task ShouldDelete_A_Row()
    {
        // Arrange
        string tableName = "users";
        string primaryKeyColumn = "Id";
        await _repository.AddAsync(tableName, new Dictionary<string, object> { { "Name", "User to delete" }, { "Age", 10 } });

        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(tableName);
        long idToDelete = (long)allUsers.First()["Id"];

        // Act
        await _repository.DeleteAsync(tableName, primaryKeyColumn, idToDelete);

        // Assert
        Dictionary<string, object>? deletedUser = await _repository.GetByIdAsync(tableName, primaryKeyColumn, idToDelete);
        Assert.Null(deletedUser);

        IEnumerable<Dictionary<string, object>> remainingUsers = await _repository.GetAllAsync(tableName);
        Assert.Empty(remainingUsers);
    }

    [Fact]
    public async Task ShouldReturn_EmptyList_WhenNoRowsExist()
    {
        // Arrange
        string tableName = "users";

        // Act
        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(tableName);

        // Assert
        Assert.Empty(allUsers);
    }
}
