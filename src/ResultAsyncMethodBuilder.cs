using System.Diagnostics;

namespace ScrubJay.Functional;

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
                Debugger.Break();
                throw new NotImplementedException();
            }
        }
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        Debugger.Break();
        throw new NotImplementedException();
    }

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        if (_stateMachine is null)
        {
            _stateMachine = stateMachine; // deref
            _stateMachine.SetStateMachine(_stateMachine);
        }
        
        ExecutionContext previous = Thread.CurrentThread.ExecutionContext!;
        try
        {
            stateMachine.MoveNext();
        }
        finally
        {
#if NETFRAMEWORK || NETSTANDARD
            throw new NotImplementedException();
#else
            ExecutionContext.Restore(previous);
#endif
        }
    }

    public void SetResult(T value)
    {
        _result = Result<T>.Ok(value!);
    }

    public void SetException(Exception exception)
    {
        _result = Result<T>.Error(exception!);
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        if (_stateMachine is null)
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        Action continuation = _stateMachine.MoveNext;
        awaiter.OnCompleted(continuation);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, 
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        if (_stateMachine is null)
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        Action continuation = _stateMachine.MoveNext;
        awaiter.UnsafeOnCompleted(continuation);
    }
}