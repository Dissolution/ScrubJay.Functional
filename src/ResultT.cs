// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031


namespace ScrubJay.Functional;

/// <summary>
/// A Result type holding a returned <typeparamref name="T"/> value or <see cref="Exception"/>.
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of value stored with an <c>Ok</c> Result
/// </typeparam>
/// <remarks> 
/// This emulates a discriminated union:
/// <code>
/// Result
/// {
///     Ok(T),
///     Error(Exception),
/// }
/// </code>
/// 🦀 Heavily inspired by Rust's Result type! 🦀
/// </remarks>
/// <seealso href="https://en.wikipedia.org/wiki/Result_type">Result Type on Wikipedia</seealso>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html">Rust's Result Type</seealso>
[PublicAPI]
#if !NETSTANDARD2_0
[AsyncMethodBuilder(typeof(ResultAsyncMethodBuilder<>))]
#endif
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T> :
    /* All commented out interfaces are implemented, but cannot be declared per CS0695:
     * 'Result<T>' cannot implement both 'X' and 'Y' because they may unify for some type parameter substitutions
     */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T>, Result<T>, bool>,
    IEqualityOperators<Result<T>, T, bool>,
    IEqualityOperators<Result<T>, Exception, bool>,
    IComparisonOperators<Result<T>, Result<T>, bool>,
    IComparisonOperators<Result<T>, T, bool>,
