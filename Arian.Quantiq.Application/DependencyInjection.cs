using Arian.Quantiq.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arian.Quantiq.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        Assembly[] assemblies = [typeof(DependencyInjection).Assembly];

        services.AddValidatorsFromAssemblies(assemblies);
        services.AddMediatR(options => options.RegisterServicesFromAssemblies(assemblies));

       return services;
    }
}


