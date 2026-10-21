namespace PlatiniumUI.Fluent.DesignSystem.Platform;

/// <summary>
/// Configuration for ripple effect appearance and behavior.
/// </summary>
public class RippleConfig
{
    /// <summary>
    /// Custom ripple color. If null, uses theme's PressedOverlay color.
    /// </summary>
    public Color? Color { get; set; }

    /// <summary>
    /// Corner radius for the ripple in device-independent pixels.
    /// If 0, uses the view's natural corner radius.
    /// </summary>
    public double Radius { get; set; } = 0;

    /// <summary>
    /// Whether the ripple is bounded (contained within the view) or unbounded.
    /// Default is true (bounded).
    /// </summary>
    public bool Bounded { get; set; } = true;

    /// <summary>
    /// Whether ripple effect is enabled. Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Creates a default ripple configuration.
    /// </summary>
    public static RippleConfig Default => new RippleConfig();

    /// <summary>
    /// Creates a ripple configuration with no ripple effect.
    /// </summary>
    public static RippleConfig None => new RippleConfig { Enabled = false };
}

/// <summary>
/// Cross-platform facade for applying ripple effects to views.
/// On Android: Uses native RippleDrawable for authentic Material-style feedback.
/// On iOS: Uses scale animation as a press effect.
/// On Windows: Uses opacity-based highlight effect.
/// On other platforms: Falls back to opacity flash.
/// </summary>
public static class FluentRippleEffect
{
    /// <summary>
    /// Applies a ripple effect to the specified view using default configuration.
    /// </summary>
    /// <param name="view">The view to apply ripple to</param>
    public static void Apply(Microsoft.Maui.Controls.View view)
    {
        Apply(view, RippleConfig.Default);
    }

    /// <summary>
    /// Applies a ripple effect to the specified view with custom configuration.
    /// </summary>
    /// <param name="view">The view to apply ripple to</param>
    /// <param name="config">Ripple configuration</param>
    public static void Apply(Microsoft.Maui.Controls.View view, RippleConfig? config)
    {
        if (view == null || config == null || !config.Enabled)
            return;

        // Hook into HandlerChanged to apply when platform view is available
        view.HandlerChanged += OnHandlerChanged;

        void OnHandlerChanged(object? sender, EventArgs e)
        {
            if (view.Handler?.PlatformView == null)
                return;

            // Unsubscribe to avoid multiple applications
            view.HandlerChanged -= OnHandlerChanged;

#if ANDROID
            ApplyAndroidRipple(view, config);
#elif IOS
            ApplyIOSRipple(view, config);
#elif WINDOWS
            ApplyWindowsRipple(view, config);
#else
            ApplyFallbackRipple(view, config);
#endif
        }
    }

    /// <summary>
    /// Removes the ripple effect from the specified view.
    /// </summary>
    /// <param name="view">The view to remove ripple from</param>
    public static void Remove(Microsoft.Maui.Controls.View view)
    {
        if (view == null)
            return;

#if ANDROID
        Platforms.Android.FluentRippleEffect.Remove(view);
#endif
        // For other platforms, ripple is applied via gestures/animations
        // which are automatically cleaned up when the view is disposed
    }

#if ANDROID
    private static void ApplyAndroidRipple(Microsoft.Maui.Controls.View view, RippleConfig config)
    {
        Android.Graphics.Color? color = null;
        if (config.Color != null)
        {
            color = new Android.Graphics.Color(
                (int)(config.Color.Red * 255),
                (int)(config.Color.Green * 255),
                (int)(config.Color.Blue * 255),
                (int)(config.Color.Alpha * 255)
            );
        }

        Platforms.Android.FluentRippleEffect.Apply(view, color, (float)config.Radius);
    }
#endif

#if IOS
    private static void ApplyIOSRipple(Microsoft.Maui.Controls.View view, RippleConfig config)
    {
        // iOS doesn't have native ripple, so we use a scale animation on tap
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            if (view is Microsoft.Maui.Controls.VisualElement visualElement)
            {
                await visualElement.ScaleToAsync(0.95, 50, Easing.CubicOut);
                await visualElement.ScaleToAsync(1.0, 50, Easing.CubicIn);
            }
        };

        if (!view.GestureRecognizers.OfType<TapGestureRecognizer>().Any())
        {
            view.GestureRecognizers.Add(tapGesture);
        }
    }
#endif

#if WINDOWS
    private static void ApplyWindowsRipple(Microsoft.Maui.Controls.View view, RippleConfig config)
    {
        // Windows uses opacity-based highlight via FluentTouchEffect
        // This is handled by the existing FluentTouchEffect.Apply() method
        FluentTouchEffect.Apply(view, enableRipple: true);
    }
#endif

#if !ANDROID && !IOS && !WINDOWS
    private static void ApplyFallbackRipple(Microsoft.Maui.Controls.View view, RippleConfig config)
    {
        // Fallback: opacity flash
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            if (view is Microsoft.Maui.Controls.VisualElement visualElement)
            {
                var originalOpacity = visualElement.Opacity;
                visualElement.Opacity = 0.7;
                await Task.Delay(100);
                visualElement.Opacity = originalOpacity;
            }
        };

        if (!view.GestureRecognizers.OfType<TapGestureRecognizer>().Any())
        {
            view.GestureRecognizers.Add(tapGesture);
        }
    }
#endif
}
