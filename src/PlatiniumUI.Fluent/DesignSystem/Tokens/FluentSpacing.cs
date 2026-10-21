namespace PlatiniumUI.Fluent.DesignSystem.Tokens;

/// <summary>
/// Fluent Design System spacing tokens based on a 4px grid.
/// All spacing values are multiples of 4 for visual consistency.
/// </summary>
public static class FluentSpacing
{
    // ─── Base Grid (4px multiples) ──────────────────────────────────────────

    public const double XXS = 2;    // Micro spacing (tight elements)
    public const double XS = 4;     // Extra small (icon padding)
    public const double S = 8;      // Small (inline gaps)
    public const double M = 12;     // Medium (compact padding)
    public const double L = 16;     // Large (standard padding)
    public const double XL = 20;    // Extra large (section spacing)
    public const double XXL = 24;   // 2X large (card padding)
    public const double XXXL = 32;  // 3X large (group spacing)
    public const double XXXXL = 40; // 4X large (section gap)
    public const double XXXXXL = 48;// 5X large (page margin)
    public const double Scale64 = 64; // 64px (large section separation)

    // ─── Semantic Spacing ───────────────────────────────────────────────────

    /// <summary>Padding inside buttons and small interactive elements</summary>
    public const double ButtonPaddingHorizontal = 16;
    public const double ButtonPaddingVertical = 8;

    /// <summary>Padding inside input fields</summary>
    public const double InputPaddingHorizontal = 12;
    public const double InputPaddingVertical = 8;

    /// <summary>Padding inside cards</summary>
    public const double CardPadding = 16;

    /// <summary>Padding inside dialogs</summary>
    public const double DialogPadding = 24;

    /// <summary>Gap between items in a list or group</summary>
    public const double ListItemGap = 8;

    /// <summary>Gap between form fields</summary>
    public const double FormFieldGap = 16;

    /// <summary>Gap between sections</summary>
    public const double SectionGap = 32;

    /// <summary>Page margin (horizontal)</summary>
    public const double PageMarginHorizontal = 16;

    /// <summary>Page margin (vertical)</summary>
    public const double PageMarginVertical = 24;

    // ─── Touch Targets ──────────────────────────────────────────────────────

    /// <summary>Minimum touch target size for mobile (44x44pt per HIG)</summary>
    public const double MinTouchTarget = 44;

    /// <summary>Comfortable touch target size</summary>
    public const double ComfortableTouchTarget = 48;

    /// <summary>Desktop minimum click target</summary>
    public const double MinDesktopTarget = 32;
}
