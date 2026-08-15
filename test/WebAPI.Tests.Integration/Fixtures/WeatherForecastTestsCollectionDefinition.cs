namespace WebAPI.Tests.Integration.Fixtures;

/// <summary>
/// Collection definition for weather forecast tests to prevent parallel execution.
/// </summary>
[CollectionDefinition("Weather Forecast Tests", DisableParallelization = true)]
public sealed class WeatherForecastTestsCollectionDefinition
{
}
