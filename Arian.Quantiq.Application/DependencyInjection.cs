using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arian.Quantiq.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        Assembly[] assemblies = [typeof(DependencyInjection).Assembly];

        _ = services.AddValidatorsFromAssemblies(assemblies);
        _ = services.AddMediatR(options => options.RegisterServicesFromAssemblies(assemblies));

        return services;
    }
}