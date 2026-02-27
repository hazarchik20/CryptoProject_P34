namespace TestProject.Tests;

public class MathServiceTests
{
    //Naming
    // MethodName_Input_ExpectedOutput
    
    [Fact]
    public void Pow_PositiveNumberPositivePower_CorrectResult()
    {
        // Assign
        var number = 3;
        var power = 2;

        // Act
        var result = MathService.Pow(number, power); 

        // Assert
        Assert.Equal(9, result);
    }
    
    [Theory]
    [InlineData(3, 2, 9)]
    [InlineData(5, 3, 125)]
    [InlineData(6, 1, 6)]
    [InlineData(7, 0, 1)]
    [InlineData(2, 2, 4)]
    [InlineData(3, -1, 0.33333)]
    public void Pow_PositiveNumberPositivePower_CorrectResultV2(int number, int power, int expected)
    {
        // Act
        var result = MathService.Pow(number, power); 

        // Assert
        Assert.Equal(expected, result);
    }
}