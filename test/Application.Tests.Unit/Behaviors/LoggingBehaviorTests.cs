using Application.Behavior;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.Tests.Unit.Behaviors;

public sealed class LoggingBehaviorTests
{
    const string LogMethodName = "Log";
    private readonly ILogger<LoggingBehavior<LoggingTestRequest, Result>> _logger;
    private readonly RequestHandlerDelegate<Result> _next;
    private readonly LoggingBehavior<LoggingTestRequest, Result> _sut;

    public LoggingBehaviorTests()
    {
        _logger = Substitute.For<ILogger<LoggingBehavior<LoggingTestRequest, Result>>>();
        _next = Substitute.For<RequestHandlerDelegate<Result>>();
        _sut = new(_logger);
    }

    [Fact]
    public async Task Handle_WhenResponseIsSuccess_ShouldNotLog()
    {
        // Arrange
        var response = Result.Ok();

        _next(CancellationToken.None).Returns(response);

        // Act
        var result = await _sut.Handle(new(), _next, CancellationToken.None);

        // Assert
        Assert.Equivalent(response, result);

        Assert.DoesNotContain(_logger.ReceivedCalls(), x => x.GetMethodInfo().Name == LogMethodName);
    }

    [Fact]
    public async Task Handle_WhenResultHasExceptionalError_ShouldEnrichLogWithExceptionData()
    {
        // Arrange
        var request = new LoggingTestRequest();
        var exception = new Exception();
        var response = Result.Fail(new Error(string.Empty).CausedBy(exception));

        _next(CancellationToken.None).Returns(response);

        Dictionary<string, object?> properties = [];
        _logger.BeginScope(Arg.Do<Dictionary<string, object?>>(x => properties = x));

        // Act
        await _sut.Handle(request, _next, CancellationToken.None);

        // Assert
        Assert.Contains("error.message", properties.Keys);
        Assert.Equivalent(exception.Message, properties["error.message"]);

        Assert.Contains("error.kind", properties.Keys);
        Assert.Equivalent(exception.GetType().Name, properties["error.kind"]);

        Assert.Contains("error.stack", properties.Keys);
        Assert.Equivalent(exception.StackTrace, properties["error.stack"]);
    }

    [Fact]
    public async Task Handle_WhenResponseHasUnexpectedError_ShouldLogError()
    {
        // Arrange
        var request = new LoggingTestRequest();
        var errorMessage = "Error message";
        var response = Result.Fail(new Error(errorMessage));

        _next(CancellationToken.None).Returns(response);

        Dictionary<string, object?> properties = [];
        _logger.BeginScope(Arg.Do<Dictionary<string, object?>>(x => properties = x));

        // Act
        var result = await _sut.Handle(request, _next, CancellationToken.None);

        // Assert
        Assert.Equivalent(response, result);
        Assert.Equivalent(request, properties["@request"]);
        Assert.Equivalent(response, properties["@result"]);

        Assert.Contains(
            _logger.ReceivedCalls(),
            x => x.GetMethodInfo().Name == LogMethodName
                && (LogLevel)x.GetArguments()[0]! == LogLevel.Error
                && x.GetArguments()[2]!.ToString() == errorMessage);
    }

    public sealed record LoggingTestRequest(int ID = 0) : IRequest<Result>;
}
