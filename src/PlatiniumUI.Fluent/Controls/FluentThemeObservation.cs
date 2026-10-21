using PlatiniumUI.Fluent.DesignSystem.Theme;

namespace PlatiniumUI.Fluent.Controls;

/// <summary>
/// Opt-in theme observation that avoids the singleton leak every control previously
/// had by doing <c>FluentThemeManager.Current.ThemeChanged += lambda</c> in its
/// constructor and never unsubscribing. The stored handler is attached on load and
/// detached on unload, so a control that leaves the visual tree stops being retained
/// by the process-wide theme manager — critical across ~44 role pages.
/// </summary>
public static class FluentThemeObservation
{
    /// <summary>
    /// Registers <paramref name="apply"/> to run whenever the theme changes, for the
    /// lifetime of the element's attachment to a window. Safe to call from a constructor.
    /// </summary>
    public static void ObserveThemeChanged(this VisualElement element, Action apply)
    {
        var manager = FluentThemeManager.Current;
        var handler = new EventHandler<FluentThemeChangedEventArgs>((_, _) =>
            element.Dispatcher.Dispatch(apply));

        element.Loaded += OnLoaded;
        element.Unloaded += OnUnloaded;

        void OnLoaded(object? sender, EventArgs e)
        {
            manager.ThemeChanged += handler;
            apply();
        }

        void OnUnloaded(object? sender, EventArgs e) => manager.ThemeChanged -= handler;
    }
}
