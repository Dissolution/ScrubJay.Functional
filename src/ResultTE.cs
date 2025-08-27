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
/// 🦀 Heavily inspired by Rust's Result type 🦀<br/>
/// Result is <see cref="IEnumerable{T}"/>
/// </remarks>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html"/>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T, E>
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{T,E}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    public static implicit operator bool(Result<T, E> result) => result._isOk;

    public static implicit operator Result<T, E>(IMPL.Ok<T> ok) => Ok(ok.Value);
    
    public static implicit operator Result<T, E>(IMPL.Error<E> error) => Error(error.Value);

#endregion

    /// <summary>
    /// The <c>Ok</c> variant of the <see cref="Result{T,E}"/> type holds the result of a successful operation.
    /// </summary>
    public static Result<T, E> Ok(T ok) => new Result<T, E>(true, ok, default);

    /// <summary>
    /// The <c>Error</c> variant of the <see cref="Result{T,E}"/> type holds details of a failed operation.
    /// </summary>
    public static Result<T, E> Error(E error) => new Result<T, E>(false, default, error);


    // discriminates between ok (true) and error (false)
    private readonly bool _isOk;

    // the ok value
    private readonly T? _value;

    // the error value
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

    public bool IsOk() => _isOk;
    
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
    
    public bool IsOkAnd(Func<T, bool> okPredicate) => _isOk && okPredicate(_value!);
    
    public T OkOr(T fallback)
    {
        if (_isOk)
            return _value!;
        return fallback;
    }
    
    public T OkOr(Func<T> getFallback)
    {
        if (_isOk)
            return _value!;
        return getFallback();
    }
    
    public T? OkOrDefault()
    {
        if (_isOk)
            return _value!;
        return default(T);
    }
    
    public T OkOrThrow(string? exceptionMessage = null)
    {
        if (_isOk)
            return _value!;
        throw new InvalidOperationException(exceptionMessage ??
                                            $"There is no Ok value in {ToString()}");
    }


#endregion

#region Error

    public bool IsError() => !_isOk;

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
    
    public E ErrorOrThrow(string? exceptionMessage = null)
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(exceptionMessage ??
                                            $"There is no Error value in {ToString()}");
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

#region LINQ + IEnumerable

    public Result<N, E> Select<N>(Func<T, N> selector)
    {
        if (_isOk)
        {
            return Result<N,E>.Ok(selector(_value!));
        }

        return Result<N, E>.Error(_error!);
    }


    public Result<N, E> Select<N>(Func<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).IsSome(out var value))
        {
            return Result<N, E>.Ok(value);
        }

        return Result<N, E>.Error(_error!);
    }


    public Result<N, E> Select<N>(Func<T, Result<N, E>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }

        return Result<N, E>.Error(_error!);
    }


    public Result<N, E> Select<X, N>(X state, Func<X, T, N> selector)
    {
        if (_isOk)
        {
            return Result<N, E>.Ok(selector(state, _value!));
        }

        return Result<N,E>.Error(_error!);
    }


    public Result<N, E> SelectMany<N>(Func<T, Result<N, E>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }

        return Result<N,E>.Error(_error!);
    }


    public Result<N, E> SelectMany<K, N>(
        Func<T, Result<K, E>> keySelector,
        Func<T, K, N> newSelector)
    {
        if (_isOk && keySelector(_value!).IsOk(out var key))
        {
            return Result<N, E>.Ok(newSelector(_value!, key));
        }

        return Result<N, E>.Error(_error!);
    }

#region IEnumerable
    
    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public ref struct ResultEnumerator
    {
        private readonly Result<T, E> _result;
        private bool _canYield;
        
        public T Current => _result.OkOrThrow();

        public ResultEnumerator(Result<T, E> result)
        {
            _result = result;
            _canYield = result._isOk;
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

    public override string ToString()
    {
        if (_isOk)
        {
            return $"Ok<{typeof(T)}>({Compat<T>.ToString(_value)}";
        }
        else
        {
            return $"Error<{typeof(E)}>({Compat<E>.ToString(_error)}";
        }
    }
}