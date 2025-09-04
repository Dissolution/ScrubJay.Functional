using System.Diagnostics;
using ScrubJay.Functional;


Result<double> Parse(string input) =>
    Result.Try(() => double.Parse(input));

Result<double> Divide(double x, double y) =>
    Result.Try(() => x / y);

async Result<double> Do(string a, string b)
{
    var x = await Parse(a);
    var y = await Parse(b);
    await Task.Delay(100);
    Console.WriteLine("Successfully parsed inputs");
    return await Divide(x, y);
}

async Result<double> Do2(string str)
{
    return await Parse(str);
}

// Usage
var output =
    from x in Parse("147")
    from y in Parse("13")
    from z in Divide(x,y)
    select new { F64 = x };



Console.WriteLine((object)output);
Debugger.Break();
return;


namespace ScrubJay.Functional.ConsoleApp
{
    // static class Testing
    // {
    //     public static async Scratch.Result<(int, string)> GetThingAsync()
    //     {
    //         await Task.Delay(100);
    //         return await Scratch.Result<(int, string)>.Ok((147, "TJ"));
    //     }
    // }
}