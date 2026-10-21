namespace PlatiniumUI.Fluent.DesignSystem.Theme;

/// <summary>
/// OK Pars dark theme. Dark surfaces tinted toward the brand deep green, with
/// a slightly lighter accent for contrast/visibility on dark backgrounds.
/// Mirrors <see cref="OkParsLightTheme"/>'s token slots.
/// </summary>
public class OkParsDarkTheme : IFluentTheme
{
    public string Name => "OK Pars Dark";
    public bool IsDark => true;

    // ─── Background Colors ──────────────────────────────────────────────────
    public Color Background => Color.FromArgb("#0f1a16");           // deep green-gray base
    public Color BackgroundSecondary => Color.FromArgb("#16221d");
    public Color BackgroundTertiary => Color.FromArgb("#1c2b24");
    public Color Surface => Color.FromArgb("#16221d");
    public Color SurfaceVariant => Color.FromArgb("#1c2b24");
    public Color SurfaceCard => Color.FromArgb("#1a2820");

    // ─── Text Colors ────────────────────────────────────────────────────────
    public Color TextPrimary => Color.FromArgb("#f0f2f1");
    public Color TextSecondary => Color.FromArgb("#b7c3be");
    public Color TextTertiary => Color.FromArgb("#7f8c87");
    public Color TextDisabled => Color.FromArgb("#55605b");
    public Color TextOnAccent => Color.FromArgb("#ffffff");

    // ─── Accent Colors (lighter green for dark visibility) ──────────────────
    public Color Accent => Color.FromArgb("#2d6a54");               // --color-primary-light
    public Color AccentHover => Color.FromArgb("#3d8b6f");
    public Color AccentPressed => Color.FromArgb("#1a4a3a");        // --color-primary
    public Color AccentLight => Color.FromArgb("#0d2818");          // --color-primary-dark

    // ─── Border Colors ──────────────────────────────────────────────────────
    public Color Border => Color.FromArgb("#2a3b33");
    public Color BorderSubtle => Color.FromArgb("#223029");
    public Color BorderStrong => Color.FromArgb("#3a4f45");
    public Color FocusStroke => Color.FromArgb("#4a9d7f");

    // ─── Semantic Colors (brightened for dark) ──────────────────────────────
    public Color Success => Color.FromArgb("#4ade80");
    public Color Warning => Color.FromArgb("#fbbf24");
    public Color Error => Color.FromArgb("#f87171");
    public Color Info => Color.FromArgb("#60a5fa");

    // ─── Interaction ────────────────────────────────────────────────────────
    public Color HoverOverlay => Color.FromArgb("#0FFFFFFF");       // ~6% white
    public Color PressedOverlay => Color.FromArgb("#19FFFFFF");     // ~10% white
    public Color Scrim => Color.FromArgb("#99000000");              // 60% black
}
