namespace MicaForEveryone.Core.Interfaces;

public interface IAppLifeTimeService
{
    bool IsFirstInstance();
    void OpenSettingsWindow();
    Task InitializeRuleServiceAsync();
    Task RunViewServiceAsync();
    void ShutdownRuleService();
    void ShutdownViewService();
}