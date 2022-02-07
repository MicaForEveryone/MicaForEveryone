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
        private StartupTask _startupTask;

        private async Task<StartupTask> GetStartupTaskAsync()
        {
            if (_startupTask == null)
            {
                _startupTask = (await StartupTask.GetForCurrentPackageAsync())[0];
            }
            return _startupTask;
        }

        public async Task<bool> GetEnabledAsync()
        {
            var startupTask = await GetStartupTaskAsync();
            return startupTask.State is
                StartupTaskState.Enabled or
                StartupTaskState.EnabledByPolicy;
        }

        public async Task<bool> SetEnabledAsync(bool state)
        {
            var startupTask = await GetStartupTaskAsync();
            if (state)
            {
                var result = await startupTask.RequestEnableAsync();
                return result is StartupTaskState.Enabled or
                    StartupTaskState.EnabledByPolicy;
            }
            else
            {
                startupTask.Disable();
                return false;
            }
        }
    }
}
