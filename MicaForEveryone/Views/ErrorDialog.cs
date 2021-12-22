using MicaForEveryone.UWP;
using MicaForEveryone.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicaForEveryone.Views
{
    public class ErrorDialog : XamlDialog
    {
        private static ContentDialogView CreateView() => new ContentDialogView
        {
            ViewModel =
            {
                Title = "Error",
                IsPrimaryButtonEnabled = true,
                PrimaryButtonContent = "OK",
                PrimaryCommand = App.CloseDialogCommand,
            },
        };

        public ErrorDialog() : base(CreateView())
        {
            ClassName = "Dialog";
            Title = "Mica For Everyone";
            Width = 576;
            Height = 320;
            ((ContentDialogView)View).ViewModel.PrimaryCommandParameter = this;
        }

        public void SetMessage(string message)
        {
            ((ContentDialogView)View).ViewModel.Content = message;
        }
    }
}
