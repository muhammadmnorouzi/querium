using Arian.Quantiq.Domain.Interfaces;
using Arian.Quantiq.Infrastructure.Persistence.EF;
using Arian.Quantiq.Infrastructure.Services;
using Arian.Querium.SQL.QueryBuilders;
using Arian.Querium.SQL.Repositories;
using Arian.Querium.SQLite.Implementations.QueryBuilders;
using Arian.Querium.SQLite.Implementations.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IQueryBuilderFactory , SqliteQueryBuilderFactory>();
builder.Services.AddScoped<ICreateTableQueryBuilder , SqliteCreateTableQueryBuilder>();
builder.Services.AddScoped<IDeleteQueryBuilder , SqliteDeleteQueryBuilder>();
builder.Services.AddScoped<ISqlDialect , SqliteDialect>();
builder.Services.AddScoped<IInsertQueryBuilder , SqliteInsertQueryBuilder>();
builder.Services.AddScoped<ISelectQueryBuilder , SqliteSelectQueryBuilder>();
builder.Services.AddScoped<ISelectQueryBuilder , SqliteSelectQueryBuilder>();
builder.Services.AddScoped<IUpdateQueryBuilder , SqliteUpdateQueryBuilder>();
builder.Services.AddScoped<IDynamicSQLRepository , SQliteDynamicRepository>();
builder.Services.AddScoped<IUserContextService , UserContextService>();

builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();