using MicaForEveryone.Models;
using MicaForEveryone.Views;

namespace MicaForEveryone.Interfaces
{
    public interface IViewService
    {
        public TitlebarColorMode SystemColorMode { get; }

        public MainWindow MainWindow { get; }
    }
}
