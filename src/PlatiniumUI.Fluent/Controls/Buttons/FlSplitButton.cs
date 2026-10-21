using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Platform;
using PlatiniumUI.Fluent.DesignSystem.Icons;

namespace PlatiniumUI.Fluent.Controls.Buttons;

/// <summary>
/// Fluent Design split button with a primary action and dropdown.
/// </summary>
public class FlSplitButton : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlSplitButton), string.Empty,
            propertyChanged: (b, _, _) => ((FlSplitButton)b).UpdateContent());

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FlSplitButton), FluentCornerRadius.Button,
            propertyChanged: (b, _, _) => ((FlSplitButton)b).ApplyVisualState());

    public event EventHandler? Clicked;
    public event EventHandler? DropdownClicked;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private readonly Border _container;
    private readonly HorizontalStackLayout _layout;
    private readonly Label _textLabel;
    private readonly Label _arrowLabel;
    private readonly Border _divider;

    public FlSplitButton()
    {
        _textLabel = new Label
        {
            FontSize = 14,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        _arrowLabel = new Label
        {
            Text = FluentIcons.chevron_down_12_regular,
            FontFamily = FluentIcons.FontFamily,
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Rotation = 90
        };

        _divider = new Border
        {
            WidthRequest = 1,
            Stroke = Colors.Transparent,
            BackgroundColor = Color.FromArgb("#40FFFFFF"),
            Margin = new Thickness(0, 6)
        };

        var mainTap = new TapGestureRecognizer();
        mainTap.Tapped += (_, _) => Clicked?.Invoke(this, EventArgs.Empty);

        var dropTap = new TapGestureRecognizer();
        dropTap.Tapped += (_, _) => DropdownClicked?.Invoke(this, EventArgs.Empty);

        var mainBorder = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(CornerRadius, 0, 0, CornerRadius)
            },
            Stroke = Colors.Transparent,
            Padding = new Thickness(16, 8),
            Content = _textLabel
        };
        mainBorder.GestureRecognizers.Add(mainTap);

        var dropBorder = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(0, CornerRadius, CornerRadius, 0)
            },
            Stroke = Colors.Transparent,
            Padding = new Thickness(8, 8),
            Content = _arrowLabel
        };
        dropBorder.GestureRecognizers.Add(dropTap);

        _layout = new HorizontalStackLayout
        {
            Spacing = 0,
            Children = { mainBorder, _divider, dropBorder }
        };

        _container = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(CornerRadius)
            },
            Stroke = Colors.Transparent,
            Content = _layout,
            HorizontalOptions = LayoutOptions.Start
        };

        Content = _container;
        ApplyVisualState();

        this.ObserveThemeChanged(ApplyVisualState);
    }

    private void UpdateContent()
    {
        _textLabel.Text = Text;
    }

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _container.BackgroundColor = theme.Accent;
        _container.Stroke = Colors.Transparent;
        _textLabel.TextColor = theme.TextOnAccent;
        _arrowLabel.TextColor = theme.TextOnAccent;
        _divider.BackgroundColor = Color.FromArgb("#40FFFFFF");

        _container.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(CornerRadius)
        };
    }
}

/// <summary>
/// Fluent Design dropdown button that opens a popup with options.
/// </summary>
public class FlDropDownButton : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlDropDownButton), string.Empty,
            propertyChanged: (b, _, _) => ((FlDropDownButton)b).UpdateContent());

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FlDropDownButton), FluentCornerRadius.Button,
            propertyChanged: (b, _, _) => ((FlDropDownButton)b).ApplyVisualState());

    public event EventHandler? Clicked;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private readonly Border _border;
    private readonly HorizontalStackLayout _content;
    private readonly Label _textLabel;
    private readonly Label _arrowLabel;

    public FlDropDownButton()
    {
        _textLabel = new Label
        {
            FontSize = 14,
            VerticalOptions = LayoutOptions.Center
        };

        _arrowLabel = new Label
        {
            Text = FluentIcons.chevron_down_12_regular,
            FontFamily = FluentIcons.FontFamily,
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            Rotation = 90,
            Margin = new Thickness(4, 0, 0, 0)
        };

        _content = new HorizontalStackLayout
        {
            Spacing = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _textLabel, _arrowLabel }
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
            Content = _content,
            HeightRequest = 40,
            HorizontalOptions = LayoutOptions.Start
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => Clicked?.Invoke(this, EventArgs.Empty);
        _border.GestureRecognizers.Add(tap);

        Content = _border;
        ApplyVisualState();

        this.ObserveThemeChanged(ApplyVisualState);
    }

    private void UpdateContent()
    {
        _textLabel.Text = Text;
    }

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _border.BackgroundColor = theme.Accent;
        _border.Stroke = Colors.Transparent;
        _textLabel.TextColor = theme.TextOnAccent;
        _arrowLabel.TextColor = theme.TextOnAccent;

        _border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(CornerRadius)
        };
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        FluentRippleEffect.Apply(_border);
    }
}
