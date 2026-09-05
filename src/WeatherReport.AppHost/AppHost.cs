using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("RedisConnection");

var useLocalRedis = string.IsNullOrEmpty(connectionString);

if (useLocalRedis)
{
    var redisReference = builder.AddRedis("RedisConnection")
        .WithRedisInsight();

    builder.AddProject<Projects.WebAPI>("webapi")
        .WithReference(redisReference)
        .WithEnvironment("OTEL_SERVICE_NAME", "weather-report-api");
}
else
{
    var remoteRedis = builder.AddConnectionString("RedisConnection", connectionString);

    builder.AddContainer("redisinsight", "redis/redisinsight", "latest")
        .WithHttpEndpoint(port: 55400, targetPort: 55400, name: "http");

    builder.AddProject<Projects.WebAPI>("webapi")
        .WithReference(remoteRedis)
        .WithEnvironment("OTEL_SERVICE_NAME", "weather-report-api");
}

builder.Build().Run();
