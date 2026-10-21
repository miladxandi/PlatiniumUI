namespace PlatiniumUI.Fluent.DesignSystem.Tokens;

/// <summary>
/// Fluent Design System corner radius tokens.
/// Consistent rounding creates visual harmony across all controls.
/// </summary>
public static class FluentCornerRadius
{
    /// <summary>No rounding (0px) - sharp edges</summary>
    public const double None = 0;

    /// <summary>Small radius (4px) - buttons, inputs, chips</summary>
    public const double Small = 4;

    /// <summary>Medium radius (8px) - cards, dialogs, popups</summary>
    public const double Medium = 8;

    /// <summary>Large radius (12px) - large cards, modals</summary>
    public const double Large = 12;

    /// <summary>Extra large radius (16px) - hero cards, feature sections</summary>
    public const double XLarge = 16;

    /// <summary>Extra extra large radius (20px) - special containers</summary>
    public const double XXLarge = 20;

    /// <summary>Circular (999px) - avatars, FABs, pill buttons</summary>
    public const double Circular = 999;

    // ─── Semantic Corner Radii ──────────────────────────────────────────────

    /// <summary>Corner radius for buttons</summary>
    public const double Button = Small;

    /// <summary>Corner radius for text inputs</summary>
    public const double Input = Small;

    /// <summary>Corner radius for cards</summary>
    public const double Card = Medium;

    /// <summary>Corner radius for dialogs</summary>
    public const double Dialog = Large;

    /// <summary>Corner radius for bottom sheets</summary>
    public const double BottomSheet = XLarge;

    /// <summary>Corner radius for chips/tags</summary>
    public const double Chip = Circular;

    /// <summary>Corner radius for tooltips</summary>
    public const double Tooltip = Small;

    /// <summary>Corner radius for badges</summary>
    public const double Badge = Circular;

    // ─── Helper ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a CornerRadius with the same value on all corners.
    /// </summary>
    public static CornerRadius Uniform(double radius) => new(radius);

    /// <summary>
    /// Creates a CornerRadius with different top and bottom values.
    /// </summary>
    public static CornerRadius TopBottom(double top, double bottom) =>
        new(top, top, bottom, bottom);
}
