using MicaForEveryone.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace MicaForEveryone.App.Views;

public sealed partial class AddClassRuleContentDialog : ContentDialog
{
    private AddClassRuleContentDialogViewModel ViewModel { get; }

    public AddClassRuleContentDialog()
    {
        this.InitializeComponent();

        ViewModel = App.Services.GetRequiredService<AddClassRuleContentDialogViewModel>();
    }
}
