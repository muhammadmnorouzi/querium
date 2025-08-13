using Arian.Quantiq.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Arian.Quantiq.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="IUserContextService"/> to provide user ID and connection string.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserContextService"/> class.
/// </remarks>
/// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
/// <param name="userManager">Manages user-related operations.</param>
public class UserContextService(IHttpContextAccessor httpContextAccessor,
                                IConfiguration configurationManager) : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc />
    public Task<string> GetUserIdAsync()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        string userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        return Task.FromResult(userId);
    }

    /// <inheritdoc />
    public Task<string?> GetUserConnectionString()
    {
        return Task.FromResult(configurationManager.GetConnectionString("UserDefaultConnection"));
    }
}