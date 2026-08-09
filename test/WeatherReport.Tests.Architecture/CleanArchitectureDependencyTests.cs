using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace WeatherReport.Tests.Architecture;

/// <summary>
/// Tests to ensure clean architecture layer dependencies are maintained.
/// Domain should not depend on any other layer.
/// Application should depend only on Domain.
/// Infrastructure should depend only on Application (and transitively Domain).
/// WebAPI should depend only on Infrastructure (and transitively Application and Domain).
/// </summary>
public class CleanArchitectureDependencyTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Domain.WeatherForecast).Assembly,
            typeof(Application.DependencyInjection).Assembly,
            typeof(Infrastructure.DependencyInjection).Assembly,
            System.Reflection.Assembly.Load("WebAPI"))
        .Build();

    [Fact]
    public void Domain_Should_Not_Depend_On_Application()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Application"))
            .Because("Domain is the core layer and must not depend on Application");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Infrastructure"))
            .Because("Domain is the core layer and must not depend on Infrastructure");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_WebAPI()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("WebAPI"))
            .Because("Domain is the core layer and must not depend on WebAPI");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Infrastructure"))
            .Because("Application layer must not depend on Infrastructure implementation details");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_WebAPI()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("WebAPI"))
            .Because("Application layer must not depend on WebAPI presentation layer");

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_WebAPI()
    {
        var rule = Types()
            .That().ResideInNamespace("Infrastructure")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("WebAPI"))
            .Because("Infrastructure should not depend on presentation layer");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Have_ASP_NET_Dependencies()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.AspNetCore"))
            .Because("Domain must be framework-agnostic");

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Not_Have_MediatR_Dependencies()
    {
        var rule = Types()
            .That().ResideInNamespace("Domain")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MediatR"))
            .Because("Domain must be framework-agnostic");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Have_ASP_NET_Dependencies()
    {
        var rule = Types()
            .That().ResideInNamespace("Application")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("Microsoft.AspNetCore"))
            .Because("Application layer must not depend on web frameworks");

        rule.Check(Architecture);
    }
}
