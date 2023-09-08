using MicaForEveryone.App.Dispatching;
using MicaForEveryone.CoreUI;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;

namespace MicaForEveryone.App.Services;

public class DispatchingService : IDispatchingService
{
    private DispatcherQueue _dispatcherQueue;

    public DispatchingService(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;
    }

    public ValueTask YieldAsync()
        => new ValueTask(new DispatcherQueueValueTaskSource(_dispatcherQueue), 0);
}
