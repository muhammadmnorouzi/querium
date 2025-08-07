using Arian.Querium.SQL.QueryBuilders;

namespace Arian.Querium.SQLite.Implementations.QueryBuilders;

/// <summary>
/// Factory for creating SQLite query builders.
/// </summary>
public class SqliteQueryBuilderFactory : IQueryBuilderFactory
{
    public ISelectQueryBuilder Select() => new SqliteSelectQueryBuilder();
    public IInsertQueryBuilder Insert() => new SqliteInsertQueryBuilder();
    public IUpdateQueryBuilder Update() => new SqliteUpdateQueryBuilder();
    public IDeleteQueryBuilder Delete() => new SqliteDeleteQueryBuilder();
    public ICreateTableQueryBuilder CreateTable() => new SqliteCreateTableQueryBuilder();
}