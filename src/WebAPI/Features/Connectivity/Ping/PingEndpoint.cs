namespace WebAPI.Features.Connectivity.Ping;

internal sealed class PingEndpoint : IEndpointDefinition
{
    /// <summary>
    /// Maps the ping endpoint.
    /// </summary>
    /// <param name="app">The instance of <see cref="IEndpointRouteBuilder"/> the endpoint will be mapped to.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("ping", () => Results.NoContent())
            .WithName("Ping")
            .WithSummary("Checks the network connectivity.")
            .WithDescription("Used by client applications to test if they can reach this web API without triggering heavy internal dependency checks.")
            .Produces(StatusCodes.Status204NoContent);
    }
}
