namespace ScrubJay.Functional;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public struct ResultAsyncMethodBuilder<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ResultAsyncMethodBuilder<T> Create() => new();

    private IAsyncStateMachine? _stateMachine;
    private Result<T> _result;

    public Result<T> Task
    {
        get
        {
            if (_stateMachine is not null)
            {
                _stateMachine.MoveNext();
                Debug.Assert(_result != default(Result<T>));
                return _result;
            }
            else
            {
                Debug.Assert(_result != default(Result<T>));
                return _result;
            }
        }
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        if (_stateMachine is null)
        {
            _stateMachine = stateMachine; // deref
            _stateMachine.SetStateMachine(_stateMachine);
        }
    }

    public void Start<SM>(ref SM stateMachine)
        where SM : IAsyncStateMachine
    {
        // We need to store this statemachine
        // so we can call MoveNext on it later
        // in order to 'await' anything before us
        _stateMachine = stateMachine;
        stateMachine.SetStateMachine(_stateMachine);
        stateMachine.MoveNext();
        /*
        if (_stateMachine is null)
        {
            _stateMachine = stateMachine; // deref
            _stateMachine.SetStateMachine(_stateMachine);
        }

//#if NETFRAMEWORK || NETSTANDARD
        stateMachine.MoveNext();
// #else
//         ExecutionContext? prevExecCtx = Thread.CurrentThread.ExecutionContext;
//         SynchronizationContext? prevSyncCtx = SynchronizationContext.Current;
//
//         try
//         {
//             stateMachine.MoveNext();
//         }
//         finally
//         {
//             if (prevSyncCtx != null && prevSyncCtx != SynchronizationContext.Current)
//                 SynchronizationContext.SetSynchronizationContext(prevSyncCtx);
//             if (prevExecCtx != null && prevExecCtx != Thread.CurrentThread.ExecutionContext)
//                 ExecutionContext.Restore(prevExecCtx);
//         }
// #endif
*/
    }

    public void SetResult(T value)
    {
        _result = Result<T>.Ok(value!);
    }

    public void SetException(Exception exception)
    {
        _result = Result<T>.Error(exception);
    }

    private Action CreateCompletionAction<SM>(
        ref SM stateMachine)
        where SM : IAsyncStateMachine
    {
        var boxedStateMachine = stateMachine;
        return boxedStateMachine.MoveNext;
    }
    
    public void AwaitOnCompleted<A, SM>(
        ref A awaiter,
        ref SM stateMachine)
        where A : INotifyCompletion
        where SM : IAsyncStateMachine
    {
        var completion = CreateCompletionAction<SM>(ref stateMachine);
        awaiter.OnCompleted(completion);
        /*
        Action continuation;

        if (_stateMachine is null)
        {
            continuation = stateMachine.MoveNext;
        }
        else
        {
            continuation = _stateMachine.MoveNext;
        }

        awaiter.OnCompleted(continuation);
        */
    }

    public void AwaitUnsafeOnCompleted<A, SM>(
        ref A awaiter,
        ref SM stateMachine)
        where A : ICriticalNotifyCompletion
        where SM : IAsyncStateMachine
    {
        var completion = CreateCompletionAction<SM>(ref stateMachine);
        awaiter.OnCompleted(completion);
        /*

        Action continuation;

        if (_stateMachine is null)
        {
            continuation = stateMachine.MoveNext;
        }
        else
        {
            continuation = _stateMachine.MoveNext;
        }

        awaiter.UnsafeOnCompleted(continuation);
        */
    }
}