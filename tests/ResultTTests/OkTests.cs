namespace ScrubJay.Functional.Tests.ResultTTests;

public class OkTests
{
    public static MiscTheoryData TestValues => CommonTheoryData.TestOkValues;
    public static MiscTheoryData TestExceptions => CommonTheoryData.TestExceptions;
    public static MiscTheoryData TestResults => CommonTheoryData.TestResultTs;


    [Theory]
    [MemberData(nameof(TestValues))]
    public void OkWorks<T>(T ok)
    {
        Result<T> result = Result<T>.Ok(ok);
        Assert.True(result.IsOk(out var value));
        Assert.Equal(ok, value);
    }
    //
    // [Theory]
    // [MemberData(nameof(TestResults))]
    // public void IsOkWorks<T>(Result<T> result)
    // {
    //     bool isOk = result.IsOk();
    //     Assert.Equal(result._error is null, isOk);
    // }
    //
    // [Theory]
    // [MemberData(nameof(TestResults))]
    // public void IsOkOutValueWorks<T>(Result<T> result)
    // {
    //     bool isOk = result.IsOk(out var value);
    //     if (result._error is null)
    //     {
    //         Assert.True(isOk);
    //         Assert.Equal(result._value, value);
    //     }
    //     else
    //     {
    //         Assert.False(isOk);
    //     }
    // }
    //
    // [Theory]
    // [MemberData(nameof(TestResults))]
    // public void IsOkOutValueOutExceptionWorks<T>(Result<T> result)
    // {
    //     bool isOk = result.IsOk(out var value, out var ex);
    //     if (result._error is null)
    //     {
    //         Assert.True(isOk);
    //         Assert.Equal(result._value, value);
    //         Assert.Null(ex);
    //     }
    //     else
    //     {
    //         Assert.False(isOk);
    //         Assert.NotNull(ex);
    //     }
    // }
    //
    // [Theory]
    // [MemberData(nameof(TestResults))]
    // public void IsOkAndWorks<T>(Result<T> result)
    // {
    //     bool isOkAndTrue = result.IsOkAnd(static _ => true);
    //     bool isOkAndFalse = result.IsOkAnd(static _ => false);
    //     if (result._error is null)
    //     {
    //         Assert.True(isOkAndTrue);
    //         Assert.False(isOkAndFalse);
    //     }
    //     else
    //     {
    //         Assert.False(isOkAndTrue);
    //         Assert.False(isOkAndFalse);
    //     }
    // }

    [Theory]
    [MemberData(nameof(TestValues))]
    public void OkOrFallbackWorks<T>(T value)
    {
        Result<T> okResult = Result<T>.Ok(default(T)!);
        var output = okResult.OkOr(value);
        Assert.Equal(default(T), output);

        Result<T> errorResult = Result<T>.Error(new InvalidOperationException());
        output = errorResult.OkOr(value);
        Assert.Equal(value, output);
    }

    [Theory]
    [MemberData(nameof(TestValues))]
    public void OkOrFallbackFactoryWorks<T>(T value)
    {
        Func<T> fallback = () => value;

        Result<T> okResult = Result<T>.Ok(default(T)!);
        var output = okResult.OkOr(fallback);
        Assert.Equal(default(T), output);

        Result<T> errorResult = Result<T>.Error(new InvalidOperationException());
        output = errorResult.OkOr(fallback);
        Assert.Equal(value, output);
    }

    [Theory]
    [MemberData(nameof(TestValues))]
    public void OkOrDefaultWorks<T>(T value)
    {
        Result<T> okResult = Result<T>.Ok(value);
        var output = okResult.OkOrDefault();
        Assert.Equal(value, output);

        Result<T> errorResult = Result<T>.Error(new InvalidOperationException());
        output = errorResult.OkOrDefault();
        Assert.Equal(default(T), output);
    }

    [Theory]
    [MemberData(nameof(TestExceptions))]
    public void OkOrThrowWorks<E>(E ex)
        where E : Exception
    {
        Result<int> okResult = Result<int>.Ok(147);
        var output = okResult.OkOrThrow();
        Assert.Equal(147, output);

        Result<int> errorResult = Result<int>.Error(ex);
        Assert.Throws<E>(() => errorResult.OkOrThrow());
    }
}