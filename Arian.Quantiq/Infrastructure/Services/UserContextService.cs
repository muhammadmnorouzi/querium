using Arian.Quantiq.Domain.Entities.Identity;
using Arian.Quantiq.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Arian.Quantiq.Infrastructure.Services
{
    /// <summary>
    /// Implementation of <see cref="IUserContextService"/> to provide user ID and connection string.
    /// </summary>
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserContextService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="userManager">Manages user-related operations.</param>
        public UserContextService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        /// <inheritdoc />
        public Task<string> GetUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            return Task.FromResult(userId);
        }

        /// <inheritdoc />
        public async Task<string> GetUserConnectionStringAsync()
        {
            var userId = await GetUserIdAsync();
            if (string.IsNullOrEmpty(userId)) return string.Empty;
            var user = await _userManager.FindByIdAsync(userId);
            return user?.ConnectionString ?? string.Empty;
        }
    }
}
