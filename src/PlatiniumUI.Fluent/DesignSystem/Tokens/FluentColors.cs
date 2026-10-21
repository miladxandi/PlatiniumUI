namespace PlatiniumUI.Fluent.DesignSystem.Tokens;

/// <summary>
/// Fluent Design System color tokens built around the primary accent #F26522.
/// Provides a complete palette for light and dark themes following Microsoft's Fluent Design principles.
/// </summary>
public static class FluentColors
{
    // ─── Primary Accent (#F26522) ───────────────────────────────────────────
    // Derived tints (lighter) and shades (darker) of the accent color

    public static readonly Color Accent50 = Color.FromArgb("#FFF3EC");   // Very light tint - backgrounds
    public static readonly Color Accent100 = Color.FromArgb("#FEDBC8");  // Light tint
    public static readonly Color Accent200 = Color.FromArgb("#FDBFA0");  // Lighter tint
    public static readonly Color Accent300 = Color.FromArgb("#FBA378");  // Light tint
    public static readonly Color Accent400 = Color.FromArgb("#F98750");  // Tint
    public static readonly Color Accent500 = Color.FromArgb("#F26522");  // Primary accent
    public static readonly Color Accent600 = Color.FromArgb("#D95A1E");  // Shade
    public static readonly Color Accent700 = Color.FromArgb("#BF4F1A");  // Darker shade
    public static readonly Color Accent800 = Color.FromArgb("#A64416");  // Dark shade
    public static readonly Color Accent900 = Color.FromArgb("#8C3A12");  // Darkest shade

    // ─── Neutral Palette ────────────────────────────────────────────────────
    // Grays for text, backgrounds, borders, and surfaces

    public static readonly Color Neutral0 = Color.FromArgb("#FFFFFF");   // White
    public static readonly Color Neutral10 = Color.FromArgb("#FAFAFA");  // Snow
    public static readonly Color Neutral20 = Color.FromArgb("#F5F5F5");  // Ghost white
    public static readonly Color Neutral30 = Color.FromArgb("#EBEBEB");  // Light gray
    public static readonly Color Neutral40 = Color.FromArgb("#E0E0E0");  // Silver
    public static readonly Color Neutral50 = Color.FromArgb("#C4C4C4");  // Gray
    public static readonly Color Neutral60 = Color.FromArgb("#A3A3A3");  // Medium gray
    public static readonly Color Neutral70 = Color.FromArgb("#7A7A7A");  // Dim gray
    public static readonly Color Neutral80 = Color.FromArgb("#5C5C5C");  // Dark gray
    public static readonly Color Neutral90 = Color.FromArgb("#3D3D3D");  // Dimmed text
    public static readonly Color Neutral100 = Color.FromArgb("#292929"); // Dark text
    public static readonly Color Neutral110 = Color.FromArgb("#1A1A1A"); // Near black
    public static readonly Color Neutral120 = Color.FromArgb("#0D0D0D"); // Almost black
    public static readonly Color Neutral130 = Color.FromArgb("#000000"); // Black

    // ─── Semantic Colors ────────────────────────────────────────────────────
    // Colors that convey meaning: success, warning, error, info

    // Success
    public static readonly Color Success = Color.FromArgb("#107C10");
    public static readonly Color SuccessLight = Color.FromArgb("#DFF6DD");
    public static readonly Color SuccessDark = Color.FromArgb("#0B5A0B");

    // Warning
    public static readonly Color Warning = Color.FromArgb("#FFB900");
    public static readonly Color WarningLight = Color.FromArgb("#FFF4CE");
    public static readonly Color WarningDark = Color.FromArgb("#C48C00");

    // Error / Danger
    public static readonly Color Error = Color.FromArgb("#D13438");
    public static readonly Color ErrorLight = Color.FromArgb("#FDE7E9");
    public static readonly Color ErrorDark = Color.FromArgb("#A4262C");

    // Info
    public static readonly Color Info = Color.FromArgb("#0078D4");
    public static readonly Color InfoLight = Color.FromArgb("#DEECF9");
    public static readonly Color InfoDark = Color.FromArgb("#005A9E");

