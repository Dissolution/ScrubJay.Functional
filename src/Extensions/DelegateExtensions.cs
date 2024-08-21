namespace ScrubJay.Functional.Extensions;

/// <summary>
/// Extensions on <see cref="Delegate"/> and <c>delegate</c>
/// </summary>
[PublicAPI]
public static class DelegateExtensions
{
    public static Func<Unit> ToUnitFunc(this Action action) => () =>
    {
        action();
        return default;
    };

    public static Func<T1, Unit> ToUnitFunc<T1>(this Action<T1> action) => arg1 =>
    {
        action(arg1);
        return default;
    };

    public static Func<T1, T2, Unit> ToUnitFunc<T1, T2>(this Action<T1, T2> action) => (arg1, arg2) =>
    {
        action(arg1, arg2);
        return default;
    };

    public static Func<T1, T2, T3, Unit> ToUnitFunc<T1, T2, T3>(this Action<T1, T2, T3> action) => (arg1, arg2, arg3) =>
    {
        action(arg1, arg2, arg3);
        return default;
    };

    public static Func<T1, T2, T3, T4, Unit> ToUnitFunc<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action) => (arg1, arg2, arg3, arg4) =>
    {
        action(arg1, arg2, arg3, arg4);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, Unit> ToUnitFunc<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action) => (arg1, arg2, arg3, arg4, arg5) =>
    {
        action(arg1, arg2, arg3, arg4, arg5);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, Unit> ToUnitFunc<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action) => (arg1, arg2, arg3, arg4, arg5, arg6) =>
    {
        action(arg1, arg2, arg3, arg4, arg5, arg6);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> ToUnitFunc<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action) => (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
    {
        action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        return default;
    };

    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> ToUnitFunc<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
        {
            action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return default;
        };

    public static Action ToAction(this Func<Unit> unitFunc) => () => unitFunc();

    public static Action<T1> ToAction<T1>(this Func<T1, Unit> unitFunc) => arg1 => unitFunc(arg1);

    public static Action<T1, T2> ToAction<T1, T2>(this Func<T1, T2, Unit> unitFunc) => (arg1, arg2) => unitFunc(arg1, arg2);

    public static Action<T1, T2, T3> ToAction<T1, T2, T3>(this Func<T1, T2, T3, Unit> unitFunc) => (arg1, arg2, arg3) => unitFunc(arg1, arg2, arg3);

    public static Action<T1, T2, T3, T4> ToAction<T1, T2, T3, T4>(this Func<T1, T2, T3, T4, Unit> unitFunc) => (arg1, arg2, arg3, arg4) => unitFunc(arg1, arg2, arg3, arg4);

    public static Action<T1, T2, T3, T4, T5> ToAction<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5, Unit> unitFunc) => (arg1, arg2, arg3, arg4, arg5) => unitFunc(arg1, arg2, arg3, arg4, arg5);

    public static Action<T1, T2, T3, T4, T5, T6> ToAction<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6, Unit> unitFunc) =>
        (arg1, arg2, arg3, arg4, arg5, arg6) => unitFunc(arg1, arg2, arg3, arg4, arg5, arg6);

    public static Action<T1, T2, T3, T4, T5, T6, T7> ToAction<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7, Unit> unitFunc) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => unitFunc(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

    public static Action<T1, T2, T3, T4, T5, T6, T7, T8> ToAction<T1, T2, T3, T4, T5, T6, T7, T8>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, Unit> unitFunc) =>
        (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => unitFunc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
}