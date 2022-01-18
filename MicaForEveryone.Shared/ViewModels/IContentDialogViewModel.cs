using System.ComponentModel;
using System.Windows.Input;

namespace MicaForEveryone.ViewModels
{
    public interface IContentDialogViewModel : INotifyPropertyChanged
    {
        object Title { get; set; }

        object Content { get; set; }

        bool IsPrimaryButtonEnabled { get; set; }

        object PrimaryButtonContent { get; set; }

        bool IsSecondaryButtonEnabled { get; set; }

        object SecondaryButtonContent { get; set; }

        ICommand PrimaryCommand { get; set; }

        object PrimaryCommandParameter { get; set; }

        ICommand SecondaryCommand { get; set; }

        object SecondaryCommandParameter { get; set; }
    }
}
