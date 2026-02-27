namespace TestProject;

public static class MathService
{
    public static int Pow(int x, int power)
    {
        int product = 1;
        
        for (int i = 1; i <= power; i++)
        {
            product *= x;
        }
        
        return product;
    }

    // log2(8) = 3
    // 2^3 = 8
    public static double Log(double x, int baseX)
    {
        
    }
}