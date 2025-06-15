namespace MicaForEveryone.CoreUI;

public interface ILoggingService
{
    void LogException(Exception exception);

    Task FlushAsync();
}