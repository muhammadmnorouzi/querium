namespace Arian.Querium.Abstractions.SQL;

/// <summary>
/// Represents column data types for CREATE TABLE queries in SQLite.
/// </summary>
public enum ColumnType
{
    /// <summary>
    /// INTEGER type.
    /// </summary>
    Integer,

    /// <summary>
    /// REAL (floating-point) type.
    /// </summary>
    Real,

    /// <summary>
    /// TEXT (string) type.
    /// </summary>
    Text,

    /// <summary>
    /// BLOB (binary) type.
    /// </summary>
    Blob,

    /// <summary>
    /// NUMERIC type (affinity).
    /// </summary>
    Numeric,

    /// <summary>
    /// Boolean type (binary state).
    /// </summary>
    Boolean,
}