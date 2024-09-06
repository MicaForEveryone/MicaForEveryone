using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MicaForEveryone.App.Services;

/// <summary>
/// An awaiter returned from <see cref="GetAwaiter(TaskScheduler)"/>.
/// </summary>
public readonly struct TaskSchedulerAwaiter : ICriticalNotifyCompletion
{
    /// <summary>
    /// The scheduler for continuations.
    /// </summary>
    private readonly TaskScheduler scheduler;

    /// <summary>
    /// A value indicating whether <see cref="IsCompleted"/>
    /// should always return false.
    /// </summary>
    private readonly bool alwaysYield;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskSchedulerAwaiter"/> struct.
    /// </summary>
    /// <param name="scheduler">The scheduler for continuations.</param>
    /// <param name="alwaysYield">A value indicating whether the caller should yield even if
    /// already executing on the desired task scheduler.</param>
    public TaskSchedulerAwaiter(TaskScheduler scheduler, bool alwaysYield = false)
    {
        this.scheduler = scheduler;
        this.alwaysYield = alwaysYield;
    }

    /// <summary>
    /// Gets a value indicating whether no yield is necessary.
    /// </summary>
    /// <value><see langword="true" /> if the caller is already running on that TaskScheduler.</value>
    public bool IsCompleted
    {
        get
        {
            if (this.alwaysYield)
            {
                return false;
            }

            // We special case the TaskScheduler.Default since that is semantically equivalent to being
            // on a ThreadPool thread, and there are various ways to get on those threads.
            // TaskScheduler.Current is never null.  Even if no scheduler is really active and the current
            // thread is not a threadpool thread, TaskScheduler.Current == TaskScheduler.Default, so we have
            // to protect against that case too.
            bool isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
            return (this.scheduler == TaskScheduler.Default && isThreadPoolThread)
                || (this.scheduler == TaskScheduler.Current && TaskScheduler.Current != TaskScheduler.Default);
        }
    }

    /// <summary>
    /// Schedules a continuation to execute using the specified task scheduler.
    /// </summary>
    /// <param name="continuation">The delegate to invoke.</param>
    public void OnCompleted(Action continuation)
    {
        if (this.scheduler == TaskScheduler.Default)
        {
            ThreadPool.QueueUserWorkItem(state => ((Action)state!)(), continuation);
        }
        else
        {
            Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }
    }

    /// <summary>
    /// Schedules a continuation to execute using the specified task scheduler
    /// without capturing the ExecutionContext.
    /// </summary>
    /// <param name="continuation">The action.</param>
    public void UnsafeOnCompleted(Action continuation)
    {
        if (this.scheduler == TaskScheduler.Default)
        {
            ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state!)(), continuation);
        }
        else
        {
            Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }
    }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public void GetResult()
    {
    }
}

public static class TaskSchedulerAwaitExtensions
{
    /// <summary>
    /// Gets an awaiter that schedules continuations on the specified task scheduler.
    /// </summary>
    /// <param name="scheduler">The task scheduler.</param>
    /// <returns>The awaiter.</returns>
    public static TaskSchedulerAwaiter GetAwaiter(this TaskScheduler scheduler) => new(scheduler);
}