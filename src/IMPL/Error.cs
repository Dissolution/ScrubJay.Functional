namespace ScrubJay.Functional.IMPL;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Error<E>
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
