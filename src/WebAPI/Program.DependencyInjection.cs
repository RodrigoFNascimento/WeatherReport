using Asp.Versioning;
using Microsoft.OpenApi;

namespace WebAPI;

internal static partial class Program
{
    /// <summary>
    /// Add presentation layer dependencies.
    /// </summary>
    /// <param name="services">The instance of <see cref="IServiceCollection"/> to be configured.</param>
    /// <returns>The configured instance of <see cref="IServiceCollection"/> for further configuration.</returns>
    public static IServiceCollection AddWebApi(this IServiceCollection services) =>
        services.AddDocumentation();

    private static IServiceCollection AddDocumentation(
        this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "Weather report API",
                        Description = "This is meant to be a study on how to build web APIs.",
                        Version = "1.0.0"
                    };

                    return Task.CompletedTask;
                });
            })
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
