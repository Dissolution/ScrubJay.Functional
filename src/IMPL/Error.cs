namespace ScrubJay.Functional.IMPL;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
public readonly ref struct Error<E>
    where E : allows ref struct
#else
public readonly struct Error<E>
#endif
{
    public readonly E Value;

    public Error(E value)
    {
        Value = value;
    }

    public void Deconstruct(out E error)
    {
        error = Value;
    }

    public override string ToString()
    {
        return $"Error<{typeof(E)}>";
    }
}
