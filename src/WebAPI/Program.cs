using Application;
using Infrastructure;
using WeatherReport.ServiceDefaults;
using WebAPI;
using WebAPI.Features.Connectivity;
using WebAPI.Features.WeatherForecast;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder, builder.Configuration)
    .AddWebApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();
app.UseOutputCache();

app.MapConnectivityEndpoints()
    .MapWeatherForecastEndpoints();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        foreach (var groupName in app.DescribeApiVersions()
            .Select(description => description.GroupName))
        {
            options.SwaggerEndpoint(
                $"/openapi/{groupName}.json",
                groupName);
        }
    });
}

app.Run();
