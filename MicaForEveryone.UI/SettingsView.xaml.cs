using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MicaForEveryone.UI.Models;
using Microsoft.UI.Xaml.Controls;
using MicaForEveryone.Core.Ui.ViewModels;

namespace MicaForEveryone.UI
{
    public sealed partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public ISettingsViewModel ViewModel { get; set; }

        internal Contributor[] Contributors { get; } =
        {
            new Contributor("Dongle the Gadget", "https://github.com/dongle-the-gadget", null),
            new Contributor("sitiom", "https://github.com/sitiom", null),
            new Contributor("krlvm", "https://github.com/krlvm", null),
        };

        internal Contributor[] Translators { get; } =
        {
            new Contributor("Amaro Martínez", "https://github.com/xoascf", "es"),
            new Contributor("Danik2343", "https://github.com/Danik2343", "ru"),
            new Contributor("maboroshin", "https://github.com/maboroshin", "ja"),
            new Contributor("uDEV2019", "https://github.com/uDEV2019", "de"),
            new Contributor("alessandrocaseti", "https://github.com/alessandrocaseti", "it"),
            new Contributor("AndromedaMelody", "https://github.com/AndromedaMelody", "zh-Hans"),
            new Contributor("SaiyajinK", "https://github.com/SaiyajinK", "fr"),
            new Contributor("RTFTR", "https://github.com/RTFTR", "ko"),
            new Contributor("krlvm", "https://github.com/krlvm", "ru"),
            new Contributor("FrzMtrsprt", "https://github.com/FrzMtrsprt", "zh-Hans"),
            new Contributor("Piteriuz", "https://github.com/Piteriuz", "pl"),
            new Contributor("Douglas Vianna", "https://github.com/dgsmiley18", "pt-br"),
            new Contributor("toineenzo", "https://github.com/toineenzo", "nl"),
            new Contributor("A-Loot", "https://github.com/A-Loot", "it"),
            new Contributor("MW1Z", "https://github.com/MW1Z", "zh-Hant"),
        };

        private void ListView_Loaded(object sender, RoutedEventArgs args)
        {
            ((ListView)sender).Focus(FocusState.Programmatic);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs args)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void RulesAppBarButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState((UIElement)sender, "PointerOver");
        }

        private void RulesAppBarButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState((UIElement)sender, "Pressed");
        }

        private void RulesAppBarButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState((UIElement)sender, "Normal");
        }

        private void RulesAppBarButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState((UIElement)sender, "Normal");
        }
    }
}
