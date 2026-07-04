namespace WebAPI.Endpoints;

internal static class ConnectivityEndpoints
{
    /// <summary>
    /// Adds endpoints to check the network connectivity with this web API.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> the endpoints will be added to.</param>
    /// <returns>The configured <see cref="IEndpointRouteBuilder"/> for further customization.</returns>
    public static IEndpointRouteBuilder MapConnectivityEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("ping", () => Results.NoContent())
            .WithName("Ping")
            .WithSummary("Checks the network connectivity.")
            .WithDescription("Used by client applications to test if they can reach this web API without triggering heavy internal dependency checks.")
            .Produces(StatusCodes.Status204NoContent);

        return builder;
    }
}
