using PlatiniumUI.Fluent.DesignSystem.Theme;

namespace PlatiniumUI.Fluent.Extensions;

/// <summary>
/// Extension methods for configuring PlatiniumUI.Fluent in a MAUI application.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Registers PlatiniumUI.Fluent services, fonts, and initializes the design system.
    /// Call this in your MauiProgram.CreateMauiApp() method.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.UsePlatiniumUI();
    /// </code>
    /// This registers the FluentSystemIcons font (alias "FluentUI") and the theme manager.
    /// Use icons via <c>FluentIcons</c> constants with <c>FontFamily = FluentIcons.FontFamily</c>.
    /// </example>
    public static MauiAppBuilder UsePlatiniumUI(
        this MauiAppBuilder builder,
        Action<FluentThemeManager>? configureTheme = null)
    {
        var themeManager = FluentThemeManager.Current;
        configureTheme?.Invoke(themeManager);
        builder.Services.AddSingleton(themeManager);

        // Register Fluent System Icons font
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("FluentSystemIcons-Regular.ttf", "FluentUI");
        });

        return builder;
    }
}
