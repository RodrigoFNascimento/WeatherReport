using Application.Behavior;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    /// <summary>
    /// Adds application dependencies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to be configured.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance for further configuration.</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection))
                .AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        return services;
    }
}
