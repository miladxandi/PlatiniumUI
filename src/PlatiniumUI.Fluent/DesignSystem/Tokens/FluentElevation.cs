namespace PlatiniumUI.Fluent.DesignSystem.Tokens;

/// <summary>
/// Fluent Design System elevation levels using shadows.
/// Creates depth hierarchy following Fluent's layering principles.
/// </summary>
public static class FluentElevation
{
    // ─── Elevation Levels ───────────────────────────────────────────────────

    /// <summary>Level 0 - Flat (no shadow) - Inline elements, text</summary>
    public static readonly Shadow Level0 = CreateShadow(0, 0, 0, Colors.Transparent);

    /// <summary>Level 1 - Card/Resting - Cards at rest, tiles</summary>
    public static readonly Shadow Level1 = CreateShadow(0, 2, 4, 0.08);

    /// <summary>Level 2 - Raised - Hovered cards, buttons</summary>
    public static readonly Shadow Level2 = CreateShadow(0, 4, 8, 0.12);

    /// <summary>Level 3 - Flyout - Dropdowns, tooltips, popovers</summary>
    public static readonly Shadow Level3 = CreateShadow(0, 8, 16, 0.14);

    /// <summary>Level 4 - Dialog - Modals, dialogs, bottom sheets</summary>
    public static readonly Shadow Level4 = CreateShadow(0, 16, 32, 0.18);

    /// <summary>Level 5 - Maximum - Full-screen overlays (rarely used)</summary>
    public static readonly Shadow Level5 = CreateShadow(0, 24, 48, 0.22);

    // ─── Semantic Elevation ─────────────────────────────────────────────────

    /// <summary>Shadow for cards at rest</summary>
    public static readonly Shadow Card = Level1;

    /// <summary>Shadow for cards on hover/press</summary>
    public static readonly Shadow CardHover = Level2;

    /// <summary>Shadow for buttons at rest</summary>
    public static readonly Shadow Button = Level1;

    /// <summary>Shadow for buttons on hover</summary>
    public static readonly Shadow ButtonHover = Level2;

    /// <summary>Shadow for floating action buttons</summary>
    public static readonly Shadow FAB = Level2;

    /// <summary>Shadow for FAB on hover</summary>
    public static readonly Shadow FABHover = Level3;

    /// <summary>Shadow for dropdown menus</summary>
    public static readonly Shadow Dropdown = Level3;

    /// <summary>Shadow for tooltips</summary>
    public static readonly Shadow Tooltip = Level3;

    /// <summary>Shadow for dialogs</summary>
    public static readonly Shadow Dialog = Level4;

    /// <summary>Shadow for bottom sheets</summary>
    public static readonly Shadow BottomSheet = Level4;

    /// <summary>Shadow for navigation drawers</summary>
    public static readonly Shadow NavigationDrawer = Level3;

    // ─── Dark Theme Shadows ─────────────────────────────────────────────────
    // Dark theme uses more subtle, diffused shadows with slight glow

    /// <summary>Card shadow for dark theme</summary>
    public static readonly Shadow DarkCard = CreateShadow(0, 2, 8, 0.3);

    /// <summary>Dialog shadow for dark theme</summary>
    public static readonly Shadow DarkDialog = CreateShadow(0, 16, 40, 0.4);

    // ─── Helper ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a Shadow with the given parameters.
    /// </summary>
    private static Shadow CreateShadow(double offsetX, double offsetY, double radius, double opacity)
    {
        return new Shadow
        {
            Offset = new Point(offsetX, offsetY),
            Radius = (float)radius,
            Opacity = (float)opacity,
            Brush = new SolidColorBrush(Colors.Black)
        };
    }

    /// <summary>
    /// Creates a Shadow with custom color.
    /// </summary>
    private static Shadow CreateShadow(double offsetX, double offsetY, double radius, Color color)
    {
        return new Shadow
        {
            Offset = new Point(offsetX, offsetY),
            Radius = (float)radius,
            Opacity = 1f,
            Brush = new SolidColorBrush(color)
        };
    }
}
