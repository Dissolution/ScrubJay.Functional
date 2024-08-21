namespace ScrubJay.Functional.Extensions;

public static class NullableExtensions
{
    /// <summary>
    /// Converts this <see cref="Nullable{T}"/> into an <see cref="Option{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> AsOption<T>(this T? nullable)
        where T : struct
    {
        if (nullable.HasValue)
            return Option<T>.Some(nullable.Value);
        return Option<T>.None;
    }
}