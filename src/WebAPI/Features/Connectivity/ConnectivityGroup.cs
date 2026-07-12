using WebAPI.Features.Connectivity.Ping;

namespace WebAPI.Features.Connectivity;

internal static class ConnectivityGroup
{
    /// <summary>
    /// Maps the connectivity endpoints.
    /// </summary>
    /// <param name="app">The instance of <see cref="IEndpointRouteBuilder"/> the endpoints will be mapped to.</param>
    /// <returns>The instance of <see cref="IEndpointRouteBuilder"/> for further configuration.</returns>
    public static IEndpointRouteBuilder MapConnectivityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("")
            .WithTags("Connectivity");

        new PingEndpoint().MapEndpoint(group);

        return app;
    }
}
