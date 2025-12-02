// prevent attribute name conflict with `JetBrains.Annotations.NotNullAttribute`
//global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

using ScrubJay.Functional.IMPL;

namespace ScrubJay.Functional;

/// <remarks>
/// To include these methods in a single <c>.cs</c> file, add to its <c>usings</c> section:<br/>
/// <code>
/// using static ScrubJay.Functional.Prelude
/// </code><br/>
/// To include them in an entire project, add to its <c>.csproj</c> file:<br/>
/// <code>
/// &lt;ItemGroup&gt;
///     &lt;Using Include="ScrubJay.Functional.Prelude" Static="true"/&gt;
/// &lt;/ItemGroup&gt;
/// </code><br/>
/// </remarks>
[PublicAPI]
public static class Prelude
{
    public static None None
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => field;
    } = None.Default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ok<T> Ok<T>(T value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => new Ok<T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error<E> Error<E>(E error)
#if NET9_0_OR_GREATER
        where E : allows ref struct
#endif
        => new Error<E>(error);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Unit Unit() => default(Unit);
}