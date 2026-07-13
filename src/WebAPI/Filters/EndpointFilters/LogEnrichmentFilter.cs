namespace WebAPI.Filters.EndpointFilters;

/// <summary>
/// Enriches the logs with HTTP context.
/// </summary>
internal sealed class LogEnrichmentFilter : IEndpointFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LogEnrichmentFilter> _logger;

    public LogEnrichmentFilter(IHttpContextAccessor httpContextAccessor, ILogger<LogEnrichmentFilter> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_httpContextAccessor.HttpContext == null)
            return await next(context);

        var request = _httpContextAccessor.HttpContext.Request;

        var state = new Dictionary<string, object?>()
        {
            { "http.method", request.Method },
            { "http.referer", GetHeader(request.Headers, "Referer") },
            { "http.url_details.host", GetHeader(request.Headers, "x-forwarded-host") ?? request.Host.Host },
            { "http.url_details.path", request.Path.Value },
            { "http.url_details.queryString", request.QueryString },
            { "http.url_details.scheme", request.Scheme },
            { "http.useragent", GetHeader(request.Headers, "User-Agent") },
            { "http.version", request.Protocol?.Replace("HTTP/", string.Empty) },
            { "network.client.ip", GetHeader(request.Headers, "x-forwarded-for") ?? _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() },
        };

        using (_logger.BeginScope(state))
            return await next(context);
    }

    private static string? GetHeader(IHeaderDictionary headers, string key) =>
        headers
            .FirstOrDefault(header => string.Equals(header.Key, key, StringComparison.InvariantCultureIgnoreCase))
            .Value
            .FirstOrDefault();
}