#endif
    IEquatable<Result<T>>,
    IEquatable<T>,
    IEquatable<Exception>,
    IComparable<Result<T>>,
    IComparable<T>,
    IEnumerable<T>,
    IFormattable
{
    #region Operators

    public static implicit operator bool(Result<T> result) => result._error is null;

    public static implicit operator Result(Result<T> result) =>
        result.IsError(out var error) ? Result.Error(error) : Result.Ok;

    public static implicit operator Result<T>(T value) => Ok(value);
    public static implicit operator Result<T>(Exception ex) => Error(ex);

    public static implicit operator Result<T>(IMPL.Ok<T> ok) => Ok(ok.Value);
    public static implicit operator Result<T>(IMPL.Error<Exception> error) => Error(error.Value);

    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
    public static bool operator ==(Result<T> result, T? value) => result.Equals(value);
    public static bool operator !=(Result<T> result, T? value) => !result.Equals(value);
    public static bool operator ==(Result<T> result, Exception? error) => result.Equals(error);
    public static bool operator !=(Result<T> result, Exception? error) => !result.Equals(error);

    public static bool operator >(Result<T> left, Result<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Result<T> left, Result<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Result<T> left, Result<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Result<T> left, Result<T> right) => left.CompareTo(right) <= 0;
    public static bool operator >(Result<T> left, T right) => left.CompareTo(right) > 0;
    public static bool operator >=(Result<T> left, T right) => left.CompareTo(right) >= 0;
    public static bool operator <(Result<T> left, T right) => left.CompareTo(right) < 0;
    public static bool operator <=(Result<T> left, T right) => left.CompareTo(right) <= 0;

    #endregion


    /// <summary>
    /// Creates an Ok <see cref="Result{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok(T value) => new Result<T>(value, null);

    /// <summary>
    /// Creates <see cref="Result{T}"/>.Error(<paramref name="ex"/>)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Error(Exception ex) => new Result<T>(default, ex ?? new Exception());

    // minimal fields:   `_error is null ? Ok : Error`

    // possible ok value
    private readonly T? _value;

    // possible error
    private readonly Exception? _error;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="error"></param>
    /// <remarks>
    /// <see cref="Result{T}"/> may only be constructed with <see cref="Ok"/>, <see cref="Error"/>,
    /// or an implicit conversion from a <typeparamref name="T"/> or <see cref="Exception"/>.
    /// </remarks>
    private Result(T? value, Exception? error)
    {
        _value = value;
        _error = error;
    }


    #region Ok

    /// <summary>
    /// Is this an Ok <see cref="Result{T}"/>?
    /// </summary>
    public bool IsOk() => _error is null;

    /// <summary>
    /// Is this an Ok <see cref="Result{T}"/>?
    /// </summary>
    /// <param name="value">
    /// If this is an Ok <see cref="Result{T}"/>, the Ok value;
    /// otherwise <c>default(T)</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if this is an Ok <see cref="Result{T}"/>; otherwise <c>false</c>.
    /// </returns>
    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _error is null;
    }

    /// <summary>
    /// Is this an Ok <see cref="Result{T}"/>?
    /// </summary>
    /// <param name="value">
    /// If this is an Ok <see cref="Result{T}"/>, the Ok value;
    /// otherwise <c>default(T)</c>.
    /// </param>
    /// <param name="error">
    /// If this is an Error <see cref="Result{T}"/>, the Error <see cref="Exception"/>;
    /// otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if this is an Ok <see cref="Result{T}"/>; otherwise <c>false</c>.
    /// </returns>
    public bool IsOk([MaybeNullWhen(false)] out T value, [NotNullWhen(false)] out Exception? error)
    {
        value = _value;
        error = _error;
        return error is null;
    }

    public bool IsOkAnd(Func<T, bool> okPredicate)
    {
        return _error is null && okPredicate(_value!);
    }

    public T OkOr(T fallback)
    {
        if (_error is null)
            return _value!;
        return fallback;
    }

    public T OkOr(Func<T> getFallback)
    {
        if (_error is null)
            return _value!;
        return getFallback();
    }

    public T? OkOrDefault()
    {
        if (_error is null)
            return _value!;
        return default(T);
    }

    public T OkOrThrow()
    {
        if (_error is null)
            return _value!;
        throw _error;
    }

    #endregion

    #region Error

    public bool IsError() => _error is not null;

    public bool IsError([NotNullWhen(true)] out Exception? error)
    {
        error = _error;
        return error is not null;
    }


    public bool IsError([NotNullWhen(true)] out Exception? error, [MaybeNullWhen(true)] out T ok)
    {
        error = _error;
        ok = _value;
        return error is not null;
    }

    public bool IsErrorAnd(Func<Exception, bool> errorPredicate)
    {
        return _error is not null && errorPredicate(_error);
    }

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

    public void ThrowIfError()
    {
        if (_error is not null)
        {
            throw _error;
        }
    }

    #endregion

    #region Match

    public void Match(Action<T> onOk, Action<Exception> onError)
    {
        if (_error is null)
        {
            onOk(_value!);
        }
        else
        {
            onError(_error!);
        }
    }


    public R Match<R>(Func<T, R> onOk, Func<Exception, R> onError)
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif
    {
        if (_error is null)
        {
            return onOk(_value!);
        }
        else
        {
            return onError(_error!);
        }
    }

    #endregion

    public Option<T> AsOption()
    {
        if (_error is null)
        {
            return Option<T>.Some(_value!);
        }
        else
        {
            return Option<T>.None;
        }
    }

    #region Comparison

    public int CompareTo(Result<T> other)
    {
        if (_error is null)
        {
            if (other._error is null)
            {
                return Comparer<T>.Default.Compare(_value!, other._value!);
            }
            else
            {
                return -1; // Ok < Error
            }
        }
        else
        {
            if (other._error is null)
            {
                return 1; // Error > Ok
            }
            else
            {
                return Comparer<Exception>.Default.Compare(_error!, other._error!);
            }
        }
    }

    public int CompareTo(T? ok)
    {
        if (_error is null)
        {
            return Comparer<T>.Default.Compare(_value!, ok!);
        }

        return 1; // Error < Ok
    }

    #endregion

    #region Equality

    public bool Equals(Result<T> other)
    {
        if (_error is null)
        {
            if (other._error is null)
            {
                return EqualityComparer<T>.Default.Equals(_value!, other._value!);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (other._error is null)
            {
                return false;
            }
            else
            {
                return EqualityComparer<Exception>.Default.Equals(_error!, other._error!);
            }
        }
    }

    public bool Equals(T? ok)
    {
        if (_error is null)
        {
            return EqualityComparer<T>.Default.Equals(_value!, ok!);
        }

        return false;
    }

    public bool Equals(Exception? error)
    {
        if (_error is not null)
        {
            return EqualityComparer<Exception>.Default.Equals(_error!, error!);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<T> result => Equals(result),
            T value => Equals(value),
            Exception ex => Equals(ex),
            bool isOk => isOk == _error is null,
            _ => false,
        };


    public override int GetHashCode()
    {
        if (_error is null)
        {
            if (_value is not null)
            {
                return _value.GetHashCode();
            }

            return typeof(T).GetHashCode();
        }
        else
        {
            if (_error is not null)
            {
                return _error.GetHashCode();
            }

            return typeof(Exception).GetHashCode();
        }
    }

    #endregion

    #region ToString / TryFormat

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        string? str;

        if (_error is null)
        {
            if (_value is IFormattable)
            {
                str = ((IFormattable)_value!).ToString(format, provider);
            }
            else
            {
                str = _value?.ToString();
            }

            return $"Result<{typeof(T)}>.Ok({str})";
        }
        else
        {
            return $"Result<{typeof(T)}>.Error({_error:@})";
        }
    }

    public override string ToString()
    {
        if (_error is null)
        {
            return $"Result<{typeof(T)}>.Ok({_value})";
        }
        else
        {
            return $"Result<{typeof(T)}>.Error({_error})";
        }
    }

    #endregion

    #region Linq

    public Result<N> Select<N>(Func<T, N> selector)
    {
        if (_error is null)
        {
            return Result<N>.Ok(selector(_value!));
        }
        else
        {
            return Result<N>.Error(_error);
        }
    }

    public Result<N> Select<N>(Func<T, Result<N>> selector)
    {
        if (_error is null)
        {
            return selector(_value!);
        }
        else
        {
            return Result<N>.Error(_error);
        }
    }

    public Result<N> Select<N>(Func<T, Option<N>> selector)
    {
        if (_error is null)
        {
            if (selector(_value!).IsSome(out var some))
            {
                return Result<N>.Ok(some);
            }
            else
            {
                return Result<N>.Error(new Exception());
            }
        }
        else
        {
            return Result<N>.Error(_error);
        }
    }

    public Result<N> SelectMany<K, N>(Func<T, Result<K>> keySelector, Func<T, K, N> newSelector)
    {
        if (IsOk(out var value, out var error))
        {
            var keyResult = keySelector(value!);
            if (keyResult.IsOk(out var key, out error))
            {
                var newSelect = newSelector(value, key);
                return Result<N>.Ok(newSelect);
            }
            else
            {
                return error;
            }
        }
        else
        {
            return error;
        }
    }

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public struct ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly Result<T> _result;
        private bool _canYield;

        object? IEnumerator.Current => _result.OkOrThrow();

        public T Current => _result.OkOrThrow();

        public ResultEnumerator(Result<T> result)
        {
            _result = result;
            _canYield = result._error is null;
        }

        void IDisposable.Dispose()
        {
            /* Do Nothing */
        }

        public bool MoveNext()
        {
            if (!_canYield)
            {
                return false;
            }
            else
            {
                _canYield = false;
                return true;
            }
        }

        public void Reset()
        {
            _canYield = _result._error is null;
        }
    }

    #endregion

    /// <summary>
    /// Support for <c>await</c> syntax in order to support early return from <c>async</c> methods
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ResultAwaiter<T> GetAwaiter() => new(this);
}