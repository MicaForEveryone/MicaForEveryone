using MicaForEveryone.App.Dispatching;
using MicaForEveryone.CoreUI;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;

namespace MicaForEveryone.App.Services;

public class DispatchingService : IDispatchingService
{
    private DispatcherQueueValueTaskSource _dispatcherQueueValueTaskSource;

    public DispatchingService(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueueValueTaskSource = new(dispatcherQueue);
    }

    public ValueTask YieldAsync()
        => new ValueTask(_dispatcherQueueValueTaskSource, 0);
}
