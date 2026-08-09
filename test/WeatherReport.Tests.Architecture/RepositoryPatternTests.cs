using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WeatherReport.Tests.Architecture;

/// <summary>
/// Tests to ensure repository pattern and interface placement follow clean architecture principles.
/// </summary>
public class RepositoryPatternTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.WeatherForecast).Assembly,
            typeof(Application.DependencyInjection).Assembly,
            typeof(Infrastructure.DependencyInjection).Assembly,
            System.Reflection.Assembly.Load("WebAPI"))
        .Build();

    [Fact]
    public void Repository_Interfaces_Should_Be_In_Application_Layer()
    {
        var rule = Interfaces()
            .That().HaveNameEndingWith("Repository")
            .Should().NotResideInAssembly(typeof(Infrastructure.DependencyInjection).Assembly.GetName().Name)
            .Because("Repository interfaces define application contracts and should not be in Infrastructure");

        rule.Check(Architecture);
    }

    [Fact]
    public void Repository_Implementations_Should_Be_In_Infrastructure_Layer()
    {
        var rule = Classes()
            .That().HaveNameEndingWith("Repository")
            .And().AreNotAbstract()
            .And().AreNotNested()
            .Should().NotResideInAssembly(typeof(Application.DependencyInjection).Assembly.GetName().Name)
            .Because("Repository implementations are infrastructure concerns and should not be in Application");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Reference_Infrastructure_Repositories()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types()
                .That().ResideInNamespace("Infrastructure.Repositories"))
            .Because("Application should depend on repository interfaces, not implementations");

        rule.Check(Architecture);
    }

    [Fact]
    public void WebAPI_Should_Not_Directly_Reference_Infrastructure_Repositories()
    {
        var rule = Types()
            .That().ResideInNamespace("WebAPI.Features")
            .Should().NotDependOnAny(Types()
                .That().ResideInNamespace("Infrastructure.Repositories"))
            .Because("WebAPI should communicate through Application layer, not directly with repositories");

        rule.Check(Architecture);
    }

    [Fact]
    public void Service_Interfaces_Should_Be_In_Application_Layer()
    {
        var rule = Interfaces()
            .That().ResideInNamespace("Application.Services")
            .Should().HaveNameStartingWith("I")
            .Because("Service interfaces in Application should follow naming conventions")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Reference_Infrastructure_Services()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types()
                .That().ResideInNamespace("Infrastructure.Services"))
            .Because("Application should depend on service interfaces, not implementations");

        rule.Check(Architecture);
    }
}
