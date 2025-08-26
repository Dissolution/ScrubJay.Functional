namespace ScrubJay.Functional.IMPL;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
public readonly ref struct Ok<T>
    where T : allows ref struct
#else
public readonly struct Ok<T>
#endif
{
    public readonly T Value;

    public Ok(T value)
    {
        Value = value;
    }

    public void Deconstruct(out T value)
    {
        value = Value;
    }

    public override string ToString()
    {
        return $"Ok<{typeof(T)}>";
    }
}
