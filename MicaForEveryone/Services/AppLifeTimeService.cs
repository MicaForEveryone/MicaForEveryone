using System;
using System.Threading;
using System.Threading.Tasks;

using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class AppLifeTimeService : IAppLifeTimeService, IDisposable
    {
        private readonly Mutex _singleInstanceMutex = new(true, "Mica For Everyone");
        
        private readonly IStartupService _startupService;
        private readonly ISettingsService _settingsService;
        private readonly IUiSettingsService _uiSettingsService;
        private readonly IRuleService _ruleService;
        private readonly IViewService _viewService;
        
        private App? _app;
        private Task? _uiThread;
        
        public AppLifeTimeService(IStartupService startupService, ISettingsService settingsService, IUiSettingsService uiSettingsService, IRuleService ruleService, IViewService viewService)
        {
            _startupService = startupService;
            _settingsService = settingsService;
            _uiSettingsService = uiSettingsService;
            _ruleService = ruleService;
            _viewService = viewService;
        }

        public bool IsViewServiceRunning => _uiThread != null;

        public bool IsFirstInstance()
        {
            return _singleInstanceMutex.WaitOne(0, true);
        }

        public void OpenSettingsWindow()
        {
            var msg = Win32.Window.RegisterWindowMessage(Views.MainWindow.OpenSettingsMessage);
            Win32.Window.Broadcast(msg);
        }

        public async Task InitializeRuleServiceAsync()
        {
            // load settings first
            await _settingsService.InitializeAsync();

            // initialize startup service
            _ = _startupService.InitializeAsync();

            // start rule service
            await _ruleService.MatchAndApplyRuleToAllWindowsAsync();
            _ruleService.StartService();
        }

        public async Task RunViewServiceAsync()
        {
            if (_uiThread != null)
            {
                await _uiThread;
                return;
            }

            _uiThread = Task.Run(() =>
            {
                _app ??= new App();
                
                _uiSettingsService.Load();
                
                _viewService.Initialize(_app);
                _app.RegisterExceptionHandlers();
                
                _app.Run();
                
                _app.UnregisterExceptionHandlers();
                _viewService.Unload();
            });

            await _uiThread;
            
            _uiThread = null;
        }

        public void ShutdownRuleService()
        {
            _ruleService.StopService();
        }

        public void ShutdownViewService()
        {
            if (_app == null || _uiThread == null) return;
            if (Task.CurrentId == _uiThread.Id) throw new InvalidOperationException();
            
            _uiThread.Wait();
            _uiThread.Dispose();
            _app.Dispose();
            _app = null;
            _uiThread = null;
        }
        
        public void Dispose()
        {
            if (_uiThread?.Status == TaskStatus.Running)
            {
                ShutdownViewService();
            }
            if (_ruleService.IsRunning)
            {
                ShutdownRuleService();
            }
            _singleInstanceMutex.Dispose();
            _app?.Dispose();
            _uiThread?.Dispose();
        }
    }
}