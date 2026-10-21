using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.DesignSystem.Theme;

/// <summary>
/// Fluent Design Dark Theme implementation.
/// Deep, rich dark surfaces with carefully calibrated contrast.
/// </summary>
public class FluentDarkTheme : IFluentTheme
{
    public string Name => "Dark";
    public bool IsDark => true;

    // ─── Background Colors ──────────────────────────────────────────────────
    public Color Background => FluentColors.DarkBackground;
    public Color BackgroundSecondary => FluentColors.DarkBackgroundSecondary;
    public Color BackgroundTertiary => FluentColors.DarkBackgroundTertiary;
    public Color Surface => FluentColors.DarkSurface;
    public Color SurfaceVariant => FluentColors.DarkSurfaceVariant;
    public Color SurfaceCard => FluentColors.DarkSurfaceCard;

    // ─── Text Colors ────────────────────────────────────────────────────────
    public Color TextPrimary => FluentColors.DarkTextPrimary;
    public Color TextSecondary => FluentColors.DarkTextSecondary;
    public Color TextTertiary => FluentColors.DarkTextTertiary;
    public Color TextDisabled => FluentColors.DarkTextDisabled;
    public Color TextOnAccent => FluentColors.DarkTextOnAccent;

    // ─── Accent Colors ──────────────────────────────────────────────────────
    // In dark mode, accent is slightly lighter for better visibility
    public Color Accent => FluentColors.Accent400;
    public Color AccentHover => FluentColors.Accent300;
    public Color AccentPressed => FluentColors.Accent500;
    public Color AccentLight => FluentColors.Accent900;

    // ─── Border Colors ──────────────────────────────────────────────────────
    public Color Border => FluentColors.DarkBorder;
    public Color BorderSubtle => FluentColors.DarkBorderSubtle;
    public Color BorderStrong => FluentColors.DarkBorderStrong;
    public Color FocusStroke => FluentColors.DarkFocusStroke;

    // ─── Semantic Colors ────────────────────────────────────────────────────
    // In dark mode, semantic colors are slightly brighter for visibility
    public Color Success => Color.FromArgb("#6CCB5F");
    public Color Warning => Color.FromArgb("#FFE484");
    public Color Error => Color.FromArgb("#FF6B6F");
    public Color Info => Color.FromArgb("#4FC3F7");

    // ─── Interaction ────────────────────────────────────────────────────────
    public Color HoverOverlay => FluentColors.DarkHoverOverlay;
    public Color PressedOverlay => FluentColors.DarkPressedOverlay;
    public Color Scrim => FluentColors.DarkScrim;
}
