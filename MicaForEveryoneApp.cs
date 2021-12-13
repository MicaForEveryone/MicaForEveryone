using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace MicaForEveryone
{
    public class MicaForEveryoneApp : ApplicationContext
    {
        private readonly IContainer _components = new Container();

        private readonly RuleHandler _ruleHandler = new();

        public MicaForEveryoneApp()
        {
            InitializeComponents();

            _ruleHandler.ApplyToAllWindows();
        }

        private void InitializeComponents()
        {
            var reapplyMenuItem = new ToolStripMenuItem(
                "Reapply rules",
                null,
                (sender, args) => _ruleHandler.ApplyToAllWindows()
            );

            var themeModeSubMenu = new ToolStripMenuItem
            {
                Text = "Theme",
                DropDownItems =
                {
                    {"Default", null, (sender, args) => _ruleHandler.GlobalRule.Theme = ThemeMode.Default},
                    {"Light", null, (sender, args) => _ruleHandler.GlobalRule.Theme = ThemeMode.ForceLight},
                    {"Dark", null, (sender, args) => _ruleHandler.GlobalRule.Theme = ThemeMode.ForceDark},
                }
            };

            var micaModeSubMenu = new ToolStripMenuItem
            {
                Text = "Mica",
                DropDownItems =
                {
                    {"Default", null, (sender, args) => _ruleHandler.GlobalRule.Mica = MicaMode.Default},
                    {"Force Enable", null, (sender, args) => _ruleHandler.GlobalRule.Mica = MicaMode.ForceMica},
                    {"Force Disable", null, (sender, args) => _ruleHandler.GlobalRule.Mica = MicaMode.ForceNoMica},
                }
            };

            var exitMenuItem = new ToolStripMenuItem(
                "Exit",
                null,
                (sender, args) => Application.Exit()
            );

            var notifyIcon = new NotifyIcon(_components)
            {
                Text = "Mica for Everyone",
                Icon = User32.LoadIcon(HINSTANCE.NULL, User32.IDI_APPLICATION).ToIcon(),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip
                {
                    Items =
                    {
                        themeModeSubMenu,
                        micaModeSubMenu,
                        new ToolStripSeparator(),
                        reapplyMenuItem,
                        exitMenuItem,
                    }
                },
            };

            var hook = new WinEventHook(_components);
            hook.HookTriggered += OnHookTriggered;
            hook.Hook(User32.EventConstants.EVENT_OBJECT_CREATE, User32.EventConstants.EVENT_OBJECT_CREATE);

            ThreadExit += OnThreadExit;
        }

        private void OnThreadExit(object sender, EventArgs e)
        {
            _components.Dispose();
        }

        private void OnHookTriggered(object sender, HookTriggeredEventArgs e)
        {
            _ruleHandler.ApplyToWindow(e.WindowHandle);
        }
    }
}
