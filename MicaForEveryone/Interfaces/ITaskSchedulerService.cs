namespace MicaForEveryone.Interfaces
{
    public interface ITaskSchedulerService
    {
        void CreateRunAsAdminTask();
        
        void RemoveRunAsAdminTask();

        bool IsRunAsAdminTaskCreated();

        bool IsRunAsAdminTaskEnabled();

        bool IsAvailable();
    }
}
