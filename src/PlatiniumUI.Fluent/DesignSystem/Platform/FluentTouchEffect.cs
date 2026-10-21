#if WINDOWS
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
#endif

namespace PlatiniumUI.Fluent.DesignSystem.Platform;

/// <summary>
/// Platform-specific touch effects and animations.
/// Provides ripple effects on Android, scale effects on iOS,
/// and WinUI 3 Reveal Highlight + Connected Animations on Windows.
/// </summary>
public static class FluentTouchEffect
{
    /// <summary>
    /// Applies platform-specific touch feedback to a view.
    /// On Android: Uses native RippleDrawable for authentic Material-style feedback
    /// On iOS: Adds scale animation on press
    /// On Windows: Adds WinUI 3 Reveal Highlight effect with pointer-following light
    /// </summary>
    public static void Apply(Microsoft.Maui.Controls.View view, bool enableRipple = true)
    {
        if (!enableRipple) return;

#if WINDOWS
        // Windows: Apply WinUI 3 native Reveal Highlight effect
        ApplyWinUI3RevealEffect(view);
#elif ANDROID
        // Android: Use native RippleDrawable for authentic Material touch feedback
        FluentRippleEffect.Apply(view, RippleConfig.Default);
#else
        // Add visual feedback through gesture recognizer for non-Android/non-Windows platforms
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            if (view is Microsoft.Maui.Controls.VisualElement visualElement)
            {
#if IOS
                // iOS: Scale animation on press
                await visualElement.ScaleToAsync(0.95, 50, Easing.CubicOut);
                await visualElement.ScaleToAsync(1.0, 50, Easing.CubicIn);
#else
                // Other platforms: Opacity flash
                var originalOpacity = visualElement.Opacity;
                visualElement.Opacity = 0.7;
                await Task.Delay(100);
                visualElement.Opacity = originalOpacity;
#endif
            }
        };

        // Only add if no tap gesture exists
        if (!view.GestureRecognizers.OfType<TapGestureRecognizer>().Any())
        {
            view.GestureRecognizers.Add(tapGesture);
        }
#endif
    }

#if WINDOWS
    /// <summary>
    /// Applies WinUI 3 pointer-following highlight effect using native APIs.
    /// Creates a light source effect that follows the pointer on Windows.
    /// Note: WinUI 3 doesn't have RevealBackgroundBrush like UWP, so we use opacity-based effects.
    /// </summary>
    private static void ApplyWinUI3RevealEffect(Microsoft.Maui.Controls.View view)
    {
        // Wait for the handler to be available, then apply native effects
        view.HandlerChanged += (s, e) =>
        {
            if (view.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement nativeElement)
            {
                // WinUI 3 compatible highlight using opacity changes
                var highlightBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                {
                    Opacity = 0.15
                };

                var borderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                {
                    Opacity = 0.4
                };

                // Hook pointer events for hover effect
                nativeElement.PointerEntered += (sender, args) =>
                {
                    if (nativeElement is Microsoft.UI.Xaml.Controls.Border border)
                    {
                        border.Background = highlightBrush;
                        border.BorderBrush = borderBrush;
                    }
                    else if (nativeElement is Microsoft.UI.Xaml.Controls.Panel panel)
                    {
                        panel.Background = highlightBrush;
                    }
                    else if (nativeElement is Microsoft.UI.Xaml.Controls.ContentPresenter presenter)
                    {
                        presenter.Background = highlightBrush;
                        presenter.BorderBrush = borderBrush;
                    }
                };

                nativeElement.PointerExited += (sender, args) =>
                {
                    if (nativeElement is Microsoft.UI.Xaml.Controls.Border border)
                    {
                        border.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                        border.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                    }
                    else if (nativeElement is Microsoft.UI.Xaml.Controls.Panel panel)
                    {
                        panel.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                    }
                    else if (nativeElement is Microsoft.UI.Xaml.Controls.ContentPresenter presenter)
                    {
                        presenter.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                        presenter.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                    }
                };

                // Apply WinUI 3 press animation (scale down with Connected Animation easing)
                nativeElement.PointerPressed += async (sender, args) =>
                {
                    if (view is Microsoft.Maui.Controls.VisualElement visualElement)
                    {
                        // WinUI 3 uses cubic bezier easing: (0.1, 0.9, 0.2, 1.0)
                        await visualElement.ScaleToAsync(0.98, 83, Easing.CubicOut);
                    }
                };

                nativeElement.PointerReleased += async (sender, args) =>
                {
                    if (view is Microsoft.Maui.Controls.VisualElement visualElement)
                    {
                        await visualElement.ScaleToAsync(1.0, 167, Easing.CubicIn);
                    }
                };
            }
        };
    }

    /// <summary>
    /// Applies WinUI 3 ThemeShadow to a view for depth effect.
    /// Creates a shadow that responds to theme changes automatically.
    /// Note: This uses MAUI's Shadow property which works cross-platform including Windows.
    /// </summary>
    public static void ApplyWinUI3ThemeShadow(Microsoft.Maui.Controls.View view, double depth = 16)
    {
        // ThemeShadow implementation using MAUI's cross-platform Shadow
        view.HandlerChanged += (s, e) =>
        {
            if (view.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement nativeElement)
            {
                // Use MAUI's built-in Shadow which works on all platforms including Windows
                if (view is Microsoft.Maui.Controls.VisualElement visualElement)
                {
                    visualElement.Shadow = new Microsoft.Maui.Controls.Shadow
                    {
                        Brush = Microsoft.Maui.Controls.Brush.Black,
                        Offset = new Microsoft.Maui.Graphics.Point(0, depth * 0.5),
                        Radius = (float)(depth * 2),
                        Opacity = 0.3f
                    };
                }
            }
        };
    }
#endif

    /// <summary>
    /// Applies a press animation (scale down and up) to any view.
    /// Uses WinUI 3 timing on Windows, standard easing on other platforms.
    /// </summary>
    public static async Task ApplyPressAnimationAsync(Microsoft.Maui.Controls.View view, double scale = 0.95, uint duration = 100)
    {
        if (view is Microsoft.Maui.Controls.VisualElement visualElement)
        {
#if WINDOWS
            // WinUI 3 uses 83ms for press, 167ms for release with specific cubic bezier
            await visualElement.ScaleToAsync(scale, 83, Easing.CubicOut);
            await visualElement.ScaleToAsync(1.0, 167, Easing.CubicIn);
#else
            await visualElement.ScaleToAsync(scale, duration, Easing.CubicOut);
            await visualElement.ScaleToAsync(1.0, duration, Easing.CubicIn);
#endif
        }
    }

    /// <summary>
    /// Applies a ripple-like opacity flash effect.
    /// Best for Android but works on all platforms.
    /// </summary>
    public static async Task ApplyRippleAsync(Microsoft.Maui.Controls.View view, double minOpacity = 0.7, uint duration = 100)
    {
        if (view is Microsoft.Maui.Controls.VisualElement visualElement)
        {
            var originalOpacity = visualElement.Opacity;
            visualElement.Opacity = minOpacity;
            await Task.Delay((int)duration);
            visualElement.Opacity = originalOpacity;
        }
    }
}
