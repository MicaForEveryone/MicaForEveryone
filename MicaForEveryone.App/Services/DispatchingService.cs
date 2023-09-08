using MicaForEveryone.App.Dispatching;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;

namespace MicaForEveryone.App.Services;

public interface IDispatchingService
{
    ValueTask YieldAsync();
}

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
