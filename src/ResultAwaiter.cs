namespace ScrubJay.Functional;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// This struct <b>cannot</b> be made <c>readonly</c>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public struct ResultAwaiter<T> : 
    //ICriticalNotifyCompletion,
    INotifyCompletion
{
    private readonly Result<T> _result;

    public ResultAwaiter(Result<T> result)
    {
        _result = result;
    }

    // Result doesn't do any actual work itself, so it is always 'completed'
    public readonly bool IsCompleted => true;

    // This returns the Ok part of the Result back to the caller
    // and throwing an Exception otherwise is _expected_ behavior
    // as the compiler will package that into another Result<T>
    public readonly T GetResult()
    {
        return _result.OkOrThrow();
    }

    // must call the continuation or this will block forever
    public readonly void OnCompleted(Action continuation)
    {
        continuation();
    }
    //
    // // must call the continuation or this will block forever
    // public void UnsafeOnCompleted(Action continuation)
    // {
    //     continuation();
    // }
}