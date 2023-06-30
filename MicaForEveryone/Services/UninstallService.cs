using System.Diagnostics;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal static class UninstallService
    {
        private static readonly Mutex UninstallationMutex = new(false, "MicaForEveryone_UninstallService");

        public static void Run()
        {
            var startupService = Program.Container.GetRequiredService<IStartupService>();
            var taskSchedulerService = Program.Container.GetRequiredService<ITaskSchedulerService>();

            var dialogService = Program.Container.GetRequiredService<IDialogService>();

            // remove startup entry
            startupService.InitializeAsync().Wait();
            if (startupService is { IsAvailable: true, IsEnabled: true })
            {
                startupService.SetStateAsync(false).Wait();
            }

            // remove task scheduler entry
            if (taskSchedulerService.IsRunAsAdminTaskCreated())
            {
                // Elevate process if we are not elevated
                if (taskSchedulerService.IsAvailable() == false)
                {
                    var elevated = ElevateProcess();
                    
                    if (elevated == null)
                    {
                        dialogService.RunErrorDialog(
                            "Error while elevating process!",
                            "Administrator privilege is needed to remove Task Scheduler entry for running as Administrator on startup but elevating process failed. The entry is not removed now, do it yourself!",
                            400,
                            300);
                    }
                    else
                    {
                        elevated.WaitForExit();
                    }

                    return;
                }

                taskSchedulerService.RemoveRunAsAdminTask();
            }
        }

        private static Process? ElevateProcess()
        {
            // using a mutex to prevent it from running itself again and again if something is going wrong with process elevation
            if (UninstallationMutex.WaitOne(0) == false) return null;

            try
            {
                return Process.Start(new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName, "--uninstall")
                {
                    Verb = "runas",
                    UseShellExecute = true,
                });
            }
            catch
            {
                return null;
            }
        }
    }
}
