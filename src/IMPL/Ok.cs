namespace ScrubJay.Functional.IMPL;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Ok<T>
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
