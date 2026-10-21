using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Platform;

namespace PlatiniumUI.Fluent.Controls.Buttons;

/// <summary>
/// Fluent Design toggle button with on/off states.
/// </summary>
public class FlToggleButton : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlToggleButton), string.Empty,
            propertyChanged: (b, _, _) => ((FlToggleButton)b).UpdateContent());

    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(FlToggleButton), false,
            propertyChanged: (b, _, _) => ((FlToggleButton)b).ApplyVisualState());

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FlToggleButton), FluentCornerRadius.Button,
            propertyChanged: (b, _, _) => ((FlToggleButton)b).ApplyVisualState());

    public event EventHandler<bool>? Toggled;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private readonly Border _border;
    private readonly Label _textLabel;

    public FlToggleButton()
    {
        _textLabel = new Label
        {
            FontSize = 14,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Button)
            },
            Stroke = Colors.Transparent,
            StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.ButtonPaddingHorizontal, FluentSpacing.ButtonPaddingVertical),
            Content = _textLabel,
            HeightRequest = 40,
            HorizontalOptions = LayoutOptions.Start
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        _border.GestureRecognizers.Add(tap);

        Content = _border;
        ApplyVisualState();

        this.ObserveThemeChanged(ApplyVisualState);
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled) return;
        IsToggled = !IsToggled;
        Toggled?.Invoke(this, IsToggled);
    }

    private void UpdateContent()
    {
        _textLabel.Text = Text;
    }

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(CornerRadius)
        };

        if (IsToggled)
        {
            _border.BackgroundColor = theme.Accent;
            _border.Stroke = Colors.Transparent;
            _textLabel.TextColor = theme.TextOnAccent;
        }
        else
        {
            _border.BackgroundColor = Colors.Transparent;
            _border.Stroke = theme.Border;
            _textLabel.TextColor = theme.TextPrimary;
        }

        if (!IsEnabled)
        {
            _border.BackgroundColor = theme.IsDark ? Color.FromArgb("#3D3D3D") : Color.FromArgb("#E0E0E0");
            _textLabel.TextColor = theme.TextDisabled;
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        FluentRippleEffect.Apply(_border);
    }
}
