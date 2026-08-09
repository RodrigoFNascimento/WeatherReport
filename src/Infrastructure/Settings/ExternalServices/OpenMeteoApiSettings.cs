using Microsoft.Extensions.Http.Resilience;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Settings.ExternalServices;

internal sealed class OpenMeteoApiSettings
{
    public const string ResourceName = "Open Meteo API";
    public const string SectionName = "ExternalServices:OpenMeteo";

    [Url]
    public required string Url { get; init; }
    public required TimeSpan Timeout { get; init; }
    public required HttpStandardResilienceOptions StandardRetry { get; set; }
}
