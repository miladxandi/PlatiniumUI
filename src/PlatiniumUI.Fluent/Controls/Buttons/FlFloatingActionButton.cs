using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;
using PlatiniumUI.Fluent.DesignSystem.Platform;
using PlatiniumUI.Fluent.DesignSystem.Icons;

namespace PlatiniumUI.Fluent.Controls.Buttons;

/// <summary>
/// Fluent Design Floating Action Button (FAB).
/// Material-style elevated button with icon and optional text for extended FAB.
/// Supports Mini, Normal, and Extended sizes with ripple integration.
/// </summary>
public class FlFloatingActionButton : ContentView
{
    // ─── Bindable Properties ────────────────────────────────────────────────

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(FlFloatingActionButton), null,
            propertyChanged: (b, _, _) => ((FlFloatingActionButton)b).UpdateContent());

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlFloatingActionButton), string.Empty,
            propertyChanged: (b, _, _) => ((FlFloatingActionButton)b).UpdateContent());

    public static readonly BindableProperty FABSizeProperty =
        BindableProperty.Create(nameof(FABSize), typeof(FlFABSize), typeof(FlFloatingActionButton), FlFABSize.Normal,
            propertyChanged: (b, _, _) => ((FlFloatingActionButton)b).ApplySize());

    public static readonly BindableProperty BackgroundColorOverrideProperty =
        BindableProperty.Create(nameof(BackgroundColorOverride), typeof(Color), typeof(FlFloatingActionButton), null,
            propertyChanged: (b, _, _) => ((FlFloatingActionButton)b).ApplyTheme());

    public static readonly BindableProperty IconColorProperty =
        BindableProperty.Create(nameof(IconColor), typeof(Color), typeof(FlFloatingActionButton), null,
            propertyChanged: (b, _, _) => ((FlFloatingActionButton)b).ApplyTheme());

    public static readonly BindableProperty EnableRippleProperty =
        BindableProperty.Create(nameof(EnableRipple), typeof(bool), typeof(FlFloatingActionButton), true);

    // ─── Events ─────────────────────────────────────────────────────────────

    public event EventHandler? Clicked;

    // ─── Properties ─────────────────────────────────────────────────────────

    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public FlFABSize FABSize
    {
        get => (FlFABSize)GetValue(FABSizeProperty);
        set => SetValue(FABSizeProperty, value);
    }

    public Color? BackgroundColorOverride
    {
        get => (Color?)GetValue(BackgroundColorOverrideProperty);
        set => SetValue(BackgroundColorOverrideProperty, value);
    }

    public Color? IconColor
    {
        get => (Color?)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public bool EnableRipple
    {
        get => (bool)GetValue(EnableRippleProperty);
        set => SetValue(EnableRippleProperty, value);
    }

    // ─── Private Fields ─────────────────────────────────────────────────────

    private readonly Border _border;
    private readonly Label _iconLabel;
    private readonly Label _textLabel;
    private readonly HorizontalStackLayout _content;
    private readonly TapGestureRecognizer _tapGesture;

    // ─── Constructor ────────────────────────────────────────────────────────

    public FlFloatingActionButton()
    {
        _iconLabel = new Label
        {
            FontFamily = FluentIcons.FontFamily,
            FontSize = 24,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        _textLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _content = new HorizontalStackLayout
        {
            Spacing = FluentSpacing.S,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _iconLabel, _textLabel }
        };

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Circular)
            },
            Stroke = Colors.Transparent,
            Content = _content,
            HorizontalOptions = LayoutOptions.Center,
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
            ApplySize();
        });
    }

    // ─── Theme ──────────────────────────────────────────────────────────────

    private void ApplyTheme()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _border.BackgroundColor = BackgroundColorOverride ?? theme.Accent;
        _iconLabel.TextColor = IconColor ?? theme.TextOnAccent;
        _textLabel.TextColor = IconColor ?? theme.TextOnAccent;

        // Apply FAB elevation shadow
        _border.Shadow = FluentElevation.FAB;

        if (!IsEnabled)
        {
            ApplyDisabledState();
        }
    }

    // ─── Size ───────────────────────────────────────────────────────────────

    private void ApplySize()
    {
        bool isExtended = FABSize == FlFABSize.Extended || !string.IsNullOrEmpty(Text);

        var (size, iconSize, fontSize) = FABSize switch
        {
            FlFABSize.Mini => (40.0, 20.0, FluentTypography.BodySmallSize),
            FlFABSize.Normal => (56.0, 24.0, FluentTypography.BodySize),
            FlFABSize.Extended => (48.0, 24.0, FluentTypography.BodySize),
            _ => (56.0, 24.0, FluentTypography.BodySize)
        };

        _iconLabel.FontSize = iconSize;
        _textLabel.FontSize = fontSize;

        if (isExtended)
        {
            // Extended FAB: pill shape with icon + text
            _border.HeightRequest = size;
            _border.WidthRequest = -1; // Auto width
            _border.HorizontalOptions = LayoutOptions.Start;
            _border.Padding = new Thickness(FluentSpacing.L, FluentSpacing.S, FluentSpacing.L, FluentSpacing.S);

            _border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(size / 2)
            };
        }
        else
        {
            // Standard/Mini FAB: circular
            _border.WidthRequest = size;
            _border.HeightRequest = size;
            _border.HorizontalOptions = LayoutOptions.Center;
            _border.Padding = new Thickness(0);

            _border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(size / 2)
            };
        }
    }

    // ─── Content ────────────────────────────────────────────────────────────

    private void UpdateContent()
    {
        bool hasIcon = !string.IsNullOrEmpty(Icon);
        bool hasText = !string.IsNullOrEmpty(Text);
        bool isExtended = FABSize == FlFABSize.Extended || hasText;

        _iconLabel.IsVisible = hasIcon;
        _textLabel.IsVisible = isExtended && hasText;

        if (hasIcon)
        {
            _iconLabel.Text = Icon;
        }

        _textLabel.Text = Text;

        // Update layout for extended vs standard
        _content.Children.Clear();
        if (hasIcon)
        {
            _content.Children.Add(_iconLabel);
        }
        if (isExtended && hasText)
        {
            _content.Children.Add(_textLabel);
        }
    }

    // ─── Interaction ────────────────────────────────────────────────────────

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled) return;
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        // Apply native ripple effect
        if (EnableRipple && Handler?.PlatformView != null)
        {
            double cornerRadius = FABSize switch
            {
                FlFABSize.Mini => 20,
                FlFABSize.Normal => 28,
                FlFABSize.Extended => 24,
                _ => 28
            };

            var config = new RippleConfig
            {
                Radius = cornerRadius,
                Enabled = EnableRipple
            };
            FluentRippleEffect.Apply(_border, config);
        }
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

    private void ApplyDisabledState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _border.BackgroundColor = theme.IsDark
            ? Color.FromArgb("#3D3D3D")
            : Color.FromArgb("#E0E0E0");
        _iconLabel.TextColor = theme.TextDisabled;
        _textLabel.TextColor = theme.TextDisabled;
        _border.Shadow = FluentElevation.Level0;
    }
}

// ─── Enums ──────────────────────────────────────────────────────────────────

/// <summary>
/// Size variants for Floating Action Button.
/// </summary>
public enum FlFABSize
{
    /// <summary>Small FAB (40x40)</summary>
    Mini,
    /// <summary>Standard FAB (56x56)</summary>
    Normal,
    /// <summary>Extended FAB with text label</summary>
    Extended
}
