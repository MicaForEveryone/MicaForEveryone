using Microsoft.UI.Dispatching;
using System;
using System.Threading;
using WinRT;

#nullable enable

namespace MicaForEveryone.App.Dispatching;

/// <summary>
/// DispatcherQueueSynchronizationContext allows developers to await calls and get back onto the
/// UI thread. Needs to be installed on the UI thread through DispatcherQueueSynchronizationContext.SetForCurrentThread
/// </summary>
public partial class DispatcherQueueSynchronizationContext : SynchronizationContext
{
    private readonly DispatcherQueue m_dispatcherQueue;

    public DispatcherQueueSynchronizationContext(DispatcherQueue dispatcherQueue)
    {
        m_dispatcherQueue = dispatcherQueue;
    }

    /// <inheritdoc/>
    public override unsafe void Post(SendOrPostCallback d, object? state)
    {
        if (d is null)
        {
            throw new ArgumentNullException(nameof(d));
        }

        DispatcherQueueProxyHandler* dispatcherQueueProxyHandler = DispatcherQueueProxyHandler.Create(d, state);
        int hResult;

        try
        {
            IDispatcherQueue* dispatcherQueue = (IDispatcherQueue*)((IWinRTObject)m_dispatcherQueue).NativeObject.ThisPtr;
            bool success;

            hResult = dispatcherQueue->TryEnqueue(dispatcherQueueProxyHandler, &success);

            GC.KeepAlive(this);
        }
        finally
        {
            dispatcherQueueProxyHandler->Release();
        }

        if (hResult != 0)
        {
            ExceptionHelpers.ThrowExceptionForHR(hResult);
        }
    }

    /// <inheritdoc/>
    public override void Send(SendOrPostCallback d, object? state)
    {
        throw new NotSupportedException("Send not supported");
    }

    /// <inheritdoc/>
    public override SynchronizationContext CreateCopy()
    {
        return new DispatcherQueueSynchronizationContext(m_dispatcherQueue);
    }
}