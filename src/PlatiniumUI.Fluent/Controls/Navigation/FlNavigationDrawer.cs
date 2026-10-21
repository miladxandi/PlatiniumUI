using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Navigation;

/// <summary>
/// Fluent Design navigation drawer (left/right, overlay/push).
/// </summary>
public class FlNavigationDrawer : ContentView
{
    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(FlNavigationDrawer), false,
            propertyChanged: (b, _, _) => ((FlNavigationDrawer)b).AnimateDrawer());

    public static readonly BindableProperty PositionProperty =
        BindableProperty.Create(nameof(Position), typeof(FlDrawerPosition), typeof(FlNavigationDrawer), FlDrawerPosition.Left);

    public static readonly BindableProperty DrawerContentProperty =
        BindableProperty.Create(nameof(DrawerContent), typeof(View), typeof(FlNavigationDrawer), null,
            propertyChanged: (b, _, _) => ((FlNavigationDrawer)b).UpdateContent());

    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public FlDrawerPosition Position { get => (FlDrawerPosition)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    public View? DrawerContent { get => (View?)GetValue(DrawerContentProperty); set => SetValue(DrawerContentProperty, value); }

    public event EventHandler? Opened;
    public event EventHandler? Closed;

    private readonly Border _drawer;
    private readonly Grid _overlay;

    public FlNavigationDrawer()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _drawer = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(0) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Surface,
            WidthRequest = 280,
            VerticalOptions = LayoutOptions.Fill,
            Shadow = FluentElevation.Level3
        };

        _overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#00000000"),
            IsVisible = false,
            Children = { _drawer }
        };

        Content = _overlay;
    }

    private void UpdateContent()
    {
        if (DrawerContent != null)
        {
            _drawer.Content = DrawerContent;
        }
    }

    private async void AnimateDrawer()
    {
        if (IsOpen)
        {
            _overlay.IsVisible = true;
            _drawer.HorizontalOptions = Position == FlDrawerPosition.Left ? LayoutOptions.Start : LayoutOptions.End;
            _drawer.TranslationX = Position == FlDrawerPosition.Left ? -280 : 280;

            await Task.WhenAll(
                _drawer.TranslateToAsync(0, 0, FluentAnimations.Gentle, FluentAnimations.Entrance),
                _overlay.FadeToAsync(1, FluentAnimations.Gentle)
            );
            _overlay.BackgroundColor = FluentThemeManager.Current.CurrentTheme.Scrim;
            Opened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            double targetX = Position == FlDrawerPosition.Left ? -280 : 280;
            await Task.WhenAll(
                _drawer.TranslateToAsync(targetX, 0, FluentAnimations.Gentle, FluentAnimations.Exit),
                _overlay.FadeToAsync(0, FluentAnimations.Gentle)
            );
            _overlay.IsVisible = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}

public enum FlDrawerPosition { Left, Right }
