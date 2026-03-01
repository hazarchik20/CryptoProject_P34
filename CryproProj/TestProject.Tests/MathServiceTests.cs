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

    [Theory]
    [InlineData(8, 2, 3)]
    [InlineData(25, 5, 2)]
    [InlineData(20, 5, 1.86)]
    public void Log_PositiveNumberPositiveBase_CorrectResult(int number, int baseX, double expected)
    {
        var result = MathService.Log(number, baseX);
        
        Assert.Equal(expected, result, precision: 2);
    }

    [Fact]
    public void Log_ZeroBase_NanValue()
    {
        var result = MathService.Log(100, 0);
        
        Assert.True(double.IsNaN(result));
    }
    
    [Fact]
    public void Log_NumberOneZeroBase_ZeroValue()
    {
        var result = MathService.Log(1, 0);
        
        Assert.Equal(0, result);
    }
    
    [Theory]
    [InlineData(8, 2, 4)]
    [InlineData(25, 5, 5)]
    [InlineData(20, 5, 4)]
    public void Divide_AnyNumbers_CorrectResult(int x, int y, double expected)
    {
        var result = MathService.Divide(x, y);
        
        Assert.Equal(expected, result, precision: 2);
    }

    [Fact]
    public void Divide_Zero_DivideByZeroException()
    {
        Assert.Throws<DivideByZeroException>(() => MathService.Divide(10, 0));
    }
}