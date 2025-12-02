using System.Reflection;
using System.Reflection.Emit;
#if NET9_0_OR_GREATER
using BF = System.Reflection.BindingFlags;
#endif

namespace ScrubJay.Functional;

/// <summary>
/// Extensions that provide a <see cref="Stringify{T}(T?)"/> operation on <i>any</i> type.
/// </summary>
/// <remarks>
/// This is vital when interacting with <c>.NET 9.0+</c>'s generic type 'constraint' <c>allows ref struct</c>.<br/>
/// You cannot call <see cref="object.ToString()"/> on a generic value constrained with <c>allows ref struct</c>.<br/>
/// All non-<c>ref struct</c> values and many <c>ref struct</c>s provide their own <c>ToString()</c> implementation,<br/>
/// so this class creates custom delegates to call those methods (with a fallback for any type that doesn't have one).
/// </remarks>
[PublicAPI]
public static class StringifyExtensions
{
    internal static string? AsFormatString(this ReadOnlySpan<char> format)
    {
        if (format.IsEmpty)
            return null;
        return format.ToString();
    }

#if NET9_0_OR_GREATER

    // use a static class to contain the delegates!
    private static class ToStringDelegateCache<T>
        where T : allows ref struct
    {
        private static readonly Func<T, string> _fallbackToString = _ => $"{typeof(T)} instance";

        public static readonly Func<T, string> ToStringFunc = CreateToStringDelegate();

        private static MethodInfo? FindToStringMethod(Type type, BF flags)
        {
            var toStringMethod = type
                .GetMethods(flags)
                .Where(static method => method.Name == nameof(object.ToString) &&
                    method.ReturnType == typeof(string) &&
                    method.GetParameters().Length == 0)
                .FirstOrDefault();
            return toStringMethod;
        }

        private static Func<T, string> CreateToStringDelegate()
        {
            Type instanceType = typeof(T);
            MethodInfo? toStringMethod;

            // Enums
            if (instanceType.IsEnum)
            {
                // Enum instances do not have a special ToString, they use the common Enum.ToString()
                toStringMethod =
                    FindToStringMethod(typeof(Enum), BF.Public | BF.NonPublic | BF.Instance | BF.DeclaredOnly);
            }
            else
            {
                // For all other types, first look for an instance method declared exactly on that type
                toStringMethod =
                    FindToStringMethod(instanceType, BF.Public | BF.NonPublic | BF.Instance | BF.DeclaredOnly);

                // If this is a non-value, non-ref type, we can also scan higher
                if (toStringMethod is null && (!instanceType.IsByRef && !instanceType.IsValueType))
                {
                    toStringMethod = FindToStringMethod(instanceType, BF.Public | BF.NonPublic | BF.Instance);
                }
            }

            if (toStringMethod is null)
            {
                // fallback to describing the type
                return _fallbackToString;
            }

            // We have to emit a dynamic method,
            // as Expressions cannot handle ref structs
            var dyn = new DynamicMethod(
                name: $"{typeof(T).FullName}_ToString",
                attributes: MethodAttributes.Public | MethodAttributes.Static,
                callingConvention: CallingConventions.Standard,
                returnType: typeof(string),
                parameterTypes: [instanceType],
                m: typeof(StringifyExtensions).Module,
                skipVisibility: true);
            var gen = dyn.GetILGenerator();

            // first, load the instance

            // stack types
            if (instanceType.IsEnum ||
                instanceType.IsByRef ||
                instanceType.IsByRefLike ||
                instanceType.IsValueType)
            {
                // load a ref to this value
                gen.Emit(OpCodes.Ldarga_S, 0);
            }
            // heap types
            else if (instanceType.IsClass || instanceType.IsInterface)
            {
                // load this class
                gen.Emit(OpCodes.Ldarg_0);
            }
            else
            {
                // we shouldn't be able to get here
                return _fallbackToString;
            }

            // second, call the ToString Method

            // enums + byref likes we can use Constrained
            if (instanceType.IsByRef ||
                instanceType.IsEnum ||
                instanceType.IsByRefLike)
            {
                gen.Emit(OpCodes.Constrained, instanceType);
                gen.Emit(OpCodes.Callvirt, toStringMethod);
            }
            // value types we can just call
            else if (instanceType.IsValueType)
            {
                gen.Emit(OpCodes.Call, toStringMethod);
            }
            // class types we callvir to account for overloads
            else
            {
                gen.Emit(OpCodes.Callvirt, toStringMethod);
            }

            // return
            gen.Emit(OpCodes.Ret);

            // create the function
            Func<T, string> func;
            try
            {
                func = dyn.CreateDelegate<Func<T, string>>();
            }
            catch (Exception)
            {
                // fallback
                func = _fallbackToString;
            }

            return func;
        }
    }

    public static string Stringify<T>(this T? value)
        where T : allows ref struct
    {
        if (value is null)
            return string.Empty;
        return ToStringDelegateCache<T>.ToStringFunc(value);
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify<T>(this T? value)
    {
        return value?.ToString() ?? string.Empty;
    }

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify<T>(this scoped ReadOnlySpan<T> span)
    {
        return span.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify<T>(this scoped Span<T> span)
    {
        return span.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Stringify(this scoped ReadOnlySpan<char> text)
    {
        return text.ToString();
    }
}