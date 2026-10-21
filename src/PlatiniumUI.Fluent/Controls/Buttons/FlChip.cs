using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Platform;

namespace PlatiniumUI.Fluent.Controls.Buttons;

/// <summary>
/// Fluent Design chip/tag control for filtering, labeling, or visual options.
/// </summary>
public class FlChip : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlChip), string.Empty,
            propertyChanged: (b, _, _) => ((FlChip)b).UpdateContent());

    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(FlChip), false,
            propertyChanged: (b, _, _) => ((FlChip)b).ApplyVisualState());

    public static readonly BindableProperty IsClosableProperty =
        BindableProperty.Create(nameof(IsClosable), typeof(bool), typeof(FlChip), false,
            propertyChanged: (b, _, _) => ((FlChip)b).UpdateContent());

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(FlChip), null,
            propertyChanged: (b, _, _) => ((FlChip)b).UpdateContent());

    public event EventHandler? Clicked;
    public event EventHandler? Closed;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsClosable
    {
        get => (bool)GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private readonly Border _border;
    private readonly HorizontalStackLayout _content;
    private readonly Label _iconLabel;
    private readonly Label _textLabel;
    private readonly Label _closeLabel;

    public FlChip()
    {
        _iconLabel = new Label
        {
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _textLabel = new Label
        {
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center
        };

        _closeLabel = new Label
        {
            Text = "\u2715",
            FontSize = 10,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false,
            Margin = new Thickness(4, 0, 0, 0)
        };

        _content = new HorizontalStackLayout
        {
            Spacing = 6,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _iconLabel, _textLabel, _closeLabel }
        };

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Chip)
            },
            Stroke = Colors.Transparent,
            Padding = new Thickness(12, 6),
            Content = _content,
            HorizontalOptions = LayoutOptions.Start
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        _border.GestureRecognizers.Add(tap);

        var closeTap = new TapGestureRecognizer();
        closeTap.Tapped += OnCloseTapped;
        _closeLabel.GestureRecognizers.Add(closeTap);

        Content = _border;
        ApplyVisualState();

        this.ObserveThemeChanged(ApplyVisualState);
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled) return;
        IsSelected = !IsSelected;
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    private void OnCloseTapped(object? sender, TappedEventArgs e)
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateContent()
    {
        _textLabel.Text = Text;
        _iconLabel.IsVisible = !string.IsNullOrEmpty(Icon);
        _iconLabel.Text = Icon ?? string.Empty;
        _closeLabel.IsVisible = IsClosable;
    }

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        if (IsSelected)
        {
            _border.BackgroundColor = theme.Accent;
            _border.Stroke = Colors.Transparent;
            _textLabel.TextColor = theme.TextOnAccent;
            _iconLabel.TextColor = theme.TextOnAccent;
            _closeLabel.TextColor = theme.TextOnAccent;
        }
        else
        {
            _border.BackgroundColor = theme.IsDark
                ? Color.FromArgb("#383838")
                : Color.FromArgb("#F5F5F5");
            _border.Stroke = theme.BorderSubtle;
            _border.StrokeThickness = 1;
            _textLabel.TextColor = theme.TextPrimary;
            _iconLabel.TextColor = theme.TextSecondary;
            _closeLabel.TextColor = theme.TextTertiary;
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        FluentRippleEffect.Apply(_border);
    }
}
