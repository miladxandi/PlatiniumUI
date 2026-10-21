namespace PlatiniumUI.Fluent.DesignSystem.Theme;

/// <summary>
/// Contract for Fluent Design themes. Defines all color tokens that must be
/// provided by both light and dark theme implementations.
/// </summary>
public interface IFluentTheme
{
    /// <summary>Theme display name</summary>
    string Name { get; }

    /// <summary>Whether this is a dark theme</summary>
    bool IsDark { get; }

    // ─── Background Colors ──────────────────────────────────────────────────
    Color Background { get; }
    Color BackgroundSecondary { get; }
    Color BackgroundTertiary { get; }
    Color Surface { get; }
    Color SurfaceVariant { get; }
    Color SurfaceCard { get; }

    // ─── Text Colors ────────────────────────────────────────────────────────
    Color TextPrimary { get; }
    Color TextSecondary { get; }
    Color TextTertiary { get; }
    Color TextDisabled { get; }
    Color TextOnAccent { get; }

    // ─── Accent Colors ──────────────────────────────────────────────────────
    Color Accent { get; }
    Color AccentHover { get; }
    Color AccentPressed { get; }
    Color AccentLight { get; }

    // ─── Border Colors ──────────────────────────────────────────────────────
    Color Border { get; }
    Color BorderSubtle { get; }
    Color BorderStrong { get; }
    Color FocusStroke { get; }

    // ─── Semantic Colors ────────────────────────────────────────────────────
    Color Success { get; }
    Color Warning { get; }
    Color Error { get; }
    Color Info { get; }

    // ─── Interaction ────────────────────────────────────────────────────────
    Color HoverOverlay { get; }
    Color PressedOverlay { get; }
    Color Scrim { get; }
}
