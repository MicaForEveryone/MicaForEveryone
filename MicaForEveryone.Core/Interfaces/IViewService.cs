namespace MicaForEveryone.Core.Interfaces;

public interface IAppLifeTimeService
{
    bool IsFirstInstance();
    
    Task InitializeRuleServiceAsync();
    void ShutdownRuleService();
    
    bool IsViewServiceRunning { get; }
    Task RunViewServiceAsync();
    void ShutdownViewService();

    void OpenSettingsWindow();
}