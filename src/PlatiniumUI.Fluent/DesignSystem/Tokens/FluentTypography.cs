namespace PlatiniumUI.Fluent.DesignSystem.Tokens;

/// <summary>
/// Fluent Design System typography scale based on Segoe UI Variable.
/// Follows the Fluent type ramp for consistent text hierarchy.
/// </summary>
public static class FluentTypography
{
    // ─── Font Families ──────────────────────────────────────────────────────

    public const string Primary = "Segoe UI Variable";
    public const string PrimaryFallback = "Segoe UI";
    public const string Monospace = "Cascadia Mono";
    public const string MonospaceFallback = "Consolas";

    // ─── Type Ramp (Fluent Design) ──────────────────────────────────────────
    // Each level defines size, line height, and letter spacing

    /// <summary>Display - Hero text, 68px (used sparingly)</summary>
    public const double DisplaySize = 68;
    public const double DisplayLineHeight = 92;
    public const double DisplayLetterSpacing = -1.5;

    /// <summary>Large Title - Page headers, 40px</summary>
    public const double LargeTitleSize = 40;
    public const double LargeTitleLineHeight = 52;
    public const double LargeTitleLetterSpacing = -0.8;

    /// <summary>Title - Section headers, 28px</summary>
    public const double TitleSize = 28;
    public const double TitleLineHeight = 36;
    public const double TitleLetterSpacing = -0.4;

    /// <summary>Subtitle - Card titles, dialog headers, 20px</summary>
    public const double SubtitleSize = 20;
    public const double SubtitleLineHeight = 28;
    public const double SubtitleLetterSpacing = 0;

    /// <summary>Body Strong - Emphasized body text, 16px</summary>
    public const double BodyStrongSize = 16;
    public const double BodyStrongLineHeight = 24;
    public const double BodyStrongLetterSpacing = 0;

    /// <summary>Body - Default body text, 14px</summary>
    public const double BodySize = 14;
    public const double BodyLineHeight = 20;
    public const double BodyLetterSpacing = 0;

    /// <summary>Body Small - Secondary text, 12px</summary>
    public const double BodySmallSize = 12;
    public const double BodySmallLineHeight = 16;
    public const double BodySmallLetterSpacing = 0.2;

    /// <summary>Caption - Labels, timestamps, 12px</summary>
    public const double CaptionSize = 12;
    public const double CaptionLineHeight = 16;
    public const double CaptionLetterSpacing = 0.4;

    /// <summary>Overline - Category labels, 10px</summary>
    public const double OverlineSize = 10;
    public const double OverlineLineHeight = 14;
    public const double OverlineLetterSpacing = 1.5;

    // ─── Font Weights ───────────────────────────────────────────────────────

    public const int WeightLight = 300;
    public const int WeightRegular = 400;
    public const int WeightMedium = 500;
    public const int WeightSemiBold = 600;
    public const int WeightBold = 700;

    // ─── Helper: Create Font from type ramp level ───────────────────────────

    /// <summary>
    /// Returns a Font instance for the given type ramp level.
    /// </summary>
    public static Microsoft.Maui.Font GetFont(FluentFontType type) => type switch
    {
        FluentFontType.Display => Microsoft.Maui.Font.OfSize(Primary, DisplaySize),
        FluentFontType.LargeTitle => Microsoft.Maui.Font.OfSize(Primary, LargeTitleSize),
        FluentFontType.Title => Microsoft.Maui.Font.OfSize(Primary, TitleSize),
        FluentFontType.Subtitle => Microsoft.Maui.Font.OfSize(Primary, SubtitleSize),
        FluentFontType.BodyStrong => Microsoft.Maui.Font.OfSize(Primary, BodyStrongSize).WithWeight(FontWeight.Bold),
        FluentFontType.Body => Microsoft.Maui.Font.OfSize(Primary, BodySize),
        FluentFontType.BodySmall => Microsoft.Maui.Font.OfSize(Primary, BodySmallSize),
        FluentFontType.Caption => Microsoft.Maui.Font.OfSize(Primary, CaptionSize),
        FluentFontType.Overline => Microsoft.Maui.Font.OfSize(Primary, OverlineSize),
        _ => Microsoft.Maui.Font.OfSize(Primary, BodySize)
    };
}

/// <summary>
/// Defines the type ramp levels for Fluent typography.
/// </summary>
public enum FluentFontType
{
    Display,
    LargeTitle,
    Title,
    Subtitle,
    BodyStrong,
    Body,
    BodySmall,
    Caption,
    Overline
}
