using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

using MicaForEveryone.Interfaces;

namespace MicaForEveryone.Services
{
    internal class UwpStartupService : IStartupService
    {
        private const string TaskName = "MicaForEveryone";

        private StartupTask _task;

        public async Task InitializeAsync()
        {
            _task = await StartupTask.GetAsync(TaskName);
        }

        public bool IsAvailable
        {
            get
            {
                if (_task == null)
                    return false;
                return _task.State is not (StartupTaskState.DisabledByUser
                    or StartupTaskState.DisabledByPolicy
                    or StartupTaskState.EnabledByPolicy);
            }
        }

        public bool IsEnabled
        {
            get
            {
                if (_task == null)
                    return false;
                return _task.State is StartupTaskState.Enabled
                    or StartupTaskState.EnabledByPolicy;
            }
        }

        public async Task<bool> SetStateAsync(bool state)
        {
            if (_task == null)
                return false;
            if (state == true)
            {
                var result = await _task.RequestEnableAsync();
                return result is StartupTaskState.Enabled
                    or StartupTaskState.EnabledByPolicy;
            }

            _task.Disable();
            return false;
        }

        public void Dispose()
        {
        }
    }
}
