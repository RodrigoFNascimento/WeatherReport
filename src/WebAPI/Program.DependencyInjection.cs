using Asp.Versioning;
using FluentResults;
using FluentResults.HttpMapping;
using Microsoft.OpenApi;
using System.Net;
using System.Net.Mime;

namespace WebAPI;

internal static partial class Program
{
    /// <summary>
    /// Add presentation layer dependencies.
    /// </summary>
    /// <param name="services">The instance of <see cref="IServiceCollection"/> to be configured.</param>
    /// <returns>The configured instance of <see cref="IServiceCollection"/> for further configuration.</returns>
    public static IServiceCollection AddWebApi(this IServiceCollection services) =>
        services
        .AddDocumentation()
        .AddHttpContextAccessor()
        .AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.Remove("exception");
            };
        })
        .AddResultMapping();

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

    private static IServiceCollection AddResultMapping(this IServiceCollection services) =>
        services.AddHttpResultMapping(mapper =>
        {
            mapper
                .WhenFailure()
                .WithHeader("cache-control", new StringValues(["no-cache", "no-store"]))
                .WithHeader("expires", "-1")
                .WithHeader("pragma", "no-cache")
                .Problem(p => p
                    .WithStatus(HttpStatusCode.InternalServerError)
                    .WithTitle("Unexpected internal error.")
                    .WithDetail("An unexpected internal error occurred. Try again later."));

            mapper
                .When(ctx => ctx.Result is Result<Stream>)
                .Map(ctx => Results.File((ctx.Result as Result<Stream>)!.Value, MediaTypeNames.Application.Pdf));
        });
}
