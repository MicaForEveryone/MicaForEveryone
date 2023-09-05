using MicaForEveryone.App.Service;
using MicaForEveryone.App.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using WinUIEx;
using static MicaForEveryone.PInvoke.Generic;
using static MicaForEveryone.PInvoke.Macros;
using static MicaForEveryone.PInvoke.Messaging;
using static MicaForEveryone.PInvoke.Modules;
using static MicaForEveryone.PInvoke.Monitor;
using static MicaForEveryone.PInvoke.NotifyIcon;
using static MicaForEveryone.PInvoke.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public unsafe partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Services.GetRequiredService<MainAppService>().Initialize();
    }
}
