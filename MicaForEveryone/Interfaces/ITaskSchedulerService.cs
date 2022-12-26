using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
