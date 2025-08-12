using Arian.Quantiq.Application.Interfaces;
using Arian.Quantiq.Domain.Entities.Identity;
using Arian.Quantiq.Domain.Interfaces;
using Arian.Quantiq.Infrastructure.Persistence.EF;
using Arian.Quantiq.Infrastructure.Services;
using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arian.Quantiq.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        _ = services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

        _ = services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        _ = services.AddScoped<IUserContextService, UserContextService>();
        _ = services.AddScoped<ISQLTableManager, SqlServerTableManager>();
        _ = services.AddScoped<IDatabaseCompiler, SqlServerCompiler>();

        _ = services.AddScoped<IEmailSender, NullEmailSender>();
        return services;
    }
}
