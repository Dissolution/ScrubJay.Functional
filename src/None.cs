namespace ScrubJay.Functional;

/// <summary>
/// The None part of <see cref="Option{T}"/>.<see cref="Option{T}.None"/>
/// </summary>
/// <remarks>
/// None acts implicitly <c>false</c>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 0)]
public readonly struct None :
#if NET7_0_OR_GREATER
    IEqualityOperators<None, None, bool>,
    IBitwiseOperators<None, None, bool>,
#endif
    IEquatable<None>
{
    // None is false
    public static implicit operator bool(None none) => false;

    public static bool operator true(None none) => false;
    public static bool operator false(None none) => true;

    public static bool operator &(None left, None right) => false;
    public static bool operator |(None left, None right) => false;
    public static bool operator ^(None left, None right) => false;

    [Obsolete("Cannot apply operator '~' to operand of type 'None'", true)]
    public static bool operator ~(None none) => throw new NotSupportedException();
    
    // All Nones are the same
    public static bool operator ==(None left, None right) => true;
    public static bool operator !=(None left, None right) => false;
    
    public static readonly None Default = default;
    
    public bool Equals(None none) => true;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is None;
    public override int GetHashCode() => 0;
    public override string ToString() => nameof(None);
}