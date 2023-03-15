using System;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas.Effects;
using Windows.System.Power;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using MicaForEveryone.Core.Ui.Interfaces;

// based on gist: https://gist.github.com/lhak/641becbc6a4a5f809c224fc83600bb36

namespace MicaForEveryone.UI.Brushes
{
    public sealed class XamlMicaBrush : XamlCompositionBrushBase
    {
        private static CompositionBrush BuildMicaEffectBrush(Compositor compositor, Color tintColor, float tintOpacity, float luminosityOpacity)
        {        
            // Tint Color.
            var tintColorEffect = new ColorSourceEffect
            {
                Name = "TintColor",
                Color = tintColor
            };

            // OpacityEffect applied to Tint.
            var tintOpacityEffect = new OpacityEffect
            {
                Name = "TintOpacity",
                Opacity = tintOpacity,
                Source = tintColorEffect
            };

            // Apply Luminosity:

            // Luminosity Color.
            var luminosityColorEffect = new ColorSourceEffect
            {
                Color = tintColor
            };

            // OpacityEffect applied to Luminosity.
            var luminosityOpacityEffect = new OpacityEffect
            {
                Name = "LuminosityOpacity",
                Opacity = luminosityOpacity,
                Source = luminosityColorEffect
            };

            // Luminosity Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            var luminosityBlendEffect = new BlendEffect
            {
                Mode = BlendEffectMode.Color,
                Background = new CompositionEffectSourceParameter("BlurredWallpaperBackdrop"),
                Foreground = luminosityOpacityEffect
            };

            // Apply Tint:

            // Color Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            var colorBlendEffect = new BlendEffect
            {
                Mode = BlendEffectMode.Luminosity,
                Background = luminosityBlendEffect,
                Foreground = tintOpacityEffect
            };

            CompositionEffectBrush micaEffectBrush = compositor.CreateEffectFactory(colorBlendEffect).CreateBrush();
            micaEffectBrush.SetSourceParameter("BlurredWallpaperBackdrop", compositor.TryCreateBlurredWallpaperBackdropBrush());

            return micaEffectBrush;
        }

        private static CompositionBrush CreateCrossFadeEffectBrush(Compositor compositor, CompositionBrush from, CompositionBrush to)
        {
            var crossFadeEffect = new CrossFadeEffect
            {
                Name = "Crossfade", // Name to reference when starting the animation.
                Source1 = new CompositionEffectSourceParameter("source1"),
                Source2 = new CompositionEffectSourceParameter("source2"),
                CrossFade = 0
            };

            CompositionEffectBrush crossFadeEffectBrush = compositor.CreateEffectFactory(crossFadeEffect, new List<string>() { "Crossfade.CrossFade" }).CreateBrush();
            crossFadeEffectBrush.Comment = "Crossfade";
            // The inputs have to be swapped here to work correctly...
            crossFadeEffectBrush.SetSourceParameter("source1", to);
            crossFadeEffectBrush.SetSourceParameter("source2", from);
            return crossFadeEffectBrush;
        }

        private static ScalarKeyFrameAnimation CreateCrossFadeAnimation(Compositor compositor)
        {
            ScalarKeyFrameAnimation animation = compositor.CreateScalarKeyFrameAnimation();
            LinearEasingFunction linearEasing = compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(0.0f, 0.0f, linearEasing);
            animation.InsertKeyFrame(1.0f, 1.0f, linearEasing);
            animation.Duration = TimeSpan.FromMilliseconds(250);
            return animation;
        }

        public XamlMicaBrush(FrameworkElement root, IFocusableWindow window)
        {
            RootElement = root;
            FocusableWindow = window;

            FocusableWindow.GotFocus += Window_GotFocus;
            FocusableWindow.LostFocus += Window_LostFocus;
        }

        ~XamlMicaBrush()
        {
            FocusableWindow.GotFocus -= Window_GotFocus;
            FocusableWindow.LostFocus -= Window_LostFocus;
        }

        public FrameworkElement RootElement { get; }

        public IFocusableWindow FocusableWindow { get; }

