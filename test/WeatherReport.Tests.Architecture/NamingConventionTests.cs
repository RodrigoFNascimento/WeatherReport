using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WeatherReport.Tests.Architecture;

/// <summary>
/// Tests to ensure naming conventions are followed across the codebase.
/// </summary>
public class NamingConventionTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.WeatherForecast).Assembly,
            typeof(Application.DependencyInjection).Assembly,
            typeof(Infrastructure.DependencyInjection).Assembly,
            System.Reflection.Assembly.Load("WebAPI"))
        .Build();

    [Fact]
    public void Interfaces_Should_Start_With_I()
    {
        var rule = Interfaces()
            .Should().HaveNameStartingWith("I")
            .Because("Interfaces should follow standard .NET naming conventions");

        rule.Check(Architecture);
    }

    [Fact]
    public void Repository_Interfaces_Should_End_With_Repository()
    {
        var rule = Interfaces()
            .That().ResideInNamespace("Application.Repositories")
            .Should().HaveNameEndingWith("Repository")
            .Because("Repository interfaces should follow naming conventions")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Repository_Implementations_Should_End_With_Repository()
    {
        var rule = Classes()
            .That().ResideInNamespace("Infrastructure.Repositories")
            .And().AreNotAbstract()
            .And().AreNotNested()
            .Should().HaveNameEndingWith("Repository")
            .Because("Repository implementations should follow naming conventions");

        rule.Check(Architecture);
    }

    [Fact]
    public void MediatR_Handlers_Should_End_With_Handler()
    {
        var rule = Classes()
            .That().ResideInNamespace("Application")
            .And().HaveNameContaining("Handler")
            .And().AreNotAbstract()
            .Should().HaveNameEndingWith("Handler")
            .Because("MediatR handlers should follow naming conventions")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Settings_Classes_Should_End_With_Settings()
    {
        var rule = Classes()
            .That().ResideInNamespace("Infrastructure.Settings")
            .Should().HaveNameEndingWith("Settings")
            .Because("Configuration classes should follow naming conventions")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void API_Endpoints_Should_End_With_Endpoint()
    {
        var rule = Classes()
            .That().ResideInNamespace("WebAPI.Features")
            .And().HaveNameContaining("Endpoint")
            .Should().HaveNameEndingWith("Endpoint")
            .Because("API endpoint classes should follow naming conventions")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void HealthChecks_Should_End_With_HealthCheck()
    {
        var rule = Classes()
            .That().ResideInNamespace("Infrastructure.HealthChecks")
            .And().AreNotAbstract()
            .Should().HaveNameEndingWith("HealthCheck")
            .Because("Health check classes should follow naming conventions");

        rule.Check(Architecture);
    }

    [Fact]
    public void MediatR_Behaviors_Should_End_With_Behavior()
    {
        var rule = Classes()
            .That().ResideInNamespace("Application.Behavior")
            .And().AreNotAbstract()
            .And().AreNotNested()
            .And().DoNotHaveNameMatching(@".*`.*")
            .Should().HaveNameEndingWith("Behavior")
            .Because("MediatR pipeline behaviors should follow naming conventions")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }
}
