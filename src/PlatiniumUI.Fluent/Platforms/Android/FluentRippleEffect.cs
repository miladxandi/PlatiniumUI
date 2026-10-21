#if ANDROID
using global::Android.Graphics.Drawables;
using Android.Content.Res;
using global::Android.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Platform;

namespace PlatiniumUI.Fluent.Platforms.Android;

/// <summary>
/// Android-specific helper for applying native Material-style ripple effects to views.
/// Uses Android's RippleDrawable for authentic touch feedback that follows pointer position.
/// </summary>
public static class FluentRippleEffect
{
    /// <summary>
    /// Applies a native Android ripple effect to the specified MAUI view.
    /// </summary>
    /// <param name="view">The MAUI view to apply ripple to</param>
    /// <param name="rippleColor">The color of the ripple (defaults to theme's PressedOverlay)</param>
    /// <param name="cornerRadius">Corner radius for rounded ripple (0 for square)</param>
    public static void Apply(Microsoft.Maui.Controls.View view, global::Android.Graphics.Color? rippleColor = null, float cornerRadius = 0f)
    {
        if (view?.Handler?.PlatformView is not global::Android.Views.View nativeView)
            return;

        var color = rippleColor ?? GetThemeRippleColor();
        
        // Create ColorStateList for ripple
        var colorStateList = ColorStateList.ValueOf(color);
        
        // Create RippleDrawable
        var rippleDrawable = new RippleDrawable(colorStateList, null, null);
        
        // Apply as foreground (works on API 23+)
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            if (nativeView is ViewGroup viewGroup)
            {
                viewGroup.Foreground = rippleDrawable;
                viewGroup.Clickable = true;
                
                // Set up rounded corners if specified
                if (cornerRadius > 0)
                {
                    nativeView.OutlineProvider = new RoundedRectOutlineProvider(cornerRadius);
                    nativeView.ClipToOutline = true;
                }
            }
            else
            {
                // For non-ViewGroup, try setting as background
                nativeView.Background = rippleDrawable;
                
                if (cornerRadius > 0)
                {
                    nativeView.OutlineProvider = new RoundedRectOutlineProvider(cornerRadius);
                    nativeView.ClipToOutline = true;
                }
            }
        }
        else
        {
            // API 21-22 fallback: use background drawable
            nativeView.Background = rippleDrawable;
            if (cornerRadius > 0)
            {
                nativeView.OutlineProvider = new RoundedRectOutlineProvider(cornerRadius);
                nativeView.ClipToOutline = true;
            }
        }
    }

    /// <summary>
    /// Removes the ripple effect from the specified view.
    /// </summary>
    public static void Remove(Microsoft.Maui.Controls.View view)
    {
        if (view?.Handler?.PlatformView is not global::Android.Views.View nativeView)
            return;

        if (nativeView is ViewGroup viewGroup && OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            viewGroup.Foreground = null;
        }
        else
        {
            nativeView.Background = null;
        }
        
        nativeView.OutlineProvider = global::Android.Views.ViewOutlineProvider.Background;
        nativeView.ClipToOutline = false;
    }

    private static global::Android.Graphics.Color GetThemeRippleColor()
    {
        // Use theme's PressedOverlay color, or default to semi-transparent white
        try
        {
            var theme = DesignSystem.Theme.FluentThemeManager.Current.CurrentTheme;
            var pressedOverlay = theme.PressedOverlay;
            
            if (pressedOverlay != null)
            {
                return new global::Android.Graphics.Color(
                    (int)(pressedOverlay.Red * 255),
                    (int)(pressedOverlay.Green * 255),
                    (int)(pressedOverlay.Blue * 255),
                    (int)(pressedOverlay.Alpha * 255)
                );
            }
        }
        catch
        {
            // Fallback to default
        }
        
        // Default: 12% opacity white (Material Design standard)
        return new global::Android.Graphics.Color(255, 255, 255, 31);
    }
}

/// <summary>
/// Custom OutlineProvider for rounded rectangle ripples.
/// </summary>
internal class RoundedRectOutlineProvider : global::Android.Views.ViewOutlineProvider
{
    private readonly float _cornerRadius;

    public RoundedRectOutlineProvider(float cornerRadius)
    {
        _cornerRadius = cornerRadius;
    }

    public override void GetOutline(global::Android.Views.View view, global::Android.Graphics.Outline outline)
    {
        outline.SetRoundRect(0, 0, view.Width, view.Height, _cornerRadius);
    }
}
#endif
