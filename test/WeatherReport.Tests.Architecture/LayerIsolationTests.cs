using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WeatherReport.Tests.Architecture;

/// <summary>
/// Tests to ensure proper layer isolation and that each layer contains only appropriate concerns.
/// </summary>
public class LayerIsolationTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.WeatherForecast).Assembly,
            typeof(Application.DependencyInjection).Assembly,
            typeof(Infrastructure.DependencyInjection).Assembly,
            System.Reflection.Assembly.Load("WebAPI"))
        .Build();

    [Fact]
    public void Domain_Should_Not_Depend_On_EntityFramework()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.EntityFrameworkCore"))
            .Because("Domain must be persistence-ignorant");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_EntityFramework()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.EntityFrameworkCore"))
            .Because("Application must be persistence-ignorant");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Use_HTTP_Types()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().HaveFullNameContaining("System.Net.Http"))
            .Because("Domain should not have HTTP dependencies");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Use_HTTP_Types()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types().That().HaveFullNameContaining("System.Net.Http"))
            .Because("Application should not have direct HTTP dependencies");

        rule.Check(Architecture);
    }

    [Fact]
    public void Only_WebAPI_Should_Use_AspNetCore_Mvc()
    {
        var rule = Types()
            .That().DependOnAny(Types().That().ResideInNamespace("Microsoft.AspNetCore.Mvc"))
            .Should().ResideInNamespace("WebAPI")
            .Because("Only WebAPI should have MVC dependencies")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Use_Logging()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.Extensions.Logging"))
            .Because("Domain should not have logging infrastructure dependencies");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Use_Configuration()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.Extensions.Configuration"))
            .Because("Domain should not have configuration dependencies");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Use_Configuration()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.Extensions.Configuration"))
            .Because("Application should not depend on configuration infrastructure");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Have_DependencyInjection_Concerns()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.Extensions.DependencyInjection"))
            .Because("Domain should not have DI framework dependencies");

        rule.Check(Architecture);
    }
}
