using Arian.Quantiq.Domain.Interfaces;
using Arian.Quantiq.Domain.Entities.Identity;
using Arian.Quantiq.Infrastructure.Persistence.EF;
using Arian.Quantiq.Infrastructure.Services;
using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQL.Repositories;
using Arian.Querium.SQLite.Implementations.QueryBuilders;
using Arian.Querium.SQLite.Implementations.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arian.Quantiq.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IQueryBuilderFactory, SqliteQueryBuilderFactory>();
            services.AddScoped<ICreateTableQueryBuilder, SqliteCreateTableQueryBuilder>();
            services.AddScoped<IDeleteQueryBuilder, SqliteDeleteQueryBuilder>();
            services.AddScoped<ISqlDialect, SqliteDialect>();
            services.AddScoped<IInsertQueryBuilder, SqliteInsertQueryBuilder>();
            services.AddScoped<ISelectQueryBuilder, SqliteSelectQueryBuilder>();
            services.AddScoped<ISelectQueryBuilder, SqliteSelectQueryBuilder>();
            services.AddScoped<IUpdateQueryBuilder, SqliteUpdateQueryBuilder>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IDynamicSQLRepository, SQliteDynamicRepository>();

            return services;
        }
    }
}
