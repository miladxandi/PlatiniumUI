namespace PlatiniumUI.Fluent.DesignSystem.Theme;

/// <summary>
/// Manages the current Fluent theme and notifies listeners of theme changes.
/// Supports runtime theme switching between light and dark modes.
/// </summary>
public class FluentThemeManager
{
    private static readonly Lazy<FluentThemeManager> _instance =
        new(() => new FluentThemeManager());

    /// <summary>Singleton instance of the theme manager</summary>
    public static FluentThemeManager Current => _instance.Value;

    private IFluentTheme _currentTheme;

    /// <summary>
    /// Event raised when the theme changes.
    /// </summary>
    public event EventHandler<FluentThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// The current active theme.
    /// </summary>
    public IFluentTheme CurrentTheme
    {
        get => _currentTheme;
        private set
        {
            if (_currentTheme == value) return;
            var oldTheme = _currentTheme;
            _currentTheme = value;
            ThemeChanged?.Invoke(this, new FluentThemeChangedEventArgs(oldTheme, value));
        }
    }

    /// <summary>
    /// Whether the current theme is dark.
    /// </summary>
    public bool IsDarkMode => _currentTheme.IsDark;

    private FluentThemeManager()
    {
        _currentTheme = new FluentLightTheme();
    }

    /// <summary>
    /// Sets the theme to Light mode.
    /// </summary>
    public void SetLightTheme()
    {
        CurrentTheme = new FluentLightTheme();
    }

    /// <summary>
    /// Sets the theme to Dark mode.
    /// </summary>
    public void SetDarkTheme()
    {
        CurrentTheme = new FluentDarkTheme();
    }

    /// <summary>
    /// Toggles between light and dark themes.
    /// </summary>
    public void ToggleTheme()
    {
        if (IsDarkMode)
            SetLightTheme();
        else
            SetDarkTheme();
    }

    /// <summary>
    /// Sets a custom theme implementation.
    /// </summary>
    public void SetTheme(IFluentTheme theme)
    {
        CurrentTheme = theme;
    }

    /// <summary>
    /// Follows the system theme preference if available.
    /// Falls back to light theme if system preference cannot be determined.
    /// </summary>
    public void FollowSystemTheme()
    {
        var appTheme = Application.Current?.UserAppTheme;
        if (appTheme == AppTheme.Dark)
            SetDarkTheme();
        else if (appTheme == AppTheme.Light)
            SetLightTheme();
        else
            SetLightTheme(); // Default fallback
    }
}

/// <summary>
/// Event args for theme change notifications.
/// </summary>
public class FluentThemeChangedEventArgs : EventArgs
{
    public IFluentTheme OldTheme { get; }
    public IFluentTheme NewTheme { get; }

    public FluentThemeChangedEventArgs(IFluentTheme oldTheme, IFluentTheme newTheme)
    {
        OldTheme = oldTheme;
        NewTheme = newTheme;
    }
}
