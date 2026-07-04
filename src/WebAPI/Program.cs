using WeatherReport.ServiceDefaults;
using WebAPI;
using WebAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddWebApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.MapConnectivityEndpoints()
    .NewVersionedApi()
    .ReportApiVersions()
    .MapWeatherEndpoints();

// Configure the HTTP request pipeline.
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
