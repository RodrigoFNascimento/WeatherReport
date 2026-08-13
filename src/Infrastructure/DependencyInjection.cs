using Application.Repositories.WeatherForecast;
using Application.Services.SpanEnricher;
using Infrastructure.HealthChecks;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings.ExternalServices;
using Microsoft.AspNetCore.OutputCaching.StackExchangeRedis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Polly;
using StackExchange.Redis;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Infrastructure;

public static class DependencyInjection
{
    private const string ReadyHealthCheckTag = "ready";

    /// <summary>
    /// Add infrastructure dependencies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance to be configured.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance for further configuration.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IHostApplicationBuilder hostApplicationBuilder,
        IConfiguration configuration)
    {
        services
            .AddCache(hostApplicationBuilder, configuration)
            .AddTelemetry();

        services.AddOpenMeteoApi(
            hostApplicationBuilder.Services.AddHealthChecks());

        return services;
    }

    private static IServiceCollection AddCache(
        this IServiceCollection services,
        IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        var aspireRedisSection = configuration.GetSection("Aspire:StackExchange:Redis");

        builder.AddRedisClient("RedisConnection");

        services.Configure<ConfigurationOptions>(options =>
        {
            var certPath = aspireRedisSection.GetValue<string>("LocalCertificatePath");

            if (!string.IsNullOrEmpty(certPath))
            {
                options.ConfigureSSL(certPath, options.Password ?? string.Empty);
            }
        });

        builder.AddRedisDistributedCache("RedisConnection");
        builder.AddRedisOutputCache("RedisConnection");

        services.Configure<RedisCacheOptions>(
            aspireRedisSection);

        services.Configure<RedisOutputCacheOptions>(
            aspireRedisSection);

        return services;
    }

    private static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddRedisInstrumentation());

        services.AddSingleton<ISpanEnricher, SpanEnricher>();

        return services;
    }

    private static void ConfigureSSL(
        this ConfigurationOptions options,
        string certificate,
        string password)
    {
        options.Ssl = true;
        options.SslProtocols = SslProtocols.Tls12;
        options.CertificateSelection += delegate
        {
            if (string.IsNullOrWhiteSpace(certificate))
                throw new ArgumentNullException(
                    nameof(certificate),
                    "A valid certificate must be informed.");

            if (certificate.Contains(".pfx", StringComparison.InvariantCultureIgnoreCase))
            {
                return X509CertificateLoader.LoadPkcs12FromFile(
                    certificate,
                    password,
                    X509KeyStorageFlags.DefaultKeySet);
            }

            if (certificate.Contains(".pem", StringComparison.InvariantCultureIgnoreCase))
            {
                return X509CertificateLoader.LoadCertificateFromFile(certificate);
            }

            throw new ArgumentException(
                $"Invalid certificate extension: {certificate}",
                nameof(certificate));
        };
        options.CertificateValidation += delegate (
            object sender,
            X509Certificate? certificate,
            X509Chain? chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return certificate?.Subject != null;
        };
    }

    private static IServiceCollection AddOpenMeteoApi(
        this IServiceCollection services,
        IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddOptions<OpenMeteoApiSettings>()
            .BindConfiguration(OpenMeteoApiSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddHttpClient<IWeatherForecastRepository, OpenMeteoApiRepository>((serviceProvider, httpClient) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<OpenMeteoApiSettings>>().Value;

                httpClient.BaseAddress = new(settings.Url);
                httpClient.Timeout = Timeout.InfiniteTimeSpan;
            })
            .AddResilienceHandler(
                "Open-Meteo-Standard-Pipeline",
                static (builder, context) =>
                {
                    context.EnableReloads<OpenMeteoApiSettings>();

                    var apiSettings = context.GetOptions<OpenMeteoApiSettings>();

                    builder.AddTimeout(apiSettings.StandardRetry.TotalRequestTimeout);

                    apiSettings.StandardRetry.CircuitBreaker.ShouldHandle = static args =>
                        ValueTask.FromResult(args is
                        {
                            Outcome.Result.StatusCode: HttpStatusCode.InternalServerError
                        } or
                        {
                            Outcome.Result.StatusCode: HttpStatusCode.GatewayTimeout
                        });

                    builder.AddCircuitBreaker(apiSettings.StandardRetry.CircuitBreaker);

                    apiSettings.StandardRetry.Retry.ShouldHandle = static args =>
                        ValueTask.FromResult(args is
                        {
                            Outcome.Result.StatusCode: HttpStatusCode.InternalServerError
                        } or
                        {
                            Outcome.Result.StatusCode: HttpStatusCode.GatewayTimeout
                        });

                    builder.AddRetry(apiSettings.StandardRetry.Retry);

                    builder.AddTimeout(apiSettings.StandardRetry.AttemptTimeout);
                });

        healthChecksBuilder.AddCheck<OpenMeteoApiHealthCheck>(
            OpenMeteoApiSettings.SectionName,
            tags: [ReadyHealthCheckTag]);

        return services;
    }
}
