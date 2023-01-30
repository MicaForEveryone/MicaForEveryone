namespace MicaForEveryone.Core.Interfaces;

public interface IAppLifeTimeService
{
    bool IsFirstInstance();
    
    Task InitializeRuleServiceAsync();
    void ShutdownRuleService();
    
    Task RunViewServiceAsync();
    void ShutdownViewService();

    void OpenSettingsWindow();
}