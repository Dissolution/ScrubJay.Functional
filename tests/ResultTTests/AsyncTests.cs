namespace ScrubJay.Functional.Tests.ResultTTests;

public class AsyncTests
{
    /*
    private async Result<double> ParseDoubleAsync(string? str)
    {
        await Task.Delay(1);
        var result = double.Parse(str!);
        await Task.Delay(1);
        return result;
    }

    private async Result<double> DivideAsync(double numerator, double denominator)
    {
        await Task.Delay(1);
        var result = numerator / denominator;
        await Task.Delay(1);
        return result;
    }

    private async Result<double> DoAsync(string? numerator, string? denominator)
    {
        await Task.Delay(1);
        var n = await ParseDoubleAsync(numerator);
        await Task.Delay(1);
        var d = await ParseDoubleAsync(denominator);
        await Task.Delay(1);
        var r = await DivideAsync(n, d);
        await Task.Delay(1);
        return r;
    }
    */
    
    
    private static Result<double> Parse(string input) =>
        Result.Try(() => double.Parse(input));

    private static async Result<double> ParseAsync(string input) => double.Parse(input);

    private static Result<double> Divide(double x, double y) =>
        Result.Try(() => x / y);

    private static async Result<double> DivideAsync(double x, double y) => (x / y);

    private static async Result<double> Do(string a, string b)
    {
        await Task.Delay(10);
        var x = await ParseAsync(a);
        await Task.Delay(10);
        var y = await ParseAsync(b);
        await Task.Delay(10);
        //Console.WriteLine("Successfully parsed inputs");
        var result = await DivideAsync(x, y);
        await Task.Delay(10);
        return result;
    }
    
    [Fact]
    public async Task SimpleAwaitWorks()
    {
        Result<double> result = Do("147", "13");
        Assert.True(result.IsOk(out var f64));
        Assert.Equal(147d / 13d, f64);
        
        result = Do("TRJ", "13");
        Assert.True(result.IsError());
        
        result = Do("147", "TRJ");
        Assert.True(result.IsError());

        result = Do("TRJ", "TRJ");
        Assert.True(result.IsError());
        
        
        /*
        Result<double> result = DoAsync("147", "13");
        Assert.True(result.IsOk(out var f64));
        Assert.Equal(147d / 13d, f64);
        
        result = DoAsync("TRJ", "13");
        Assert.True(result.IsError());
        
        result = DoAsync("147", "TRJ");
        Assert.True(result.IsError());

        result = DoAsync("TRJ", "TRJ");
        Assert.True(result.IsError());
        */
    }
}