namespace ScrubJay.Functional;

/// <summary>
/// Option represents an optional value: every Option is either Some and contains a value, or None, and does not
/// </summary>
/// <typeparam name="T">The generic <see cref="Type"/> for a Some value</typeparam>
/// <remarks>
/// Heavily inspired by Rust's Option type<br/>
/// <a href="https://doc.rust-lang.org/std/option/"/><br/>
/// <a href="https://doc.rust-lang.org/std/option/enum.Option.html"/><br/>
/// </remarks>
[PublicAPI]
public readonly struct Option<T> :
/* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
#if NET7_0_OR_GREATER
    IEqualityOperators<Option<T>, Option<T>, bool>,
    IEqualityOperators<Option<T>, None, bool>,
    //IEqualityOperators<Option<T>, T, bool>,
#endif
    IEquatable<Option<T>>,
    IEquatable<None>,
    //IEquatable<T>,
#if NET7_0_OR_GREATER
    IComparisonOperators<Option<T>, Option<T>, bool>,
    IComparisonOperators<Option<T>, None, bool>,
    //IComparisonOperators<Option<T>, T, bool>,
#endif
    IComparable<Option<T>>,
    IComparable<None>,
    //IComparable<T>,
    IEnumerable<T>,
    IEnumerable
{
#region Operators

    public static implicit operator bool(Option<T> option) => option._isSome;
    public static implicit operator Option<T>(None none) => None;
    public static implicit operator Option<T>(T some) => Some(some);

    public static bool operator true(Option<T> option) => option._isSome;
    public static bool operator false(Option<T> option) => !option._isSome;

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
    public static bool operator >(Option<T> left, Option<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Option<T> left, Option<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Option<T> left, Option<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Option<T> left, Option<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Option<T> option, None none) => option.IsNone();
    public static bool operator !=(Option<T> option, None none) => option.IsSome();
    public static bool operator >(Option<T> option, None none) => option.CompareTo(none) > 0;
    public static bool operator >=(Option<T> option, None none) => option.CompareTo(none) >= 0;
    public static bool operator <(Option<T> option, None none) => option.CompareTo(none) < 0;
    public static bool operator <=(Option<T> option, None none) => option.CompareTo(none) <= 0;

    public static bool operator ==(Option<T> option, T? some) => option.Equals(some);
    public static bool operator !=(Option<T> option, T? some) => !option.Equals(some);
    public static bool operator >(Option<T> option, T? some) => option.CompareTo(some) > 0;
    public static bool operator >=(Option<T> option, T? some) => option.CompareTo(some) >= 0;
    public static bool operator <(Option<T> option, T? some) => option.CompareTo(some) < 0;
    public static bool operator <=(Option<T> option, T? some) => option.CompareTo(some) <= 0;

#endregion

    /// <summary>
    /// Gets the None option
    /// </summary>
    public static Option<T> None => default;

    /// <summary>
    /// Creates a new <see cref="Option{T}"/>.Some containing <paramref name="some"/>
    /// </summary>
    /// <param name="some"></param>
    /// <returns></returns>
    public static Option<T> Some(T some) => new(some);


    private readonly bool _isSome;
    private readonly T? _some;

    private Option(T some)
    {
        _isSome = true;
        _some = some;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some"/>
    public bool IsSome() => _isSome;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="somePredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some_and"/>
    public bool IsSomeAnd(Func<T, bool> somePredicate) => _isSome && somePredicate(_some!);

    public bool IsSome([MaybeNullWhen(false)] out T some)
    {
        if (_isSome)
        {
            some = _some!;
            return true;
        }

        some = default;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_none"/>
    public bool IsNone() => !_isSome;


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap"/>
    public T SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _some!;
        throw new InvalidOperationException(errorMessage ?? "This Option is not Some");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fallbackSome"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or"/>
    public T SomeOr(T fallbackSome)
    {
        if (_isSome)
            return _some!;
        return fallbackSome;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_default"/>
    public T? SomeOrDefault()
    {
        {
            if (_isSome)
                return _some!;
            return default(T);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getSome"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_else"/>
    public T SomeOrElse(Func<T> getSome)
    {
        if (_isSome)
            return _some!;
        return getSome();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.ok_or"/>
    public Result<T, TError> SomeOr<TError>(TError error)
    {
        if (_isSome)
            return Result<T, TError>.Ok(_some!);
        return Result<T, TError>.Error(error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getError"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.ok_or_else"/>
    public Result<T, TError> SomeOrElse<TError>(Func<TError> getError)
    {
        if (_isSome)
            return Result<T, TError>.Ok(_some!);
        return Result<T, TError>.Error(getError());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.filter"/>
    public Option<T> Filter(Func<T, bool> predicate)
    {
        if (IsSome(out var value) && predicate(value))
            return this;
        return None;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map"/>
    public Option<TNew> MapSome<TNew>(Func<T, TNew> map)
    {
        if (IsSome(out var value))
        {
            return Some<TNew>(map(value));
        }

        return Option<TNew>.None;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or"/>
    public TNew MapSomeOr<TNew>(Func<T, TNew> map, TNew defaultValue)
    {
        if (IsSome(out var value))
        {
            return map(value);
        }

        return defaultValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="getDefaultValue"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or_else"/>
    public TNew MapSomeOrElse<TNew>(Func<T, TNew> map, Func<TNew> getDefaultValue)
    {
        if (IsSome(out var value))
        {
            return map(value);
        }

        return getDefaultValue();
    }

    public void Match(Action<T> onSome, Action onNone)
    {
        if (_isSome)
        {
            onSome(_some!);
        }
        else
        {
            onNone();
        }
    }

    public void Match(Action<T> onSome, Action<None> onNone)
    {
        if (_isSome)
        {
            onSome(_some!);
        }
        else
        {
            onNone(default);
        }
    }

    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
    {
        if (_isSome)
        {
            return some(_some!);
        }
        else
        {
            return none();
        }
    }

    public TResult Match<TResult>(Func<T, TResult> some, Func<None, TResult> none)
    {
        if (_isSome)
        {
            return some(_some!);
        }
        else
        {
            return none(default);
        }
    }

    public Result<T, Exception> AsResult()
    {
        if (_isSome)
            return _some!;
        return new InvalidOperationException("Option.None");
    }

#region Compare

    public int CompareTo(Option<T> other)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            if (other._isSome)
            {
                return Comparer<T>.Default.Compare(_some!, other._some!);
            }
            else // y is none
            {
                return 1; // I am greater
            }
        }
        else // x is none
        {
            if (other._isSome)
            {
                return -1; // I am lesser
            }
            else
            {
                // None == None
                return 0;
            }
        }
    }

    public int CompareTo(T? other)
    {
        if (_isSome)
        {
            return Comparer<T>.Default.Compare(_some!, other!);
        }
        else
        {
            // None compares as less than any Some
            return -1;
        }
    }

    public int CompareTo(None none)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            return 1; // I am greater
        }
        else // x is none
        {
            // None == None
            return 0;
        }
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Option<T> option => CompareTo(option),
            T some => CompareTo(some),
            None none => CompareTo(none),
            _ => 1, // unknown values sort before
        };
    }

#endregion

#region Equals

    public bool Equals(Option<T> other)
    {
        if (_isSome)
        {
            if (other._isSome)
            {
                return EqualityComparer<T>.Default.Equals(_some!, other._some!);
            }
            else // y is none
            {
                return false;
            }
        }
        else // x is none
        {
            if (_isSome)
            {
                return false;
            }
            else
            {
                // None == None
                return true;
            }
        }
    }

    public bool Equals(T? value)
    {
        return _isSome && EqualityComparer<T>.Default.Equals(_some!, value!);
    }

    public bool Equals(None none) => !_isSome;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            Option<T> option => Equals(option),
            T value => Equals(value),
            None none => Equals(none),
            _ => false,
        };
    }

#endregion

    public override int GetHashCode()
    {
        if (_isSome)
        {
            return _some?.GetHashCode() ?? 0;
        }
        return 0;
    }

    public override string ToString()
    {
        return Match(static value => $"Some({value})", static () => nameof(None));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.iter"/>
    public OptionEnumerator GetEnumerator() => new OptionEnumerator(this);


    public struct OptionEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _yielded;
        private readonly T _value;

        object? IEnumerator.Current => _value;
        public T Current => _value;

        public OptionEnumerator(Option<T> option)
        {
            if (option._isSome)
            {
                _value = option._some!;
                _yielded = false;
            }
            else
            {
                _value = default!;
                _yielded = true;
            }
        }

        public bool MoveNext()
        {
            if (_yielded)
                return false;
            _yielded = true;
            return true;
        }

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose()
        {
            // Do nothing
        }
    }
}