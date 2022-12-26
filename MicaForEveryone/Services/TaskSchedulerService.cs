using System;
using System.Linq;
using System.Security.Principal;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using Microsoft.Win32.TaskScheduler;

namespace MicaForEveryone.Services
{
    internal class TaskSchedulerService : ITaskSchedulerService
    {
        private string GetExecutableLocation()
        {
            if (Application.IsPackaged)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\WindowsApps\\mfe.exe";
            }

            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        public void CreateRunAsAdminTask()
        {
            using var taskService = new TaskService();
            if (taskService.RootFolder.Tasks.Exists("MicaForEveryone_RunAsAdmin"))
            {
                taskService.RootFolder.Tasks.First((task) => task.Name == "MicaForEveryone_RunAsAdmin").Enabled = true;
                return;
            }
            var task = taskService.NewTask();
            task.RegistrationInfo.Description = "Run Mica For Everyone as Administrator on Startup";
            task.Principal.RunLevel = TaskRunLevel.Highest;
            task.Triggers.Add(new LogonTrigger());
            task.Actions.Add(new ExecAction(GetExecutableLocation()));
            // Allow the task to start on battery power and run for as long as it wants.
            task.Settings.DisallowStartIfOnBatteries = false;
            task.Settings.StopIfGoingOnBatteries = false;
            task.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            var registeredTask = taskService.RootFolder.RegisterTaskDefinition("MicaForEveryone_RunAsAdmin", task);
            registeredTask.Enabled = true;
        }

        public void RemoveRunAsAdminTask()
        {
            using var taskService = new TaskService();
            taskService.RootFolder.DeleteTask("MicaForEveryone_RunAsAdmin");
        }

        public bool IsRunAsAdminTaskCreated()
        {
            using var taskService = new TaskService();
            return taskService.RootFolder.Tasks.Count((task) => task.Name == "MicaForEveryone_RunAsAdmin") > 0;
        }

        public bool IsRunAsAdminTaskEnabled()
        {
            using var taskService = new TaskService();
            return taskService.RootFolder.Tasks.Count((task) => task.Name == "MicaForEveryone_RunAsAdmin" && task.Enabled) == 1;
        }

        public bool IsAvailable()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}