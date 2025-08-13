namespace Arian.Quantiq.Domain.Interfaces;

/// <summary>
/// Provides methods to retrieve the current user's ID and connection string.
/// </summary>
public interface IUserContextService
{
    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    /// <returns>A task that returns the user ID as a string.</returns>
    Task<string> GetUserIdAsync();

    /// <summary>
    /// Gets the current user's connection string.
    /// </summary>
    /// <returns>A task that returns the user's connection string or null if could not be found.</returns>
    Task<string?> GetUserConnectionString();
}