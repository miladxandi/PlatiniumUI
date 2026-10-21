namespace PlatiniumUI.Fluent.DesignSystem.Theme;

/// <summary>
/// OK Pars light theme. Re-skins every PlatiniumUI control at once by
/// implementing <see cref="IFluentTheme"/> with the palette sourced from the
/// OK Pars web app's <c>design-tokens.css</c> (primary deep green), instead of
/// the library's default orange Fluent accent ramp. The library default remains
/// untouched so the Sample and other consumers keep their Fluent look.
/// </summary>
public class OkParsLightTheme : IFluentTheme
{
    public string Name => "OK Pars Light";
    public bool IsDark => false;

    // ─── Background Colors ──────────────────────────────────────────────────
    public Color Background => Color.FromArgb("#f0f2f1");           // --color-bg
    public Color BackgroundSecondary => Color.FromArgb("#e7ebe9");   // one step deeper than bg
    public Color BackgroundTertiary => Color.FromArgb("#dfe4e2");    // one step deeper still
    public Color Surface => Color.FromArgb("#ffffff");              // --color-surface
    public Color SurfaceVariant => Color.FromArgb("#f9fafb");       // --color-surface-hover
    public Color SurfaceCard => Color.FromArgb("#ffffff");          // cards on light bg

    // ─── Text Colors ────────────────────────────────────────────────────────
    public Color TextPrimary => Color.FromArgb("#111827");          // --color-text-primary
    public Color TextSecondary => Color.FromArgb("#4b5563");        // --color-text-secondary
    public Color TextTertiary => Color.FromArgb("#9ca3af");         // --color-text-muted
    public Color TextDisabled => Color.FromArgb("#c2c8d0");         // muted, disabled
    public Color TextOnAccent => Color.FromArgb("#ffffff");         // --color-text-inverse

    // ─── Accent Colors (OK Pars deep green) ─────────────────────────────────
    public Color Accent => Color.FromArgb("#1a4a3a");               // --color-primary
    public Color AccentHover => Color.FromArgb("#245a47");         // --color-primary-hover
    public Color AccentPressed => Color.FromArgb("#0d2818");       // --color-primary-dark
    public Color AccentLight => Color.FromArgb("#2d6a54");         // --color-primary-light

    // ─── Border Colors ──────────────────────────────────────────────────────
    public Color Border => Color.FromArgb("#e5e7eb");               // --color-border
    public Color BorderSubtle => Color.FromArgb("#f3f4f6");         // --color-border-light
    public Color BorderStrong => Color.FromArgb("#d1d5db");         // one step stronger
    public Color FocusStroke => Color.FromArgb("#2d6a54");          // primary-light focus ring

    // ─── Semantic Colors ────────────────────────────────────────────────────
    public Color Success => Color.FromArgb("#16a34a");              // --color-success
    public Color Warning => Color.FromArgb("#d97706");              // --color-warning
    public Color Error => Color.FromArgb("#dc2626");                // --color-danger
    public Color Info => Color.FromArgb("#2563eb");                 // --color-info

    // ─── Interaction ────────────────────────────────────────────────────────
    public Color HoverOverlay => Color.FromArgb("#0A000000");       // ~4% black
    public Color PressedOverlay => Color.FromArgb("#14000000");     // ~8% black
    public Color Scrim => Color.FromArgb("#80000000");              // 50% black

    /// <summary>Driver-role accent (informational blue) used to tint driver screens.</summary>
    public static Color DriverAccent => Color.FromArgb("#2980B9");
}
