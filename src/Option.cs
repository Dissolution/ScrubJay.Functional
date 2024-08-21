namespace ScrubJay.Functional;

[PublicAPI]
public static class Option
{
    /// <summary>
    /// Returns a <see cref="Option{T}"/>.<see cref="Option{T}.Some"/> if <paramref name="value"/> is not <c>null</c>;<br/>
    /// otherwise returns <see cref="None"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> NotNull<T>(T? value)
        where T : notnull
    {
        if (value is not null)
            return Option<T>.Some(value);
        return Option<T>.None;
    }

    /// <summary>
    /// Returns a common <see cref="None"/> that implicitly converts to any <see cref="Option{T}"/>
    /// </summary>
    public static None None => default;

    /// <summary>
    /// Returns <see cref="Option{T}"/>.<see cref="Option{T}.Some">Some</see>(<paramref name="value"/>)
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
}