    // ─── Light Theme Surface Colors ─────────────────────────────────────────

    public static readonly Color LightBackground = Color.FromArgb("#FFFFFF");
    public static readonly Color LightBackgroundSecondary = Color.FromArgb("#FAFAFA");
    public static readonly Color LightBackgroundTertiary = Color.FromArgb("#F5F5F5");
    public static readonly Color LightSurface = Color.FromArgb("#FFFFFF");
    public static readonly Color LightSurfaceVariant = Color.FromArgb("#F5F5F5");
    public static readonly Color LightSurfaceCard = Color.FromArgb("#FFFFFF");

    // ─── Dark Theme Surface Colors ──────────────────────────────────────────

    public static readonly Color DarkBackground = Color.FromArgb("#1A1A1A");
    public static readonly Color DarkBackgroundSecondary = Color.FromArgb("#242424");
    public static readonly Color DarkBackgroundTertiary = Color.FromArgb("#2E2E2E");
    public static readonly Color DarkSurface = Color.FromArgb("#2E2E2E");
    public static readonly Color DarkSurfaceVariant = Color.FromArgb("#383838");
    public static readonly Color DarkSurfaceCard = Color.FromArgb("#2E2E2E");

    // ─── Text Colors (Light Theme) ──────────────────────────────────────────

    public static readonly Color LightTextPrimary = Color.FromArgb("#1A1A1A");
    public static readonly Color LightTextSecondary = Color.FromArgb("#5C5C5C");
    public static readonly Color LightTextTertiary = Color.FromArgb("#7A7A7A");
    public static readonly Color LightTextDisabled = Color.FromArgb("#A3A3A3");
    public static readonly Color LightTextOnAccent = Color.FromArgb("#FFFFFF");

    // ─── Text Colors (Dark Theme) ───────────────────────────────────────────

    public static readonly Color DarkTextPrimary = Color.FromArgb("#FFFFFF");
    public static readonly Color DarkTextSecondary = Color.FromArgb("#C4C4C4");
    public static readonly Color DarkTextTertiary = Color.FromArgb("#A3A3A3");
    public static readonly Color DarkTextDisabled = Color.FromArgb("#7A7A7A");
    public static readonly Color DarkTextOnAccent = Color.FromArgb("#FFFFFF");

    // ─── Border Colors ──────────────────────────────────────────────────────

    public static readonly Color LightBorder = Color.FromArgb("#E0E0E0");
    public static readonly Color LightBorderSubtle = Color.FromArgb("#EBEBEB");
    public static readonly Color LightBorderStrong = Color.FromArgb("#C4C4C4");

    public static readonly Color DarkBorder = Color.FromArgb("#3D3D3D");
    public static readonly Color DarkBorderSubtle = Color.FromArgb("#333333");
    public static readonly Color DarkBorderStrong = Color.FromArgb("#5C5C5C");

    // ─── Focus & Interaction ────────────────────────────────────────────────

    public static readonly Color FocusStroke = Color.FromArgb("#1A1A1A");
    public static readonly Color DarkFocusStroke = Color.FromArgb("#FFFFFF");
    public static readonly Color HoverOverlay = Color.FromArgb("#0A000000");
    public static readonly Color PressedOverlay = Color.FromArgb("#14000000");
    public static readonly Color DarkHoverOverlay = Color.FromArgb("#0AFFFFFF");
    public static readonly Color DarkPressedOverlay = Color.FromArgb("#14FFFFFF");

    // ─── Utility ────────────────────────────────────────────────────────────

    public static readonly Color Transparent = Colors.Transparent;
    public static readonly Color Scrim = Color.FromArgb("#52000000");     // 32% black overlay
    public static readonly Color DarkScrim = Color.FromArgb("#52000000");

    /// <summary>
    /// Returns the appropriate accent color shade/tint by level (50-900).
    /// </summary>
    public static Color GetAccent(int level) => level switch
    {
        50 => Accent50,
        100 => Accent100,
        200 => Accent200,
        300 => Accent300,
        400 => Accent400,
        500 => Accent500,
        600 => Accent600,
        700 => Accent700,
        800 => Accent800,
        900 => Accent900,
        _ => Accent500
    };
}
