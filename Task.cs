using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable
public class Task<TResult> : Task, ITask<TResult>
{
    protected Func<TResult>? Callback { get => callback as Func<TResult>; }
    public TResult? Result { get { if (result != null) { return (TResult)result; } return default(TResult); } protected set { result = value; } }
    public Task(ITaskObservable processor) : base(processor)
    {

    }
    public Task() : base()
    {

    }
    public Task(Processor processor, TResult? result) : this(processor)
    {
        Result = result;
    }
    public void OnCompleted(TResult result)
    {
        EndTask();
        this.result = result;
        //FinishCompletion();
    }
    public ITask<TResult> Then(Func<TResult> callback) 
    {
        Action cb = () => { result = callback(); };
        Then(cb);
        return this;
        //Func<object> func = () => { return callback()!; };
        //this.callback += func;
        //return this;
    }

    public static Task<TResult> ReturnWithResult(ITask task, TResult result)
    {
        Task<TResult> resultOfTask = new();
        task.Await().Then(() => { resultOfTask.OnCompleted(result); });
        return resultOfTask;
    }

    public static ITask<TResult> CompletedTask(TResult result)
    {
        var task = new Task<TResult>();
        task.Result = result;
        return task;
    }

    //public void OnCompleted()
    //{
    //    EndTask();
    //}
}

public class Task : Disposable, ITask, ITaskObserver
{
    protected object? result;
    protected Func<object>? callback;
    //private bool disposedValue;
    protected List<ITaskObserver> observers;
    protected ITaskObservable? processor;
    public bool IsTaskProcessing { get; set; }
    public virtual bool Completed { get { if (result != null) { return true; } return false; } protected set { result = value; } }
    public Task(ITaskObservable processor) : this()
    {
        this.processor = processor;
        Start();

    }
    public Task()
    {
        //this.processor = processor;
        IsTaskProcessing = false;
        observers = new();
    }

    public virtual void EndTask()
    {
        if (IsTaskProcessing)
        {
            FinishCurrentTask();
            result = IsTaskProcessing;

            IsTaskProcessing = false;
            if (observers.Count > 0)
            {
                while (observers.Count != 0)
                {
                    InformObserver();
                }
            }

        }
    }
    public void Start()
    {
        IsTaskProcessing = true;
    }

    protected virtual void InformObserver()
    {
        observers[0].OnCompleted();
        observers.RemoveAt(0);
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                EndTask();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Task()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    //public void Dispose()
    //{
    //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //    Dispose(disposing: true);
    //    GC.SuppressFinalize(this);
    //}

    public IDisposable Subscribe(ITaskObserver taskObserver)
    {
        if (!Completed)
        {
            observers.Add(taskObserver);
        }
        else
        {
            taskObserver.OnCompleted();
        }
        
        return this;
    }

    //public virtual void OnCompleted()
    //{
    //    OnCompleted(IsTaskProcessing);
    //    //FinishCompletion();


    //}




    public Task Await()
    {
        return this;
    }

    protected void FinishCurrentTask()
    {
        if(processor!= null)
        {
            processor.EndTask();
            //OnCompleted();
        }
    }

    public static ITask CompletedTask()
    {
        var completedTask = new Task();
        completedTask.EndTask();
        return completedTask;
    }
    public Task Then(Action callback)
    {
        Func<object> func = () => { callback(); return result!; };
        this.callback += func;
        return this;
    }
    public virtual ITask<T> Then<T>(Func<T> callback)
    {
        //var resultObserver = new Task<T>();
        //this.resultObserver = resultObserver;
        var thisTaskObsv = new Task<T>(this);
        var retTask = thisTaskObsv.Then(callback);
        Subscribe(retTask);
        //observers.Add(thisTaskObsv);
        //resultObserver = (ITask<object>)thisTaskObsv;
        return retTask;
        


        //Func<object> func = () => { return callback()!; };
        //this.callback += func;
        //return thisTaskObsv!;
    }

    public virtual void OnCompleted()
    {
        if (callback != null)
        {
            var result = callback();
            if (result is Task && !((ITask)result).Completed)
            {
                ((ITask)result).Subscribe(this);
            }
            else
            {
                //if (resultObserver != null)
                //{
                //    resultObserver.EndTask();
                //}
                EndTask();
                this.result = result;

            }
            callback = null;
        }
        else
        {
            EndTask();
        }

    }





}

//public class PostTask : Task, IPostTask
//{
//    private Func<object>? callback;
//    //private ITask<object> resultObserver;
//    //private ITask observingTask;

//    public PostTask(ITask observingTask) : base(observingTask)
//    {

//        observingTask.Subscribe(this);
//    }
//    public IPostTask Then(Action callback)
//    {
//        Func<object> func = () => { callback(); return result!; };
//        this.callback += func;
//        return this;
//    }
//    public IPostTask ThenWait(Func<ITask> callback)
//    {
//        Func<object> func = () => { return callback(); };
//        this.callback += func;
//        return this;
//    }
//    public ITask<T> Then<T>(Func<T> callback)
//    {
//        //var resultObserver = new Task<T>();
//        //this.resultObserver = resultObserver;
//        var thisTaskObsv = new Task<T>();
//        //resultObserver = (ITask<object>)thisTaskObsv;
//        Action act = () => { result = callback(); thisTaskObsv.OnCompleted((T)result!); };
//        Then(act);
        
        
//        //Func<object> func = () => { return callback()!; };
//        //this.callback += func;
//        return thisTaskObsv!;
//    }
//    public virtual void OnCompleted()
//    {
//        if(callback != null)
//        {
//            object result = callback();
//            if(result is Task && !((ITask)result).Completed)
//            {
//                ((ITask)result).Subscribe(this);
//            }
//            else 
//            {
//                //if (resultObserver != null)
//                //{
//                //    resultObserver.EndTask();
//                //}
//                EndTask();

//            }
//            callback = null;
//        }
//        else
//        {
//            EndTask();
//        }
        
//    }

//    public IPostTask? ThenWait(ITask dashTask)
//    {
//        Func<ITask> watiingTask = () => { return dashTask; };
//        return ThenWait(watiingTask);
//    }
//}


public class Disposable : IDisposable
{
    protected bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Disposable()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public interface IHandler : IDisposable//will get replaced by ITask
{
    public IAction? GetProcessingAction();

}

public interface ITask<TResult> : ITask, ITaskObserver
{
    public TResult? Result { get; }
    public void OnCompleted(TResult result);


}

public interface ITask : ITaskObservable
{
    public bool IsTaskProcessing { get; set; }
    public bool Completed { get; }

    Task Await();
    
    public IDisposable Subscribe(ITaskObserver taskObserver);
    //public void EndTask();
    //void OnCompleted(object result);
    //public void OnCompleted();

}

public interface IPostTask : ITask, ITaskObserver
{
    public IPostTask Then(Action callback);
    public ITask<T> Then<T>(Func<T> callback);
    IPostTask? ThenWait(ITask dashTask);
}

public interface ITaskObservable
{
    public void EndTask();
}

public interface ITaskObserver
{
    public void OnCompleted();
    //void OnCompleted(object result);

}
