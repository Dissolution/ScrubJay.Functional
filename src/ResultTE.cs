using System.Text;

namespace ScrubJay.Functional;

/// <summary>
/// <c>Result&lt;T, E&gt;</c> is used for return and error propagation<br/>
/// It acts like a discriminated union with two values:<br/>
/// <c>Ok&lt;T&gt;</c> -> Indicates success with a contained <typeparamref name="T"/> value<br/>
/// <c>Error&lt;E&gt;</c> -> Indicates failure with a contained <typeparamref name="E"/> value
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of value contained in an <c>Ok</c></typeparam>
/// <typeparam name="E">The <see cref="Type"/> of value contained in an <c>Error</c></typeparam>
/// <remarks>
/// 🦀 Heavily inspired by Rust's Result type 🦀
/// </remarks>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html"/>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T, E> :
    /* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T, E>, Result<T, E>, bool>,
    // IEqualityOperators<Result<T, E>, T, bool>,
    // IEqualityOperators<Result<T, E>, E, bool>,
#endif
    IEquatable<Result<T, E>>,
    // IEquatable<T>,
    // IEquatable<E>,
    IEnumerable<T>
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{T,E}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    public static implicit operator bool(Result<T, E> result) => result._isOk;

    public static implicit operator Result<T, E>(IMPL.Ok<T> ok) => Ok(ok.Value);
    public static implicit operator Result<T, E>(IMPL.Error<E> error) => Error(error.Value);

    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);
    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !left.Equals(right);
    public static bool operator ==(Result<T, E> result, T? ok) => result.Equals(ok);
    public static bool operator !=(Result<T, E> result, T? ok) => !result.Equals(ok);
    public static bool operator ==(Result<T, E> result, E? error) => result.Equals(error);
    public static bool operator !=(Result<T, E> result, E? error) => !result.Equals(error);

#endregion

    /// <summary>
    /// Creates a new Ok <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="ok">The Ok value</param>
    /// <returns></returns>
    public static Result<T, E> Ok(T ok) => new Result<T, E>(true, ok, default);

    /// <summary>
    /// Creates a new Error <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="error">The Error value</param>
    /// <returns></returns>
    public static Result<T, E> Error(E error) => new Result<T, E>(false, default, error);


    // is this Result.Ok?
    // default(Result) implies !_isOk, thus default(Result) == None
    private readonly bool _isOk;

    // if this is Result.Ok, the Ok Value
    private readonly T? _value;

    // if this is Result.Error, the Error Value
    private readonly E? _error;

    /// <summary>
    /// Result may only be constructed through <see cref="Ok(T)"/>, <see cref="Error(E)"/>,
    /// or through implicit conversion from <see cref="IMPL.Ok{T}"/> or <see cref="IMPL.Error{E}"/>
    /// </summary>
    private Result(bool isOk, T? value, E? error)
    {
        _isOk = isOk;
        _value = value;
        _error = error;
    }

#region Ok

    public Option<T> IsOk()
    {
        if (_isOk)
            return Some(_value!);
        else
            return None;
    }

    /// <summary>
    /// Returns <c>true</c> and <paramref name="value"/> if this Result is Ok
    /// </summary>
    /// <param name="value">
    /// If this is an Ok result, the Ok value, otherwise default(<typeparamref name="T"/>)
    /// </param>
    /// <returns></returns>
    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        if (_isOk)
        {
            value = _value!;
            return true;
        }

        value = default!;
        return false;
    }

    public bool IsOk([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out E error)
    {
        ok = _value;
        error = _error;
        return _isOk;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Ok and the value inside of it matches a predicate
    /// </summary>
    /// <param name="okPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok_and"/>
    public bool IsOkAnd(Func<T, bool> okPredicate) => _isOk && okPredicate(_value!);

    /// <summary>
    /// Returns the contained Ok value
    /// </summary>
    /// <returns>
    ///
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the value is an Error
    /// </exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap"/>
    public T OkOrThrow(string? errorMessage = null)
    {
        if (_isOk)
            return _value!;
        if (_error is Exception ex)
            throw ex;
        throw new InvalidOperationException(errorMessage ?? this.ToString());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fallback"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or"/>
    public T OkOr(T fallback)
    {
        if (_isOk)
            return _value!;
        return fallback;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getFallback"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_else"/>
    public T OkOr(Func<T> getFallback)
    {
        if (_isOk)
            return _value!;
        return getFallback();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_default"/>
    public T? OkOrDefault()
    {
        if (_isOk)
            return _value!;
        return default(T);
    }

#endregion

#region Error

    public Option<E> IsError()
    {
        if (!_isOk)
            return Some(_error!);
        else
            return None;
    }

    public bool IsError([MaybeNullWhen(false)] out E error)
    {
        if (!_isOk)
        {
            error = _error!;
            return true;
        }

        error = default!;
        return false;
    }

    public bool IsError([MaybeNullWhen(false)] out E error, [MaybeNullWhen(true)] out T ok)
    {
        error = _error;
        ok = _value;
        return !_isOk;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    public bool IsErrorAnd(Func<E, bool> errorPredicate) => !_isOk && errorPredicate(_error!);


    public E ErrorOr(E error)
    {
        if (!_isOk)
            return _error!;
        return error;
    }


    public E ErrorOr(Func<E> getError)
    {
        if (_isOk)
            return _error!;
        return getError();
    }


    public E? ErrorOrDefault()
    {
        if (!_isOk)
            return _error!;
        return default(E);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.unwrap_err"/>
    public E ErrorOrThrow()
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(ToString());
    }

#endregion

#region Match

    public void Match(Action<T> onOk, Action<E> onError)
    {
        if (_isOk)
        {
            onOk(_value!);
        }
        else
        {
            onError(_error!);
        }
    }


    public R Match<R>(Func<T, R> onOk, Func<E, R> onError)
    {
        if (_isOk)
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
        if (_isOk)
        {
            return Some(_value!);
        }
        else
        {
            return None;
        }
    }

#region Equal

    public bool Equals(Result<T, E> result)
    {
        if (_isOk)
        {
            if (result._isOk)
            {
                return EqualityComparer<T>.Default.Equals(_value!, result._value!);
            }

            return false;
        }

        if (result._isOk)
        {
            return false;
        }

        return EqualityComparer<E>.Default.Equals(_error!, result._error!);
    }

    public bool Equals(Result<T, E> other,
        IEqualityComparer<T>? okComparer,
        IEqualityComparer<E>? errorComparer)
    {
        if (_isOk)
        {
            if (other._isOk)
            {
                return (okComparer ?? EqualityComparer<T>.Default).Equals(_value!, other._value!);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (other._isOk)
            {
                return false;
            }
            else
            {
                return (errorComparer ?? EqualityComparer<E>.Default).Equals(_error!, other._error!);
            }
        }
    }

    public bool Equals(T? ok)
    {
        if (_isOk)
        {
            return EqualityComparer<T>.Default.Equals(_value!, ok!);
        }

        return false;
    }

    public bool Equals(T? ok, IEqualityComparer<T>? comparer)
    {
        if (_isOk)
        {
            return (comparer ?? EqualityComparer<T>.Default).Equals(_value!, ok!);
        }

        return false;
    }

    public bool Equals(E? error)
    {
        if (!_isOk)
        {
            return EqualityComparer<E>.Default.Equals(_error!, error!);
        }

        return false;
    }

    public bool Equals(E? error, IEqualityComparer<E>? comparer)
    {
        if (!_isOk)
        {
            return (comparer ?? EqualityComparer<E>.Default).Equals(_error!, error!);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<E, E> result => Equals(result),
            T ok => Equals(ok),
            E error => Equals(error),
            bool isOk => _isOk == isOk,
            _ => false,
        };

    public override int GetHashCode()
    {
        if (_isOk)
        {
            return _value?.GetHashCode() ?? 0;
        }
        else
        {
            return _error?.GetHashCode() ?? 0;
        }
    }

#endregion

#region ToString / TryFormat

  
    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append("Result<")
            .Append(typeof(T))
            .Append(", ")
            .Append(typeof(E))
            .Append(">.");
        if (_isOk)
        {
            builder.Append("Ok(")
                .Append(_value);
        }
        else
        {
            builder.Append("Error(")
                .Append(_error);
        }

        builder.Append(')');
        return builder.ToString();
    }

#endregion


#region LINQ + IEnumerable

    public Result<N, E> Select<N>(Func<T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(_value!), default);
        }

        return new(false, default, _error);
    }


    public Result<N, E> Select<N>(Func<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).IsSome(out var value))
        {
            return new(true, value, default);
        }

        return new(false, default, _error);
    }


    public Result<N, E> Select<N>(Func<T, Result<N, E>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }

        return new(false, default, _error);
    }


    public Result<N, E> Select<X, N>(X state, Func<X, T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(state, _value!), default);
        }

        return new(false, default, _error);
    }


    public Result<N, E> SelectMany<N>(Func<T, Result<N, E>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }

        return new(false, default, _error);
    }


    public Result<N, E> SelectMany<K, N>(
        Func<T, Result<K, E>> keySelector,
        Func<T, K, N> newSelector)
    {
        if (_isOk && keySelector(_value!).IsOk(out var key))
        {
            return new(true, newSelector(_value!, key), default);
        }

        return new(false, default, _error);
    }

#region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public struct ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly Result<T, E> _result;
        private bool _canYield;

        object? IEnumerator.Current => _result.OkOrThrow();

        public T Current => _result.OkOrThrow();

        public ResultEnumerator(Result<T, E> result)
        {
            _result = result;
            _canYield = result._isOk;
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
            _canYield = _result._isOk;
        }
    }

#endregion

#endregion
}