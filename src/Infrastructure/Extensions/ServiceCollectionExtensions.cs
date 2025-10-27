using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Services;
using HR.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Infrastructure.Extensions;

/// <summary>
///     Extension methods for configuring infrastructure and application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers application services and repository implementations with the provided service collection.
    /// </summary>
    public static IServiceCollection AddHrPlatform(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }
}
