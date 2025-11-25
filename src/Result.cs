// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031


namespace ScrubJay.Functional;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result, Result, bool>,
#endif
    IEquatable<Result>
{
    #region Operators

    public static implicit operator bool(Result result) => result._error is null;
    public static implicit operator Result(bool success) => success ? Ok : Error(new InvalidOperationException());
    public static implicit operator Result(Exception ex) => Error(ex);
    public static implicit operator Result(IMPL.Error<Exception> error) => Error(error.Value);

    public static bool operator ==(Result left, Result right) => left.Equals(right);
    public static bool operator !=(Result left, Result right) => !left.Equals(right);

    #endregion


    public static readonly Result Ok = new Result(null);
    public static Result Error(Exception ex) => new Result(ex);

    // Unlike Result<T> and Result<T,E>, default(Result) == true
    private readonly Exception? _error;

    private Result(Exception? error)
    {
        _error = error;
    }


    public bool IsOk() => _error is null;


    #region Error

    public bool IsError() => _error is not null;

    public bool IsError([MaybeNullWhen(false)] out Exception error)
    {
        error = _error;
        return error is not null;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    public bool IsErrorAnd(Func<Exception, bool> errorPredicate) => _error is not null && errorPredicate(_error!);

    public Exception ErrorOr(Exception fallback)
    {
        if (_error is not null)
            return _error;
        return fallback;
    }

    public Exception ErrorOr(Func<Exception> getFallback)
    {
        if (_error is not null)
            return _error;
        return getFallback();
    }

    [StackTraceHidden]
    public void ThrowIfError()
    {
        if (_error is not null)
        {
            throw _error;
        }
    }

    #endregion

    #region Match

    public void Match(Action onOk, Action<Exception> onError)
    {
        if (_error is null)
        {
            onOk();
        }
        else
        {
            onError(_error!);
        }
    }


    public R Match<R>(Func<R> onOk, Func<Exception, R> onError)
    {
        if (_error is null)
        {
            return onOk();
        }
        else
        {
            return onError(_error!);
        }
    }

    #endregion

    public Option<Unit> AsOption()
    {
        if (_error is null)
        {
            return Some(default(Unit));
        }
        else
        {
            return None;
        }
    }

    #region Equality

    public bool Equals(Result other)
    {
        return EqualityComparer<Exception>.Default.Equals(_error!, other._error!);
    }

    public bool Equals(Exception? error)
    {
        return EqualityComparer<Exception>.Default.Equals(_error!, error!);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result result => Equals(result),
            Exception ex => Equals(ex),
            bool isOk => isOk == _error is null,
            _ => false,
        };


    public override int GetHashCode()
    {
        if (_error is not null)
        {
            return _error.GetHashCode();
        }

        return 1;
    }

    #endregion

    #region Formatting

    public override string ToString()
    {
        if (_error is null)
        {
            return "Ok";
        }

        return $"Error({_error})";
    }

    #endregion
}