using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.DesignSystem.Theme;

/// <summary>
/// Fluent Design Light Theme implementation.
/// Clean, bright surfaces with high contrast text.
/// </summary>
public class FluentLightTheme : IFluentTheme
{
    public string Name => "Light";
    public bool IsDark => false;

    // ─── Background Colors ──────────────────────────────────────────────────
    public Color Background => FluentColors.LightBackground;
    public Color BackgroundSecondary => FluentColors.LightBackgroundSecondary;
    public Color BackgroundTertiary => FluentColors.LightBackgroundTertiary;
    public Color Surface => FluentColors.LightSurface;
    public Color SurfaceVariant => FluentColors.LightSurfaceVariant;
    public Color SurfaceCard => FluentColors.LightSurfaceCard;

    // ─── Text Colors ────────────────────────────────────────────────────────
    public Color TextPrimary => FluentColors.LightTextPrimary;
    public Color TextSecondary => FluentColors.LightTextSecondary;
    public Color TextTertiary => FluentColors.LightTextTertiary;
    public Color TextDisabled => FluentColors.LightTextDisabled;
    public Color TextOnAccent => FluentColors.LightTextOnAccent;

    // ─── Accent Colors ──────────────────────────────────────────────────────
    public Color Accent => FluentColors.Accent500;
    public Color AccentHover => FluentColors.Accent600;
    public Color AccentPressed => FluentColors.Accent700;
    public Color AccentLight => FluentColors.Accent50;

    // ─── Border Colors ──────────────────────────────────────────────────────
    public Color Border => FluentColors.LightBorder;
    public Color BorderSubtle => FluentColors.LightBorderSubtle;
    public Color BorderStrong => FluentColors.LightBorderStrong;
    public Color FocusStroke => FluentColors.FocusStroke;

    // ─── Semantic Colors ────────────────────────────────────────────────────
    public Color Success => FluentColors.Success;
    public Color Warning => FluentColors.Warning;
    public Color Error => FluentColors.Error;
    public Color Info => FluentColors.Info;

    // ─── Interaction ────────────────────────────────────────────────────────
    public Color HoverOverlay => FluentColors.HoverOverlay;
    public Color PressedOverlay => FluentColors.PressedOverlay;
    public Color Scrim => FluentColors.Scrim;
}
