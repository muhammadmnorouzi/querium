using Arian.Quantiq.Domain.Interfaces;
using Arian.Quantiq.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
                                UserManager<ApplicationUser> userManager) : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    /// <inheritdoc />
    public Task<string> GetUserIdAsync()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        string userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        return Task.FromResult(userId);
    }

    /// <inheritdoc />
    public async Task<string> GetUserConnectionStringAsync()
    {
        string userId = await GetUserIdAsync();
        if (string.IsNullOrEmpty(userId)) return string.Empty;
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        return user?.ConnectionString ?? string.Empty;
    }
}
