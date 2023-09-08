namespace MicaForEveryone.CoreUI;

public interface IDispatchingService
{
    ValueTask YieldAsync();
}