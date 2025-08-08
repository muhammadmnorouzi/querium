using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQLite.Implementations.QueryBuilders;
using Arian.Querium.SQLite.Implementations.Repositories;
using Microsoft.Data.Sqlite;
using SQLitePCL;

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

        // The initial test table is no longer created here.
        // It will be created and dropped as part of the individual tests.
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
    private readonly SQliteDynamicRepository _repository;
    private const string TestTableName = "users";

    public SqliteDynamicRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new SQliteDynamicRepository(_fixture.ConnectionString, _fixture.QueryBuilderFactory);
    }

    /// <summary>
    /// Creates a test table before each test method runs.
    /// </summary>
    public async Task InitializeAsync()
    {
        Dictionary<string, ColumnType> columns = new()
        {
            { "Id", ColumnType.Integer },
            { "Name", ColumnType.Text },
            { "Age", ColumnType.Integer },
            { "IsActive", ColumnType.Boolean }
        };
        await _repository.CreateTableAsync(TestTableName, columns);
    }

    /// <summary>
    /// Deletes the test table after each test.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _repository.DeleteTableAsync(TestTableName);
    }

    [Fact]
    public async Task ShouldAdd_AndRetrieve_A_New_Row()
    {
        // Arrange
        Dictionary<string, object> newUserData = new()
        {
            { "Name", "John Doe" },
            { "Age", 30 },
            { "IsActive", true }
        };

        // Act
        await _repository.AddAsync(TestTableName, newUserData);

        // Assert
        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(TestTableName);
        Dictionary<string, object> addedUser = Assert.Single(allUsers);
        Assert.Equal("John Doe", addedUser["Name"]);
        Assert.Equal(30L, addedUser["Age"]);
        Assert.Equal(1L, addedUser["IsActive"]);
    }

    [Fact]
    public async Task ShouldGet_A_Row_By_Id()
    {
        // Arrange
        string primaryKeyColumn = "Id";
        await _repository.AddAsync(TestTableName, new Dictionary<string, object> { { "Name", "Jane Smith" }, { "Age", 25 }, { "IsActive", false } });

        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(TestTableName);
        Assert.NotEmpty(allUsers);
        long id = (long)allUsers.First()["Id"];

        // Act
        Dictionary<string, object>? retrievedUser = await _repository.GetByIdAsync(TestTableName, primaryKeyColumn, id);

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal("Jane Smith", retrievedUser["Name"]);
        Assert.Equal(0L, retrievedUser["IsActive"]);
    }

    [Fact]
    public async Task ShouldUpdate_An_Existing_Row()
    {
        // Arrange
        string primaryKeyColumn = "Id";
        await _repository.AddAsync(TestTableName, new Dictionary<string, object> { { "Name", "Old Name" }, { "Age", 99 }, { "IsActive", true } });

        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(TestTableName);
        Assert.NotEmpty(allUsers);
        long idToUpdate = (long)allUsers.First()["Id"];

        Dictionary<string, object> updatedData = new()
        {
            { "Name", "New Name" },
            { "Age", 35 }
        };

        // Act
        await _repository.UpdateAsync(TestTableName, updatedData, primaryKeyColumn, idToUpdate);

        // Assert
        Dictionary<string, object>? updatedUser = await _repository.GetByIdAsync(TestTableName, primaryKeyColumn, idToUpdate);
        Assert.NotNull(updatedUser);
        Assert.Equal("New Name", updatedUser["Name"]);
        Assert.Equal(35L, updatedUser["Age"]);
        Assert.Equal(1L, updatedUser["IsActive"]); // This value should not have changed
    }

    [Fact]
    public async Task ShouldDelete_A_Row()
    {
        // Arrange
        string primaryKeyColumn = "Id";
        await _repository.AddAsync(TestTableName, new Dictionary<string, object> { { "Name", "User to delete" }, { "Age", 10 }, { "IsActive", true } });

        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(TestTableName);
        Assert.NotEmpty(allUsers);
        long idToDelete = (long)allUsers.First()["Id"];

        // Act
        await _repository.DeleteAsync(TestTableName, primaryKeyColumn, idToDelete);

        // Assert
        Dictionary<string, object>? deletedUser = await _repository.GetByIdAsync(TestTableName, primaryKeyColumn, idToDelete);
        Assert.Null(deletedUser);
    }

    // --- New Test Cases for Table Management and Edge Cases ---

    [Fact]
    public async Task ShouldCreate_A_Table_Successfully()
    {
        // Arrange is handled by InitializeAsync, which creates the "users" table.
        // We will test creating a new, different table here.
        const string newTableName = "products";
        Dictionary<string, ColumnType> columns = new()
        {
            { "Id", ColumnType.Integer },
            { "ProductName", ColumnType.Text },
        };

        // Act
        await _repository.CreateTableAsync(newTableName, columns);
        await _repository.AddAsync(newTableName, new Dictionary<string, object> { { "ProductName", "Laptop" } });

        // Assert
        IEnumerable<Dictionary<string, object>> products = await _repository.GetAllAsync(newTableName);
        Assert.Single(products);

        // Clean up the new table manually, since DisposeAsync only cleans the main one.
        await _repository.DeleteTableAsync(newTableName);
    }

    [Fact]
    public async Task ShouldRename_A_Table_Successfully()
    {
        // Arrange
        await _repository.AddAsync(TestTableName, new Dictionary<string, object> { { "Name", "Renamed User" }, { "Age", 42 } });
        const string newTableName = "new_users";

        // Act
        await _repository.RenameTableAsync(TestTableName, newTableName);

        // Assert
        IEnumerable<Dictionary<string, object>> renamedUsers = await _repository.GetAllAsync(newTableName);
        Assert.Single(renamedUsers);

        // We cannot use the old table name anymore.
        await Assert.ThrowsAsync<SqliteException>(() => _repository.GetAllAsync(TestTableName));

        // Clean up the new table manually.
        await _repository.DeleteTableAsync(newTableName);
    }

    [Fact]
    public async Task ShouldDelete_A_Table_Successfully()
    {
        // Arrange
        // The table is created by InitializeAsync

        // Act
        await _repository.DeleteTableAsync(TestTableName);

        // Assert
        await Assert.ThrowsAsync<SqliteException>(() => _repository.GetAllAsync(TestTableName));
    }

    [Fact]
    public async Task ShouldReturn_Null_When_GetById_DoesNotExist()
    {
        // Arrange
        await _repository.AddAsync(TestTableName, new Dictionary<string, object> { { "Name", "Existing User" }, { "Age", 100 } });

        // Act
        Dictionary<string, object>? nonExistentUser = await _repository.GetByIdAsync(TestTableName, "Id", 999);

        // Assert
        Assert.Null(nonExistentUser);
    }

    [Fact]
    public async Task ShouldNotUpdate_A_NonExistent_Row()
    {
        // Arrange
        Dictionary<string, object> updatedData = new() { { "Name", "Updated Name" } };

        // Act
        await _repository.UpdateAsync(TestTableName, updatedData, "Id", 999);

        // Assert
        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(TestTableName);
        Assert.Empty(allUsers);
    }

    [Fact]
    public async Task ShouldNotDelete_A_NonExistent_Row()
    {
        // Arrange
        await _repository.AddAsync(TestTableName, new Dictionary<string, object> { { "Name", "Existing User" }, { "Age", 100 } });

        // Act
        await _repository.DeleteAsync(TestTableName, "Id", 999);

        // Assert
        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(TestTableName);
        Assert.Single(allUsers);
    }

    [Fact]
    public async Task ShouldHandle_Empty_Table()
    {
        // Arrange
        // The table is created by InitializeAsync, but is empty.

        // Act
        IEnumerable<Dictionary<string, object>> allUsers = await _repository.GetAllAsync(TestTableName);

        // Assert
        Assert.Empty(allUsers);
    }
}
