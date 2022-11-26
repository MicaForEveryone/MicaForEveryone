using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.ViewModels
{
    internal class ContentDialogViewModel : ObservableObject, IContentDialogViewModel
    {
        private object _title;
        private object _content;
        private object _primaryButtonContent;
        private object _secondaryButtonContent;
        private bool _isPrimaryButtonEnabled;
        private bool _isSecondaryButtonEnabled;

        public object Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public object Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public bool IsPrimaryButtonEnabled
        {
            get => _isPrimaryButtonEnabled;
            set => SetProperty(ref _isPrimaryButtonEnabled, value);
        }

        public object PrimaryButtonContent
        {
            get => _primaryButtonContent;
            set => SetProperty(ref _primaryButtonContent, value);
        }

        public bool IsSecondaryButtonEnabled
        {
            get => _isSecondaryButtonEnabled;
            set => SetProperty(ref _isSecondaryButtonEnabled, value);
        }

        public object SecondaryButtonContent
        {
            get => _secondaryButtonContent;
            set => SetProperty(ref _secondaryButtonContent, value);
        }

        public IRelayCommand PrimaryCommand { get; set; }

        public object PrimaryCommandParameter { get; set; }

        public IRelayCommand SecondaryCommand { get; set; }

        public object SecondaryCommandParameter { get; set; }
    }
}
