namespace ScrubJay.Functional;

[PublicAPI]
public struct ResultAsyncMethodBuilder<T>
{
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

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        if (_stateMachine is null)
        {
            _stateMachine = stateMachine; // deref
            _stateMachine.SetStateMachine(_stateMachine);
        }

#if NETFRAMEWORK || NETSTANDARD
        stateMachine.MoveNext();
#else
        ExecutionContext? prevExecCtx = Thread.CurrentThread.ExecutionContext;
        SynchronizationContext? prevSyncCtx = SynchronizationContext.Current;

        try
        {
            stateMachine.MoveNext();
        }
        finally
        {
            if (prevSyncCtx != null && prevSyncCtx != SynchronizationContext.Current)
                SynchronizationContext.SetSynchronizationContext(prevSyncCtx);
            if (prevExecCtx != null && prevExecCtx != Thread.CurrentThread.ExecutionContext)
                ExecutionContext.Restore(prevExecCtx);
        }
#endif
    }

    public void SetResult(T value)
    {
        _result = Result<T>.Ok(value!);
    }

    public void SetException(Exception exception)
    {
        _result = Result<T>.Error(exception);
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
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
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
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
    }
}