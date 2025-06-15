using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using WinRT;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DialogWindow : Window
    {
        private TaskCompletionSource<ContentDialogResult> _tcs = new();

        public DialogWindow(string title, string message)
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;

            TitleControl.Content = title;
            ContentControl.Content = message;

            var dpi = TerraFX.Interop.Windows.Windows.GetDpiForWindow((TerraFX.Interop.Windows.HWND)WindowNative.GetWindowHandle(this));
            var scale = dpi / 96.0f;

            OverlappedPresenter presenter = AppWindow.Presenter.As<OverlappedPresenter>();
            presenter.IsResizable = presenter.IsMinimizable = presenter.IsMaximizable = false;
            AppWindow.ResizeClient(new Windows.Graphics.SizeInt32((int)(480 * scale), (int)(196 * scale)));

            var area = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)?.WorkArea;
            if (area == null) return;
            AppWindow.Move(new Windows.Graphics.PointInt32((area.Value.Width - AppWindow.Size.Width) / 2, (area.Value.Height - AppWindow.Size.Height) / 2));
        }

        public Task<ContentDialogResult> ShowAsync()
        {
            Activate();
            return _tcs.Task;
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            _tcs.SetResult(ContentDialogResult.Primary);
        }

        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            _tcs.SetResult(ContentDialogResult.Secondary);
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            _tcs.TrySetResult(ContentDialogResult.None);
        }
    }
}
