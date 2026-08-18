using Application.Behavior;
using Application.Services.SpanEnricher;
using FluentResults;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Application.Tests.Unit.Behaviors;

public sealed class ExceptionHandlingBehaviorTests
{
    private readonly ISpanEnricher _spanEnricher;
    private readonly RequestHandlerDelegate<Result> _next;
    private readonly ExceptionHandlingBehavior<ExceptionHandlingTestRequest, Result> _sut;

    public ExceptionHandlingBehaviorTests()
    {
        _spanEnricher = Substitute.For<ISpanEnricher>();
        _next = Substitute.For<RequestHandlerDelegate<Result>>();
        _sut = new(_spanEnricher);
    }

    [Fact]
    public async Task Handle_WhenNoExceptionIsThrown_ShouldReturnNext()
    {
        // Arrange
        var expected = Result.Ok();
        _next(CancellationToken.None).Returns(expected);

        // Act
        var result = await _sut.Handle(new ExceptionHandlingTestRequest(), _next, CancellationToken.None);

        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task Handle_WhenAnExceptionIsThrown_ShouldReturnInternalError()
    {
        // Arrange
        var exception = new Exception("Test exception message");
        _next(CancellationToken.None).ThrowsAsync(exception);
        var expected = Result.Fail(new Error(exception.Message).CausedBy(exception));

        // Act
        var result = await _sut.Handle(new ExceptionHandlingTestRequest(), _next, CancellationToken.None);

        // Assert
        Assert.Equivalent(expected, result);
        _spanEnricher.Received(1).EnrichWithException(exception);
    }

    public sealed record ExceptionHandlingTestRequest(string ID = "") : IRequest<Result>;
}
