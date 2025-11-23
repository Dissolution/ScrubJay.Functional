namespace ScrubJay.Functional.Tests.ResultTTests;

public class SanityTests
{
    public static MiscTheoryData TestValues => CommonTheoryData.TestOkValues;
    public static MiscTheoryData TestExceptions => CommonTheoryData.TestExceptions;
    public static MiscTheoryData TestResults => CommonTheoryData.TestResultTs;
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void CannotBeOkAndError<T>(Result<T> result)
    {
        bool ok = result.IsOk();
        bool error = result.IsError();
        Assert.NotEqual(ok, error);
    }

    
    [Theory]
    [MemberData(nameof(TestExceptions))]
    public void ErrorWorks<E>(E error)
        where E : Exception
    {
        Result<int> result = Result<int>.Error(error);
        Assert.True(result.IsError(out var ex));
        Assert.NotNull(ex);
        Assert.Equal(typeof(E), ex.GetType());
    }
    
    [Theory]
    [MemberData(nameof(TestResults))]
    public void CanEnumerate<T>(Result<T> result)
    {
        using var e = result.GetEnumerator();
        var moved = e.MoveNext();

        if (result.IsOk(out var ok))
        {
            Assert.True(moved);
            Assert.Equal(e.Current, ok);
        }
        else
        {
            Assert.False(moved);
        }

        moved = e.MoveNext();
        Assert.False(moved);
    }
}