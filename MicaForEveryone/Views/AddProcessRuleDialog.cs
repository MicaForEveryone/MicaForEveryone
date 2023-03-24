using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.Views
{
    internal class AddProcessRuleDialog : ContentDialog
    {
        public AddProcessRuleDialog() : 
            this(new(), Program.Container.GetService<IAddProcessRuleViewModel>())
        {
        }

        private AddProcessRuleDialog(ContentDialogView view, IAddProcessRuleViewModel viewModel) :
            base(view, viewModel)
        {
            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("AddRuleDialog/Title");
            Width = 400;
            Height = 300;

            ViewModel = viewModel;
            ViewModel.Title = resources.GetString("AddProcessRuleContentDialog/Title");

            var autoSuggestBox = (AutoSuggestBox)XamlReader.Load(@"
<AutoSuggestBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                x:Uid='ProcessNameSuggestBox'
                HorizontalAlignment='Stretch'
                ItemsSource='{Binding Suggestions}' 
                Text='{Binding ProcessName, Mode=TwoWay}' />
"); 
            autoSuggestBox.DataContext = ViewModel;
            autoSuggestBox.QuerySubmitted += (sender, args) =>
            {
                ViewModel.PrimaryCommand.Execute(this);
            };
            autoSuggestBox.SuggestionChosen += (sender, args) =>
            {
                ViewModel.ProcessName = args.SelectedItem.ToString();
            };
            autoSuggestBox.Loaded += (sender, args) =>
            {
                autoSuggestBox.Focus(FocusState.Programmatic);
            };
            ViewModel.Content = autoSuggestBox;

            ViewModel.IsPrimaryButtonEnabled = true;
            ViewModel.PrimaryButtonContent = resources.GetString("AddButton/Content");
            ViewModel.PrimaryCommandParameter = this;
            
            ViewModel.IsSecondaryButtonEnabled = true;
            ViewModel.SecondaryButtonContent = resources.GetString("CancelButton/Content");
            ViewModel.SecondaryCommand = CloseDialogCommand;
            ViewModel.SecondaryCommandParameter = this;
        }

        public new IAddProcessRuleViewModel ViewModel { get; }
    }
}
