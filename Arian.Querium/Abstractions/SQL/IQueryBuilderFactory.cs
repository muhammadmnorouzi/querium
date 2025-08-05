namespace Arian.Querium.Abstractions.SQL;

/// <summary>
/// Factory interface for creating query builders for different SQL commands.
/// </summary>
public interface IQueryBuilderFactory
{
    /// <summary>
    /// Creates a new SELECT query builder.
    /// </summary>
    ISelectQueryBuilder Select();

    /// <summary>
    /// Creates a new INSERT query builder.
    /// </summary>
    IInsertQueryBuilder Insert();

    /// <summary>
    /// Creates a new UPDATE query builder.
    /// </summary>
    IUpdateQueryBuilder Update();

    /// <summary>
    /// Creates a new DELETE query builder.
    /// </summary>
    IDeleteQueryBuilder Delete();

    /// <summary>
    /// Creates a new CREATE TABLE query builder.
    /// </summary>
    ICreateTableQueryBuilder CreateTable();
}