        protected override void OnConnected()
        {
            base.OnConnected();

            if (_settings == null)
                _settings = new UISettings();

            if (_accessibilitySettings == null)
                _accessibilitySettings = new AccessibilitySettings();

            if (_fastEffects == null)
                _fastEffects = CompositionCapabilities.GetForCurrentView().AreEffectsFast();

            if (_energySaver == null)
                _energySaver = PowerManager.EnergySaverStatus == EnergySaverStatus.On;

            UpdateBrush();

            _settings.ColorValuesChanged += Settings_ColorValuesChanged;
            _accessibilitySettings.HighContrastChanged += AccessibilitySettings_HighContrastChanged;
            PowerManager.EnergySaverStatusChanged += PowerManager_EnergySaverStatusChanged;
            CompositionCapabilities.GetForCurrentView().Changed += CompositionCapabilities_Changed;
            RootElement.ActualThemeChanged += RootElement_ActualThemeChanged;
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            if (_settings != null)
            {
                _settings.ColorValuesChanged -= Settings_ColorValuesChanged;
                _settings = null;
            }

            if (_accessibilitySettings != null)
            {
                _accessibilitySettings.HighContrastChanged -= AccessibilitySettings_HighContrastChanged;
                _accessibilitySettings = null;
            }

            PowerManager.EnergySaverStatusChanged -= PowerManager_EnergySaverStatusChanged;
            CompositionCapabilities.GetForCurrentView().Changed -= CompositionCapabilities_Changed;            
            RootElement.ActualThemeChanged -= RootElement_ActualThemeChanged;

            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }

        private void UpdateBrush()
        {
            if (_settings == null || _accessibilitySettings == null)
                return;

            var currentTheme = RootElement.ActualTheme;
            var compositor = Window.Current.Compositor;

            var useSolidColorFallback = 
                !Windows.Foundation.Metadata.ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush)) ||
                !_settings.AdvancedEffectsEnabled ||
                !_windowActivated ||
                _fastEffects == false ||
                _energySaver == true;

            Color tintColor = currentTheme == ElementTheme.Light ?
                Color.FromArgb(255, 243, 243, 243) :
                Color.FromArgb(255, 32, 32, 32);
            float tintOpacity = currentTheme == ElementTheme.Light ? 0.5f : 0.8f;

            if (_accessibilitySettings.HighContrast)
            {
                tintColor = _settings.GetColorValue(UIColorType.Background);
                useSolidColorFallback = true;
            }

            var newBrush = useSolidColorFallback ?
                compositor.CreateColorBrush(tintColor) :
                BuildMicaEffectBrush(compositor, tintColor, tintOpacity, 1.0f);

            var oldBrush = CompositionBrush;

            var doCrossFade = oldBrush != null && 
                CompositionBrush.Comment != "CrossFade" &&
                !(oldBrush is CompositionColorBrush && newBrush is CompositionColorBrush);

            if (doCrossFade)
            {
                var crossFadeBrush = CreateCrossFadeEffectBrush(compositor, oldBrush, newBrush);
                var animation = CreateCrossFadeAnimation(compositor);
                CompositionBrush = crossFadeBrush;

                var crossFadeAnimationBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                crossFadeBrush.StartAnimation("CrossFade.CrossFade", animation);
                crossFadeAnimationBatch.End();

                crossFadeAnimationBatch.Completed += (o, a) =>
                {
                    crossFadeBrush.Dispose();
                    oldBrush.Dispose();
                    CompositionBrush = newBrush;
                };
            }
            else
            {
                if (oldBrush != null)
                {
                    oldBrush.Dispose();
                }
                CompositionBrush = newBrush;
            }
        }

        private async void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateBrush();
            });
        }

        private async void AccessibilitySettings_HighContrastChanged(AccessibilitySettings sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateBrush();
            });
        }

        private async void CompositionCapabilities_Changed(CompositionCapabilities sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _fastEffects = sender.AreEffectsFast();
                UpdateBrush();
            });
        }

        private async void PowerManager_EnergySaverStatusChanged(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _energySaver = PowerManager.EnergySaverStatus == EnergySaverStatus.On;
                UpdateBrush();
            });
        }

        private void Window_GotFocus(object sender, EventArgs e)
        {
            _windowActivated = true;
            UpdateBrush();
        }

        private void Window_LostFocus(object sender, EventArgs e)
        {
            _windowActivated = false;
            UpdateBrush();
        }

        private void RootElement_ActualThemeChanged(FrameworkElement sender, object args)
        {
            UpdateBrush();
        }

        private UISettings _settings;
        private AccessibilitySettings _accessibilitySettings;
        private bool? _fastEffects;
        private bool? _energySaver;
        private bool _windowActivated;
    }
}
