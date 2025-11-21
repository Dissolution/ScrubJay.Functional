using System.ComponentModel.DataAnnotations;

namespace ScrubJay.Functional.Tests;

public partial class ResultT_Tests
{
    public static MiscTheoryData TestResults { get; } = new MiscTheoryData()
    {
        Result<byte>.Ok(147),
        Result<string>.Ok("TRJ"),
        Result<Type?>.Ok(typeof(Unit)),
        Result<Type?>.Ok(null),
        Result<int?>.Ok(147),
        Result<int?>.Ok(null),
        Result<byte>.Error(new InvalidOperationException()),
        Result<string>.Error(new InvalidOperationException()),
        Result<Type?>.Error(new InvalidOperationException()),
        Result<int?>.Error(new InvalidOperationException()),
        Result<Unit>.Ok(default),
        Result<Unit>.Error(default!),
        Result<Exception>.Ok(new Exception()),
        Result<Exception>.Error(new Exception()),
    };

    public static MiscTheoryData TestOkTypes { get; } = new MiscTheoryData()
    {
        (byte)147,
        (string)"TRJ",
        typeof(Unit),
        (int?)null,
        (int?)13,
        Unit.Default,
        new Exception(),
        new List<Guid>(),
    };

    public static MiscTheoryData TestValues => CommonTheoryData.Values;


    [Theory]
    [MemberData(nameof(TestResults))]
    public void CannotBeOkAndError<T>(Result<T> result)
    {
        bool ok = result.IsOk();
        bool error = result.IsError();
        Demand.NotEqual(ok, error, "A result must be Ok xor Error");
    }

    [Theory]
    [MemberData(nameof(TestResults))]
    public void CanImplicitlyCastToBool<T>(Result<T> result)
    {
        bool boolean = result;
        Demand.That(boolean).IsEqualTo(result.IsOk());
    }

    [Theory]
    [MemberData(nameof(TestResults))]
    public void CanEnumerate<T>(Result<T> result)
    {
        using var e = result.GetEnumerator();
        var moved = e.MoveNext();

        if (result.IsOk(out var ok))
        {
            Demand.That(moved).IsTrue();
            Demand.That(e.Current).IsEqualTo(ok);
        }
        else
        {
            Demand.That(moved).IsFalse();
        }

        moved = e.MoveNext();
        Demand.That(moved).IsFalse();
    }

    [Theory]
    [MemberData(nameof(TestValues))]
    public void ImplicitOkWorks<T>(T ok)
    {
        Result<T> result = ok;
        Demand.That(result.IsOk()).IsTrue();
        Demand.That(result.OkOrThrow()).IsEqualTo(ok);
    }

    [Theory]
    [MemberData(nameof(TestOkTypes))]
    public void ImplicitErrorWorks<T>(T _)
    {
        Result<T> result = new InvalidOperationException();
        Demand.That(result.IsError(out var ex)).IsTrue();
        Demand.That(ex).IsNotNull();
        Demand.That(ex!.GetType()).IsEqualTo<InvalidOperationException>();
    }

    [Theory]
    [MemberData(nameof(TestResults))]
    public void OpEqualsWorks<T>(Result<T> result)
    {
        // ReSharper disable once EqualExpressionComparison
        Demand.That(result == result).IsTrue();
        if (result.IsOk(out var ok, out var ex))
        {
            Demand.That(result == ok).IsTrue();
            Demand.That(ok == result).IsTrue();
            Demand.That(result == true).IsTrue();
            Demand.That(true == result).IsTrue();
            Demand.That(result == false).IsFalse();
            Demand.That(false == result).IsFalse();
        }
        else
        {
            Demand.That(result == ex).IsTrue();
            Demand.That(ex == result).IsTrue();
            Demand.That(result == false).IsTrue();
            Demand.That(false == result).IsTrue();
            Demand.That(result == true).IsFalse();
            Demand.That(true == result).IsFalse();
        }
    }
}