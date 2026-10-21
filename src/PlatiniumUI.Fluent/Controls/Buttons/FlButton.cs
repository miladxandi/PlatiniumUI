using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;
using PlatiniumUI.Fluent.DesignSystem.Icons;

namespace PlatiniumUI.Fluent.Controls.Buttons;

/// <summary>
/// Fluent Design button control with multiple variants, sizes, and states.
/// Supports Primary, Secondary, Subtle, Danger, and Icon-only modes.
/// </summary>
public class FlButton : ContentView
{
    // ─── Bindable Properties ────────────────────────────────────────────────

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlButton), string.Empty,
            propertyChanged: OnTextChanged);

    public static readonly BindableProperty VariantProperty =
        BindableProperty.Create(nameof(Variant), typeof(FlButtonVariant), typeof(FlButton), FlButtonVariant.Primary,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(FlButtonSize), typeof(FlButton), FlButtonSize.Medium,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(FlButton), null,
            propertyChanged: OnTextChanged);

    public static readonly BindableProperty IconPositionProperty =
        BindableProperty.Create(nameof(IconPosition), typeof(FlButtonIconPosition), typeof(FlButton), FlButtonIconPosition.Left,
            propertyChanged: OnTextChanged);

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(FlButton), false,
            propertyChanged: OnIsLoadingChanged);

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FlButton), FluentCornerRadius.Button,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty IsFullWidthProperty =
        BindableProperty.Create(nameof(IsFullWidth), typeof(bool), typeof(FlButton), false,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty EnableRippleProperty =
        BindableProperty.Create(nameof(EnableRipple), typeof(bool), typeof(FlButton), true,
            propertyChanged: OnRipplePropertyChanged);

    public static readonly BindableProperty RippleColorProperty =
        BindableProperty.Create(nameof(RippleColor), typeof(Color), typeof(FlButton), null,
            propertyChanged: OnRipplePropertyChanged);

    // ─── Events ─────────────────────────────────────────────────────────────

    public event EventHandler? Clicked;

    // ─── Properties ─────────────────────────────────────────────────────────

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public FlButtonVariant Variant
    {
        get => (FlButtonVariant)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public FlButtonSize Size
    {
        get => (FlButtonSize)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public FlButtonIconPosition IconPosition
    {
        get => (FlButtonIconPosition)GetValue(IconPositionProperty);
        set => SetValue(IconPositionProperty, value);
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public bool IsFullWidth
    {
        get => (bool)GetValue(IsFullWidthProperty);
        set => SetValue(IsFullWidthProperty, value);
    }

    public bool EnableRipple
    {
        get => (bool)GetValue(EnableRippleProperty);
        set => SetValue(EnableRippleProperty, value);
    }

    public Color? RippleColor
    {
        get => (Color?)GetValue(RippleColorProperty);
        set => SetValue(RippleColorProperty, value);
    }

    // ─── Private Fields ─────────────────────────────────────────────────────

    private readonly Border _border;
    private readonly HorizontalStackLayout _content;
    private readonly Label _iconLabel;
    private readonly Label _textLabel;
    private readonly ActivityIndicator _spinner;
    private readonly TapGestureRecognizer _tapGesture;

    // ─── Constructor ────────────────────────────────────────────────────────

    public FlButton()
    {
        _iconLabel = new Label
        {
            FontFamily = FluentIcons.FontFamily,
            FontSize = 16,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        _textLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        _spinner = new ActivityIndicator
        {
            IsRunning = false,
            IsVisible = false,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Color = Colors.White
        };

        _content = new HorizontalStackLayout
        {
            Spacing = 8,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _iconLabel, _textLabel }
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
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center
        };

        _tapGesture = new TapGestureRecognizer();
        _tapGesture.Tapped += OnTapped;
        _border.GestureRecognizers.Add(_tapGesture);

        Content = _border;

        ApplyTheme();
        ApplySize();
        UpdateContent();

        this.ObserveThemeChanged(() =>
        {
            ApplyTheme();
            UpdateContent();
        });
    }

    // ─── Property Changed Handlers ──────────────────────────────────────────

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((FlButton)bindable).UpdateContent();
    }

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var button = (FlButton)bindable;
        button.ApplyTheme();
        button.ApplySize();
        button.UpdateContent();
    }

    private static void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var button = (FlButton)bindable;
        button.UpdateLoadingState();
    }

    private static void OnRipplePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        // Ripple is applied in OnHandlerChanged, so we don't need to do anything here
        // The ripple will be applied when the handler changes
    }

    // ─── Theme ──────────────────────────────────────────────────────────────

    private void ApplyTheme()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        switch (Variant)
        {
            case FlButtonVariant.Primary:
                _border.BackgroundColor = theme.Accent;
                _border.Stroke = Colors.Transparent;
                _textLabel.TextColor = theme.TextOnAccent;
                _iconLabel.TextColor = theme.TextOnAccent;
                _spinner.Color = theme.TextOnAccent;
                break;

            case FlButtonVariant.Secondary:
                _border.BackgroundColor = Colors.Transparent;
                _border.Stroke = theme.Accent;
                _border.StrokeThickness = 1;
                _textLabel.TextColor = theme.Accent;
                _iconLabel.TextColor = theme.Accent;
                _spinner.Color = theme.Accent;
                break;

            case FlButtonVariant.Subtle:
                _border.BackgroundColor = Colors.Transparent;
                _border.Stroke = Colors.Transparent;
                _textLabel.TextColor = theme.Accent;
                _iconLabel.TextColor = theme.Accent;
                _spinner.Color = theme.Accent;
                break;

            case FlButtonVariant.Danger:
                _border.BackgroundColor = theme.Error;
                _border.Stroke = Colors.Transparent;
                _textLabel.TextColor = Colors.White;
                _iconLabel.TextColor = Colors.White;
                _spinner.Color = Colors.White;
                break;

            case FlButtonVariant.Icon:
                _border.BackgroundColor = Colors.Transparent;
                _border.Stroke = Colors.Transparent;
                _iconLabel.TextColor = theme.TextPrimary;
                _textLabel.TextColor = theme.TextPrimary;
                _spinner.Color = theme.TextPrimary;
                break;
        }

        if (!IsEnabled)
        {
            ApplyDisabledState();
        }
    }

    // ─── Size ───────────────────────────────────────────────────────────────

    private void ApplySize()
    {
        var (paddingH, paddingV, fontSize, iconSize, height) = Size switch
        {
            FlButtonSize.Small => (12.0, 6.0, 12.0, 14.0, 32.0),
            FlButtonSize.Medium => (16.0, 8.0, 14.0, 16.0, 40.0),
            FlButtonSize.Large => (20.0, 10.0, 16.0, 20.0, 48.0),
            _ => (16.0, 8.0, 14.0, 16.0, 40.0)
        };

        _border.Padding = new Thickness(paddingH, paddingV);
        _textLabel.FontSize = fontSize;
        _iconLabel.FontSize = iconSize;
        _spinner.HeightRequest = height * 0.5;
        _spinner.WidthRequest = height * 0.5;

        if (Variant == FlButtonVariant.Icon)
        {
            _border.WidthRequest = height;
            _border.HeightRequest = height;
            _border.HorizontalOptions = LayoutOptions.Start;
        }
        else
        {
            _border.HeightRequest = height;
            _border.HorizontalOptions = IsFullWidth ? LayoutOptions.Fill : LayoutOptions.Start;
        }

        _border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(CornerRadius)
        };
    }

    // ─── Content ────────────────────────────────────────────────────────────

    private void UpdateContent()
    {
        bool hasIcon = !string.IsNullOrEmpty(Icon);
        bool hasText = !string.IsNullOrEmpty(Text);

        _iconLabel.IsVisible = hasIcon && Variant != FlButtonVariant.Icon ? true : Variant == FlButtonVariant.Icon;
        _textLabel.IsVisible = hasText && Variant != FlButtonVariant.Icon;

        if (hasIcon)
        {
            _iconLabel.Text = Icon;
        }

        _textLabel.Text = Text;

        // Reorder content based on icon position
        _content.Children.Clear();
        if (IconPosition == FlButtonIconPosition.Left)
        {
            _content.Children.Add(_iconLabel);
            _content.Children.Add(_textLabel);
        }
        else
        {
            _content.Children.Add(_textLabel);
            _content.Children.Add(_iconLabel);
        }

        // For icon-only variant, only show icon
        if (Variant == FlButtonVariant.Icon)
        {
            _content.Children.Clear();
            _content.Children.Add(_iconLabel);
        }
    }

    // ─── Loading State ──────────────────────────────────────────────────────

    private void UpdateLoadingState()
    {
        _spinner.IsVisible = IsLoading;
        _spinner.IsRunning = IsLoading;
        _content.IsVisible = !IsLoading;
        _border.InputTransparent = IsLoading;

        if (IsLoading)
        {
            _border.Opacity = 0.7;
        }
        else
        {
            _border.Opacity = 1.0;
        }
    }

    // ─── Interaction ────────────────────────────────────────────────────────

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled || IsLoading) return;
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        // Apply native ripple effect on Android (and platform-specific effects on other platforms)
        if (EnableRipple && Handler?.PlatformView != null)
        {
            var config = new DesignSystem.Platform.RippleConfig
            {
                Color = RippleColor,
                Radius = CornerRadius,
                Enabled = EnableRipple
            };
            DesignSystem.Platform.FluentRippleEffect.Apply(_border, config);
        }
    }

    private void ApplyHoverState(bool isHovered)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        if (isHovered)
        {
            switch (Variant)
            {
                case FlButtonVariant.Primary:
                    _border.BackgroundColor = theme.AccentHover;
                    break;
                case FlButtonVariant.Secondary:
                    _border.BackgroundColor = theme.AccentLight;
                    break;
                case FlButtonVariant.Subtle:
                    _border.BackgroundColor = theme.HoverOverlay;
                    break;
                case FlButtonVariant.Danger:
                    _border.BackgroundColor = Color.FromArgb("#B42A2E");
                    break;
                case FlButtonVariant.Icon:
                    _border.BackgroundColor = theme.HoverOverlay;
                    break;
            }
        }
        else
        {
            ApplyTheme();
        }
    }

    private void ApplyDisabledState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _border.BackgroundColor = theme.IsDark
            ? Color.FromArgb("#3D3D3D")
            : Color.FromArgb("#E0E0E0");
        _border.Stroke = Colors.Transparent;
        _textLabel.TextColor = theme.TextDisabled;
        _iconLabel.TextColor = theme.TextDisabled;
        _spinner.Color = theme.TextDisabled;
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsEnabled))
        {
            if (IsEnabled)
            {
                ApplyTheme();
            }
            else
            {
                ApplyDisabledState();
            }
        }
    }
}

// ─── Enums ──────────────────────────────────────────────────────────────────

public enum FlButtonVariant
{
    Primary,
    Secondary,
    Subtle,
    Danger,
    Icon
}

public enum FlButtonSize
{
    Small,
    Medium,
    Large
}

public enum FlButtonIconPosition
{
    Left,
    Right
}
