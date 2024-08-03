using MicaForEveryone.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace MicaForEveryone.App.Views;

public sealed partial class AddProcessRuleContentDialog : ContentDialog
{
    private AddProcessRuleContentDialogViewModel ViewModel { get; }

    public AddProcessRuleContentDialog()
    {
        this.InitializeComponent();

        ViewModel = App.Services.GetRequiredService<AddProcessRuleContentDialogViewModel>();
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.RequestSuggestions();
        }
    }
}
