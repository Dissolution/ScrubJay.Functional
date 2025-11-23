using System.Reflection;

namespace ScrubJay.Functional.Tests.ResultTTests;

public class OperatorTests
{
    public static MiscTheoryData TestValues => CommonTheoryData.TestOkValues;
    public static MiscTheoryData TestExceptions => CommonTheoryData.TestExceptions;
    public static MiscTheoryData TestResults => CommonTheoryData.TestResultTs;
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void ImplicitCastToBoolWorks<T>(Result<T> result)
    {
        bool boolean = result;
        Assert.Equal(boolean, result.IsOk());
    }

    [Theory]
    [MemberData(nameof(TestResults))]
    public void ImplicitCastToResultWorks<T>(Result<T> resultT)
    {
        Result result = resultT;
        Assert.Equal(resultT.IsOk(), result.IsOk());
        if (resultT.IsError(out var ex))
        {
            Assert.True(result.IsError(out var ex2));
            Assert.Equal(ex, ex2);
        }
    }
    
    [Theory]
    [MemberData(nameof(TestValues))]
    public void ImplicitFromOkValueWorks<T>(T ok)
    {
        Result<T> result = ok;
        Assert.True(result.IsOk(out var value));
        Assert.Equal(ok, value);
    }

    [Theory]
    [MemberData(nameof(TestExceptions))]
    public void ImplicitFromExceptionWorks<E>(E error)
        where E : Exception
    {
        Result<int> result = error;
        Assert.True(result.IsError(out var ex));
        Assert.NotNull(ex);
        Assert.Equal(typeof(E), ex.GetType());
    }
    
    [Theory]
    [MemberData(nameof(TestValues))]
    public void ImplicitFromOkStructWorks<T>(T ok)
    {
        IMPL.Ok<T> implok = new(ok);
        Result<T> result = implok;
        Assert.True(result.IsOk(out var value));
        Assert.Equal(ok, value);
    }

    [Theory]
    [MemberData(nameof(TestExceptions))]
    public void ImplicitFromErrorStructWorks<E>(E error)
        where E : Exception
    {
        IMPL.Error<Exception> implerror = new(error);
        Result<int> result = implerror;
        Assert.True(result.IsError(out var ex));
        Assert.NotNull(ex);
        Assert.Equal(typeof(E), ex.GetType());
    }

    [Theory]
    [MemberData(nameof(TestResults))]
    public void ImplicitTrueWorks<T>(Result<T> result)
    {
        if (result)
        {
            Assert.True(result.IsOk());
        }
        else
        {
            Assert.True(result.IsError());
        }
    }
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void ImplicitFalseWorks<T>(Result<T> result)
    {
        // there is no way to directly check for this operator
        // as we have other operators that will take precedence
        // if we try to do `if (!result)`

        var opMethod = typeof(Result<T>).GetMethod("op_False",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase);
        Assert.NotNull(opMethod);
        bool isFalse = (bool)opMethod.Invoke(null, [result])!;
        Assert.Equal(result.IsError(), isFalse);
    }
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void OpEqualsWorks<T>(Result<T> result)
    {
        // ReSharper disable once EqualExpressionComparison
        Assert.True(result == result);
        if (result.IsOk(out var ok, out var ex))
        {
            Assert.True(result == ok);
            Assert.True(ok == result);
            Assert.True(result == true);
            Assert.True(true == result);
            Assert.False(result == false);
            Assert.False(false == result);
        }
        else
        {
            Assert.True(result == ex);
            Assert.True(ex == result);
            Assert.True(result == false);
            Assert.True(false == result);
            Assert.False(result == true);
            Assert.False(true == result);
        }
    }

    [Theory]
    [MemberData(nameof(TestResults))]
    public void OpNotEqualsWorks<T>(Result<T> result)
    {
        // ReSharper disable once EqualExpressionComparison
        Assert.False(result != result);
        if (result.IsOk(out var ok, out var ex))
        {
            Assert.False(result != ok);
            Assert.False(ok != result);
            Assert.False(result != true);
            Assert.False(true != result);
            Assert.True(result != false);
            Assert.True(false != result);
        }
        else
        {
            Assert.False(result != ex);
            Assert.False(ex != result);
            Assert.False(result != false);
            Assert.False(false != result);
            Assert.True(result != true);
            Assert.True(true != result);
        }
    }
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void OpGreaterThanWorks<T>(Result<T> result)
    {
        
    }
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void OpGreaterThanOrEqualToWorks<T>(Result<T> result)
    {
        
    }
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void OpLessThanWorks<T>(Result<T> result)
    {
        
    }
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void OpLessThanOrEqualToWorks<T>(Result<T> result)
    {
        
    }
}