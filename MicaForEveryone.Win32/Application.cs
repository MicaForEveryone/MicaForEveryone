using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32
{
    public class Application
    {
        private const uint APPMODEL_ERROR_NO_PACKAGE = 15700;

        public static IntPtr InstanceHandle { get; } = NativeMethods.GetCurrentModule();

        public static bool IsPackaged { get; } = GetCurrentPackageName() != null;

        public static string GetCurrentPackageName()
        {
            var length = 0u;
            NativeMethods.GetCurrentPackageFullName(ref length);

            var result = new StringBuilder((int)length);
            var error = NativeMethods.GetCurrentPackageFullName(ref length, result);

            if (error == APPMODEL_ERROR_NO_PACKAGE)
                return null;

            return result.ToString();
        }

        public static IntPtr AddDynamicDependency(string packageName)
        {
            var error = NativeMethods.TryCreatePackageDependency(IntPtr.Zero, packageName, new PACKAGE_VERSION(),
                PackageDependencyProcessorArchitectures.PackageDependencyProcessorArchitectures_X64,
                PackageDependencyLifetimeKind.PackageDependencyLifetimeKind_Process, null,
                CreatePackageDependencyOptions.CreatePackageDependencyOptions_None, out var dependencyId);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
            error = NativeMethods.AddPackageDependency(dependencyId, 1,
                AddPackageDependencyOptions.AddPackageDependencyOptions_None, out var context, out var packageFullName);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }

            Marshal.FreeHGlobal(packageFullName);
            return context;
        }

        public static void RemoveDynamicDependency(IntPtr context)
        {
            var error = NativeMethods.RemovePackageDependency(context);
            if (error != 0)
            {
                throw new Win32Exception(error);
            }
        }

        private Window _mainWindow;

        public Dispatcher Dispatcher { get; } = new();

        /// <summary>
        /// Run main loop with given main window
        /// </summary>
        public void Run(Window window)
        {
            _mainWindow = window;

            BeforeRun?.Invoke(this, EventArgs.Empty);

            _mainWindow.Destroy += Window_OnDestroy;

            while (NativeMethods.GetMessageW(out var msg, IntPtr.Zero, 0, 0))
            {
                var processed = false;
                BeforeTranslateMessage?.Invoke(window, ref msg, ref processed);
                if (processed) continue;
                NativeMethods.TranslateMessage(msg);
                NativeMethods.DispatchMessageW(msg);
                Dispatcher.Invoke();
            }

            _mainWindow.Destroy -= Window_OnDestroy;

            BeforeExit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Post WM_QUIT to stop main loop
        /// </summary>
        public void Exit()
        {
            NativeMethods.PostQuitMessage(0);
        }

        private void Window_OnDestroy(object sender, EventArgs e)
        {
            // stop main loop when window destroyed
            Exit();
        }

        public event EventHandler BeforeRun;
        public event MessageLoopHandler BeforeTranslateMessage;
        public event EventHandler BeforeExit;
    }

    public delegate void MessageLoopHandler(Window window, ref MSG message, ref bool processed);
}
