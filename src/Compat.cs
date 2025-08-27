using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace ScrubJay.Functional;

#if NET9_0_OR_GREATER
internal static class Compat<T>
    where T : allows ref struct
{
    private static Func<T, string> _toString;
    
    static Compat()
    {
        var toStringMethod = typeof(T)
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name == "ToString")
            .Where(m => m.GetParameters().Length == 0)
            .Where(m => m.ReturnType == typeof(string))
            .FirstOrDefault();

        if (toStringMethod is not null)
        {
            var method = new DynamicMethod(
                $"{typeof(T).Name}_toString",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(string),
                [typeof(T)],
                typeof(Unit).Module,
                true);
            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(opcode: OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);
            _toString = method.CreateDelegate<Func<T, string>>();
        }
        else
        {
            Debugger.Break();
        }

    }

    public static string ToString(T? value)
    {
        if (value is null)
            return string.Empty;
        return _toString(value);
    }
}
#else
internal static class Compat<T>
{
    public static string ToString<T>(T? value) => value?.ToString() ?? "";
}
#endif