using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MicaForEveryone.UI.ViewModels
{
    public interface IContentDialogViewModel : INotifyPropertyChanged
    {
        object Title { get; set; }

        object Content { get; set; }

        bool IsPrimaryButtonEnabled { get; set; }

        object PrimaryButtonContent { get; set; }

        bool IsSecondaryButtonEnabled { get; set; }

        object SecondaryButtonContent { get; set; }

        IRelayCommand PrimaryCommand { get; set; }

        object PrimaryCommandParameter { get; set; }

        IRelayCommand SecondaryCommand { get; set; }

        object SecondaryCommandParameter { get; set; }
    }
}
