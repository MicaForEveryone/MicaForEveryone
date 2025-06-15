using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Controls
{
    public sealed partial class CustomColorPicker : Control
    {
        public static DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor",
                typeof(Color),
                typeof(CustomColorPicker),
                null);

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public event RoutedEventHandler? OkButtonClicked;
        public event RoutedEventHandler? CancelButtonClicked;

        private ColorPicker? picker;

        public CustomColorPicker()
        {
            DefaultStyleKey = typeof(CustomColorPicker);
        }

        protected override void OnApplyTemplate()
        {
            picker = (ColorPicker)GetTemplateChild("Picker");
            Button okButton = (Button)GetTemplateChild("OkButton");
            okButton.Click += OkButton_Click;
            Button cancelButton = (Button)GetTemplateChild("CancelButton");
            cancelButton.Click += CancelButton_Click;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            picker!.Color = SelectedColor;
            CancelButtonClicked?.Invoke(this, e);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = picker!.Color;
            OkButtonClicked?.Invoke(this, e);
        }
    }

    public sealed partial class CustomColorPickerTemplate : ResourceDictionary
    {
        public CustomColorPickerTemplate()
        {
            InitializeComponent();
        }
    }
}
