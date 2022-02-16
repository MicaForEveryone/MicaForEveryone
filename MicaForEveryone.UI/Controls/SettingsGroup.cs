// Based on code from: https://github.com/microsoft/PowerToys/blob/03b7cb46902a6d1dfcf013bb4ebde0b42273d0f8/src/settings-ui/Settings.UI/Controls/SettingsGroup/SettingsGroup.cs

// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace MicaForEveryone.UI.Controls
{
    /// <summary>
    /// Represents a control that can contain multiple settings (or other) controls
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplatePart(Name = PartDescriptionPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PartHeaderPresenter, Type = typeof(TextBlock))]
    public partial class SettingsGroup : ItemsControl
    {
        private const string PartDescriptionPresenter = "DescriptionPresenter";
        private const string PartHeaderPresenter = "HeaderPresenter";

        private ContentPresenter _descriptionPresenter;
        private TextBlock _headerPresenter;

        private SettingsGroup _settingsGroup;

        public SettingsGroup()
        {
            DefaultStyleKey = typeof(SettingsGroup);
        }

        [Localizable(true)]
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(SettingsGroup),
            new PropertyMetadata(default(string)));

        [Localizable(true)]
        public object Description
        {
            get => (object)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(object),
            typeof(SettingsGroup),
            new PropertyMetadata(null, OnDescriptionChanged));

        protected override void OnApplyTemplate()
        {
            IsEnabledChanged -= SettingsGroup_IsEnabledChanged;
            _settingsGroup = (SettingsGroup)this;
            _headerPresenter = (TextBlock)_settingsGroup.GetTemplateChild(PartHeaderPresenter);
            _descriptionPresenter = (ContentPresenter)_settingsGroup.GetTemplateChild(PartDescriptionPresenter);
            SetEnabledState();
            IsEnabledChanged += SettingsGroup_IsEnabledChanged;
            Update();
            base.OnApplyTemplate();
        }

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingsGroup)d).Update();
        }

        private void SettingsGroup_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetEnabledState();
        }

        private void SetEnabledState()
        {
            VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
        }

        private void Update()
        {
            if (_settingsGroup == null)
            {
                return;
            }

            if (_settingsGroup.Header == null)
            {
                _settingsGroup._headerPresenter.Visibility = Visibility.Collapsed;
            }
            else
            {
                _settingsGroup._headerPresenter.Visibility = Visibility.Visible;
            }

            if (_settingsGroup.Description == null)
            {
                _settingsGroup._descriptionPresenter.Visibility = Visibility.Collapsed;
            }
            else
            {
                _settingsGroup._descriptionPresenter.Visibility = Visibility.Visible;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SettingsGroupAutomationPeer(this);
        }
    }

    public class SettingsGroupAutomationPeer : FrameworkElementAutomationPeer
    {
        public SettingsGroupAutomationPeer(SettingsGroup owner)
            : base(owner)
        {
        }

        protected override string GetNameCore()
        {
            var selectedSettingsGroup = (SettingsGroup)Owner;
            return selectedSettingsGroup.Header;
        }
    }
}
