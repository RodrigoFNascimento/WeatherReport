namespace Domain.Tests.Unit;

public sealed class TemperatureTests
{
    [Fact]
    public void FromCelsius_WhenValidCelsiusDegrees_ShouldReturnTemperature()
    {
        // Arrange
        const double MinimumCelsiusTemperature = -273.15;
        var celsiusDegrees = MinimumCelsiusTemperature + 0.01;

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

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Temperature.FromCelsius(celsiusDegrees));
    }
}
