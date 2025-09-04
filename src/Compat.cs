using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace ScrubJay.Functional;

internal static class Compat
{
    public static string ToString<T>(in T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return Compat<T>.ToString(in value);
    }
}

#if NET9_0_OR_GREATER

internal static class Compat<T>
    where T : allows ref struct
{
    private delegate string CompatToString(in T value);

    private static readonly CompatToString _toString;

    static Compat()
    {
        var type = typeof(T);

        var toStringMethods = type
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name == "ToString")
            .Where(m => m.GetParameters().Length == 0)
            .Where(m => m.ReturnType == typeof(string))
            .ToList();

        if (toStringMethods.Count != 1)
        {
            Debugger.Break();
            throw new NotImplementedException();
        }
        
        var toStringMethod = toStringMethods[0];
        
        var method = new DynamicMethod(
            $"{typeof(T).Name}_toString",
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            typeof(string),
            [typeof(T).MakeByRefType()],
            typeof(Unit).Module,
            true);
        
        var gen = method.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        // stack is:    &T

        if (type.IsByRef || type.IsByRefLike)
        {
            gen.Emit(OpCodes.Constrained, typeof(T));
            gen.Emit(OpCodes.Callvirt, toStringMethod);
        }
        else if (type.IsValueType)
        {
            gen.Emit(OpCodes.Constrained, typeof(T));
            gen.Emit(OpCodes.Callvirt, toStringMethod);
        }
        else if (type.IsClass || type.IsInterface)
        {
            gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Callvirt, toStringMethod);
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }
        
        gen.Emit(OpCodes.Ret);
        _toString = method.CreateDelegate<CompatToString>();
    }


public static string ToString(in T? value)
{
    if (value is null)
        return string.Empty;
    return _toString(in value);
}

}
#else
internal static class Compat<T>
{
    public static string ToString(in T? value)
    {
        if (value is null)
            return string.Empty;
        return value.ToString()!;
    }
}
#endif