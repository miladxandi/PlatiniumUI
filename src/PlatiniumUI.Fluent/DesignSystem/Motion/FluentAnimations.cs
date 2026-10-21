namespace PlatiniumUI.Fluent.DesignSystem.Motion;

/// <summary>
/// Fluent Design System animation constants.
/// Defines easing curves and durations following Microsoft's motion principles.
/// </summary>
public static class FluentAnimations
{
    // Duration Tokens
    public const uint UltraFast = 50;
    public const uint Faster = 100;
    public const uint Fast = 150;
    public const uint Normal = 200;
    public const uint Gentle = 250;
    public const uint Slow = 300;
    public const uint Slower = 400;
    public const uint VerySlow = 500;

    // Easing Curves
    public static readonly Easing Standard = Easing.CubicInOut;
    public static readonly Easing Entrance = Easing.CubicOut;
    public static readonly Easing Exit = Easing.CubicIn;
    public static readonly Easing Linear = Easing.Linear;
    public static readonly Easing Bounce = Easing.BounceOut;
    public static readonly Easing Spring = new Easing(t => t * t * (3 - 2 * t));
    public static readonly Easing Smooth = Easing.SinInOut;

    // Semantic Durations
    public const uint HoverDuration = UltraFast;
    public const uint PressDuration = Faster;
    public const uint FocusDuration = Fast;
    public const uint TooltipDuration = Fast;
    public const uint DropdownDuration = Normal;
    public const uint DialogDuration = Gentle;
    public const uint BottomSheetDuration = Gentle;
    public const uint ExpandDuration = Gentle;
    public const uint TabSwitchDuration = Normal;
    public const uint PageTransitionDuration = Slow;
    public const uint ShimmerDuration = 1500;
    public const uint ProgressIndeterminateDuration = 1200;
}
