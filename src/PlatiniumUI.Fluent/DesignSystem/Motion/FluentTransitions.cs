namespace PlatiniumUI.Fluent.DesignSystem.Motion;

/// <summary>
/// Reusable transition helpers for common Fluent Design animations.
/// </summary>
public static class FluentTransitions
{
    public static async Task FadeInAsync(View view, uint duration = 0)
    {
        duration = duration > 0 ? duration : FluentAnimations.Fast;
        view.Opacity = 0;
        view.IsVisible = true;
        await view.FadeToAsync(1, duration, FluentAnimations.Entrance);
    }

    public static async Task FadeOutAsync(View view, uint duration = 0)
    {
        duration = duration > 0 ? duration : FluentAnimations.Fast;
        await view.FadeToAsync(0, duration, FluentAnimations.Exit);
        view.IsVisible = false;
    }

    public static async Task SlideInAsync(View view, SlideDirection direction = SlideDirection.FromBottom, double distance = 20, uint duration = 0)
    {
        duration = duration > 0 ? duration : FluentAnimations.Gentle;
        view.IsVisible = true;
        view.Opacity = 0;

        double startX = direction switch
        {
            SlideDirection.FromLeft => -distance,
            SlideDirection.FromRight => distance,
            _ => 0
        };

        double startY = direction switch
        {
            SlideDirection.FromBottom => distance,
            SlideDirection.FromTop => -distance,
            _ => 0
        };

        view.TranslationX = startX;
        view.TranslationY = startY;

        await Task.WhenAll(
            view.FadeToAsync(1, duration, FluentAnimations.Entrance),
            view.TranslateToAsync(0, 0, duration, FluentAnimations.Entrance)
        );
    }

    public static async Task SlideOutAsync(View view, SlideDirection direction = SlideDirection.ToBottom, double distance = 20, uint duration = 0)
    {
        duration = duration > 0 ? duration : FluentAnimations.Gentle;

        double endX = direction switch
        {
            SlideDirection.ToLeft => -distance,
            SlideDirection.ToRight => distance,
            _ => 0
        };

        double endY = direction switch
        {
            SlideDirection.ToBottom => distance,
            SlideDirection.ToTop => -distance,
            _ => 0
        };

        await Task.WhenAll(
            view.FadeToAsync(0, duration, FluentAnimations.Exit),
            view.TranslateToAsync(endX, endY, duration, FluentAnimations.Exit)
        );
        view.IsVisible = false;
    }

    public static async Task ScaleInAsync(View view, double fromScale = 0.9, uint duration = 0)
    {
        duration = duration > 0 ? duration : FluentAnimations.Gentle;
        view.IsVisible = true;
        view.Opacity = 0;
        view.Scale = fromScale;

        await Task.WhenAll(
            view.FadeToAsync(1, duration, FluentAnimations.Entrance),
            view.ScaleToAsync(1, duration, FluentAnimations.Entrance)
        );
    }

    public static async Task ScaleOutAsync(View view, double toScale = 0.9, uint duration = 0)
    {
        duration = duration > 0 ? duration : FluentAnimations.Gentle;

        await Task.WhenAll(
            view.FadeToAsync(0, duration, FluentAnimations.Exit),
            view.ScaleToAsync(toScale, duration, FluentAnimations.Exit)
        );
        view.IsVisible = false;
    }

    public static async Task PulseAsync(View view, double scale = 1.05, uint duration = 200)
    {
        await view.ScaleToAsync(scale, duration / 2, FluentAnimations.Standard);
        await view.ScaleToAsync(1.0, duration / 2, FluentAnimations.Standard);
    }

    public static async Task ShakeAsync(View view, double distance = 8, uint duration = 50)
    {
        await view.TranslateToAsync(-distance, 0, duration, FluentAnimations.Standard);
        await view.TranslateToAsync(distance, 0, duration, FluentAnimations.Standard);
        await view.TranslateToAsync(-distance / 2, 0, duration, FluentAnimations.Standard);
        await view.TranslateToAsync(distance / 2, 0, duration, FluentAnimations.Standard);
        await view.TranslateToAsync(0, 0, duration, FluentAnimations.Standard);
    }
}

public enum SlideDirection
{
    FromBottom,
    FromTop,
    FromLeft,
    FromRight,
    ToBottom,
    ToTop,
    ToLeft,
    ToRight
}
