namespace Domain.Tests.Unit;

public sealed class TemperatureTests
{
    [Fact]
    public void FromCelsius_WhenValidCelsiusDegrees_ShouldReturnTemperature()
    {
        // Arrange
        const double MinimumCelsiusTemperature = -273.15;
        var celsiusDegrees = MinimumCelsiusTemperature;

        // Act
        var result = Temperature.FromCelsius(celsiusDegrees);

        // Assert
        Assert.Equal(celsiusDegrees, result.DegreesCelsius);
    }
    
    [Fact]
    public void FromCelsius_WhenInvalidCelsiusDegrees_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const double MinimumCelsiusTemperature = -273.15;
        var celsiusDegrees = MinimumCelsiusTemperature - 0.01;
        const string ExpectedParameterName = "value";

        // Act
        void act() => Temperature.FromCelsius(celsiusDegrees);

        // Act & Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Equal($"Temperature below absolute zero. (Parameter '{ExpectedParameterName}')", exception.Message);
        Assert.Equal(ExpectedParameterName, exception.ParamName);
    }
}
