﻿namespace ScrubJay.Functional;

/// <summary>
/// Global helper methods to support <see cref="Option{T}"/> and <see cref="Result{TOk, TError}"/>
/// </summary>
/// <remarks>
/// To include this in a single <c>.cs</c> file, add the following to the very top:<br/>
/// <c>using static ScrubJay.GlobalHelper;</c><br/>
/// <br/>
/// To include this in an entire project, add the following inside of an <c>&lt;ItemGroup/&gt;</c>:<br/>
/// <c>&lt;Using Include="ScrubJay.GlobalHelper" Static="true"/&gt;</c><br/>
/// </remarks>
[PublicAPI]
public static class GlobalHelper
{
    /// <inheritdoc cref="Option{T}.Some"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
}