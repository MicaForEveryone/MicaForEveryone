using Microsoft.UI.Dispatching;
using System;
using System.Threading;
using WinRT;

namespace MicaForEveryone.App.Dispatching;

public partial class DispatcherQueueSynchronizationContext : SynchronizationContext
{
    private readonly DispatcherQueue _dispatcherQueue;

    public DispatcherQueueSynchronizationContext(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        throw new NotSupportedException("'SynchronizationContext.Send' is not supported.");
    }

    public override unsafe void Post(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);

        DispatcherQueueProxyHandler* dispatcherQueueProxyHandler = DispatcherQueueProxyHandler.Create(d, state);
        int hresult;

        try
        {
            IDispatcherQueue* dispatcherQueue = (IDispatcherQueue*)((IWinRTObject)_dispatcherQueue).NativeObject.ThisPtr;
            bool success;

            // Note: we're intentionally ignoring the retval for 'DispatcherQueue::TryEnqueue'.
            // This matches the behavior for the equivalent type on WinUI 3 as well.
            hresult = dispatcherQueue->TryEnqueue(dispatcherQueueProxyHandler, &success);

            GC.KeepAlive(_dispatcherQueue);
        }
        finally
        {
            dispatcherQueueProxyHandler->Release();
        }

        ExceptionHelpers.ThrowExceptionForHR(hresult);
    }

    public override SynchronizationContext CreateCopy()
    {
        return new DispatcherQueueSynchronizationContext(_dispatcherQueue);
    }
}
