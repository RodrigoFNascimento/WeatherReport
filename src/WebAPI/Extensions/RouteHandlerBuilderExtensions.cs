using WebAPI.Filters.EndpointFilters;

namespace WebAPI.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Enriches the logs with HTTP context.
    /// </summary>
    /// <param name="builder">The <see cref="RouteHandlerBuilder"/> instance that should be configured.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> instance for further configuration.</returns>
    public static RouteHandlerBuilder EnrichLogs(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<LogEnrichmentFilter>();
}
