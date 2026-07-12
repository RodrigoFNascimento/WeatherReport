namespace WebAPI.Features;

/// <summary>
/// Defines an endpoint.
/// </summary>
internal interface IEndpointDefinition
{
    /// <summary>
    /// Maps an endpoint.
    /// </summary>
    /// <param name="app">The instance of <see cref="IEndpointRouteBuilder"/> the endpoint will be mapped to.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
