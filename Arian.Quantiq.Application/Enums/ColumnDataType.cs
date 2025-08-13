using System.Text.Json.Serialization;

namespace Arian.Quantiq.Application.Enums;

/// <summary>
/// Defines generic, database-agnostic data types for table columns.
/// This abstraction allows a single model to represent data types across different SQL dialects.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ColumnDataType
{
    /// <summary>
    /// Represents a string-based data type of a specified length. Maps to VARCHAR, NVARCHAR, etc.
    /// </summary>
    String,

    /// <summary>
    /// Represents an integer number. Maps to INT, INTEGER, etc.
    /// </summary>
    Integer,

    /// <summary>
    /// Represents a floating-point decimal number with precision and scale. Maps to DECIMAL, NUMERIC, etc.
    /// </summary>
    Decimal,

    /// <summary>
    /// Represents a date and time value. Maps to DATETIME, TIMESTAMP, etc.
    /// </summary>
    DateTime,

    /// <summary>
    /// Represents a boolean value. Maps to BIT, BOOLEAN, etc.
    /// </summary>
    Boolean
}