using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Platform;
using PlatiniumUI.Fluent.DesignSystem.Icons;

namespace PlatiniumUI.Fluent.Controls.Inputs;

/// <summary>
/// Fluent Design text input layout with floating label, leading/trailing icons,
/// clear button, prefix/suffix text, and character counter.
/// Wraps and enhances a standard text input with additional visual elements.
/// </summary>
public class FlTextInputLayout : ContentView
{
    // ─── Bindable Properties ────────────────────────────────────────────────

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlTextInputLayout), string.Empty,
            BindingMode.TwoWay, propertyChanged: OnTextChanged);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FlTextInputLayout), string.Empty,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlTextInputLayout), string.Empty,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty LeadingIconProperty =
        BindableProperty.Create(nameof(LeadingIcon), typeof(string), typeof(FlTextInputLayout), null,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty TrailingIconProperty =
        BindableProperty.Create(nameof(TrailingIcon), typeof(string), typeof(FlTextInputLayout), null,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty PrefixTextProperty =
        BindableProperty.Create(nameof(PrefixText), typeof(string), typeof(FlTextInputLayout), null,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty SuffixTextProperty =
        BindableProperty.Create(nameof(SuffixText), typeof(string), typeof(FlTextInputLayout), null,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty ShowClearButtonProperty =
        BindableProperty.Create(nameof(ShowClearButton), typeof(bool), typeof(FlTextInputLayout), false,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FlTextInputLayout), false,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty MaxLengthProperty =
        BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(FlTextInputLayout), 0,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty HelperTextProperty =
        BindableProperty.Create(nameof(HelperText), typeof(string), typeof(FlTextInputLayout), null,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(FlTextInputLayout), null,
            propertyChanged: OnVisualPropertyChanged);

    // Note: Uses inherited IsEnabledProperty from VisualElement

    // ─── Events ─────────────────────────────────────────────────────────────

    public event EventHandler<string>? TextChanged;
    public event EventHandler? ClearClicked;
    public event EventHandler? TrailingIconClicked;

    // ─── Properties ─────────────────────────────────────────────────────────

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? LeadingIcon
    {
        get => (string?)GetValue(LeadingIconProperty);
        set => SetValue(LeadingIconProperty, value);
    }

    public string? TrailingIcon
    {
        get => (string?)GetValue(TrailingIconProperty);
        set => SetValue(TrailingIconProperty, value);
    }

    public string? PrefixText
    {
        get => (string?)GetValue(PrefixTextProperty);
        set => SetValue(PrefixTextProperty, value);
    }

    public string? SuffixText
    {
        get => (string?)GetValue(SuffixTextProperty);
        set => SetValue(SuffixTextProperty, value);
    }

    public bool ShowClearButton
    {
        get => (bool)GetValue(ShowClearButtonProperty);
        set => SetValue(ShowClearButtonProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public string? HelperText
    {
        get => (string?)GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    public string? ErrorText
    {
        get => (string?)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    public new bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    // ─── Private Fields ─────────────────────────────────────────────────────

    private readonly Grid _root;
    private readonly Label _floatingLabel;
    private readonly Label _leadingIconLabel;
    private readonly Label _trailingIconLabel;
    private readonly Label _prefixLabel;
    private readonly Label _suffixLabel;
    private readonly Label _clearButton;
    private readonly Label _helperLabel;
    private readonly Label _counterLabel;
    private readonly Entry _entry;
    private readonly Border _border;

    // ─── Constructor ────────────────────────────────────────────────────────

    public FlTextInputLayout()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _floatingLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = theme.TextSecondary,
            IsVisible = false,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(FluentSpacing.InputPaddingHorizontal, 0, FluentSpacing.InputPaddingHorizontal, 0)
        };

        _leadingIconLabel = new Label
        {
            FontFamily = FluentIcons.FontFamily,
            FontSize = 18,
            TextColor = theme.TextSecondary,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false,
            Margin = new Thickness(FluentSpacing.S, 0, 0, 0)
        };

        _trailingIconLabel = new Label
        {
            FontFamily = FluentIcons.FontFamily,
            FontSize = 18,
            TextColor = theme.TextSecondary,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false,
            Margin = new Thickness(0, 0, FluentSpacing.S, 0)
        };

        var trailingTap = new TapGestureRecognizer();
        trailingTap.Tapped += (_, _) => TrailingIconClicked?.Invoke(this, EventArgs.Empty);
        _trailingIconLabel.GestureRecognizers.Add(trailingTap);

        _prefixLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextTertiary,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _suffixLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextTertiary,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _clearButton = new Label
        {
            Text = FluentIcons.dismiss_12_regular,
            FontFamily = FluentIcons.FontFamily,
            FontSize = 12,
            TextColor = theme.TextSecondary,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false,
            Padding = new Thickness(4)
        };

        var clearTap = new TapGestureRecognizer();
        clearTap.Tapped += (_, _) =>
        {
            Text = string.Empty;
            ClearClicked?.Invoke(this, EventArgs.Empty);
        };
        _clearButton.GestureRecognizers.Add(clearTap);

        _entry = new Entry
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextPrimary,
            PlaceholderColor = theme.TextTertiary,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0)
        };
        FluentInputHelper.StripNativeDecorations(_entry);
        _entry.TextChanged += OnEntryTextChanged;
        _entry.Focused += OnEntryFocused;
        _entry.Unfocused += OnEntryUnfocused;

        _helperLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = theme.TextSecondary,
            IsVisible = false,
            Margin = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.XS, FluentSpacing.InputPaddingHorizontal, 0)
        };

        _counterLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = theme.TextSecondary,
            IsVisible = false,
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(0, FluentSpacing.XS, FluentSpacing.InputPaddingHorizontal, 0)
        };

        // Input row: leading icon + prefix + entry + suffix + clear + trailing icon
        var inputRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },  // leading icon
                new ColumnDefinition { Width = GridLength.Auto },  // prefix
                new ColumnDefinition { Width = GridLength.Star },  // entry
                new ColumnDefinition { Width = GridLength.Auto },  // suffix
                new ColumnDefinition { Width = GridLength.Auto },  // clear
                new ColumnDefinition { Width = GridLength.Auto }   // trailing icon
            },
            ColumnSpacing = FluentSpacing.XS
        };
        inputRow.Children.Add(_leadingIconLabel);
        Grid.SetColumn(_leadingIconLabel, 0);
        inputRow.Children.Add(_prefixLabel);
        Grid.SetColumn(_prefixLabel, 1);
        inputRow.Children.Add(_entry);
        Grid.SetColumn(_entry, 2);
        inputRow.Children.Add(_suffixLabel);
        Grid.SetColumn(_suffixLabel, 3);
        inputRow.Children.Add(_clearButton);
        Grid.SetColumn(_clearButton, 4);
        inputRow.Children.Add(_trailingIconLabel);
        Grid.SetColumn(_trailingIconLabel, 5);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Input)
            },
            Stroke = new SolidColorBrush(theme.Border),
            StrokeThickness = 1,
            BackgroundColor = theme.Surface,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            Content = inputRow
        };

        // Bottom row: helper text + counter
        var bottomRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            Children = { _helperLabel, _counterLabel }
        };
        Grid.SetColumn(_counterLabel, 1);

        _root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },  // floating label
                new RowDefinition { Height = GridLength.Auto },  // border + input
                new RowDefinition { Height = GridLength.Auto }   // helper + counter
            },
            RowSpacing = 0
        };
        _root.Children.Add(_floatingLabel);
        Grid.SetRow(_floatingLabel, 0);
        _root.Children.Add(_border);
        Grid.SetRow(_border, 1);
        _root.Children.Add(bottomRow);
        Grid.SetRow(bottomRow, 2);

        Content = _root;

        this.ObserveThemeChanged(ApplyTheme);
        ApplyVisualState();
    }

    // ─── Property Changed Handlers ──────────────────────────────────────────

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var layout = (FlTextInputLayout)bindable;
        layout._entry.Text = (string)newValue;
        layout.UpdateClearButton();
        layout.UpdateCounter();
        layout.TextChanged?.Invoke(layout, (string)newValue);
    }

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((FlTextInputLayout)bindable).ApplyVisualState();
    }

    // ─── Entry Event Handlers ───────────────────────────────────────────────

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_entry.Text != Text)
        {
            Text = _entry.Text;
        }
    }

    private void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _border.Stroke = new SolidColorBrush(theme.Accent);
        _border.StrokeThickness = 2;

        if (string.IsNullOrEmpty(Text))
        {
            _floatingLabel.IsVisible = true;
            _floatingLabel.TextColor = theme.Accent;
        }
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        if (!string.IsNullOrEmpty(ErrorText))
        {
            _border.Stroke = new SolidColorBrush(theme.Error);
        }
        else
        {
            _border.Stroke = new SolidColorBrush(theme.Border);
        }
        _border.StrokeThickness = 1;

        if (string.IsNullOrEmpty(Text))
        {
            _floatingLabel.IsVisible = false;
        }

        _floatingLabel.TextColor = theme.TextSecondary;
    }

    // ─── Visual State ───────────────────────────────────────────────────────

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        // Floating label
        _floatingLabel.Text = Label;
        _entry.Placeholder = string.IsNullOrEmpty(Label) ? Placeholder : (string.IsNullOrEmpty(Text) ? Placeholder : string.Empty);

        if (!string.IsNullOrEmpty(Text) || _entry.IsFocused)
        {
            _floatingLabel.IsVisible = true;
        }
        else
        {
            _floatingLabel.IsVisible = false;
        }

        // Leading icon
        if (!string.IsNullOrEmpty(LeadingIcon))
        {
            _leadingIconLabel.Text = LeadingIcon;
            _leadingIconLabel.IsVisible = true;
        }
        else
        {
            _leadingIconLabel.IsVisible = false;
        }

        // Trailing icon
        if (!string.IsNullOrEmpty(TrailingIcon))
        {
            _trailingIconLabel.Text = TrailingIcon;
            _trailingIconLabel.IsVisible = true;
        }
        else
        {
            _trailingIconLabel.IsVisible = false;
        }

        // Prefix/Suffix
        if (!string.IsNullOrEmpty(PrefixText))
        {
            _prefixLabel.Text = PrefixText;
            _prefixLabel.IsVisible = true;
        }
        else
        {
            _prefixLabel.IsVisible = false;
        }

        if (!string.IsNullOrEmpty(SuffixText))
        {
            _suffixLabel.Text = SuffixText;
            _suffixLabel.IsVisible = true;
        }
        else
        {
            _suffixLabel.IsVisible = false;
        }

        // Password
        _entry.IsPassword = IsPassword;

        // Max length
        _entry.MaxLength = MaxLength > 0 ? MaxLength : int.MaxValue;

        // Helper/Error text
        if (!string.IsNullOrEmpty(ErrorText))
        {
            _helperLabel.Text = ErrorText;
            _helperLabel.TextColor = theme.Error;
            _helperLabel.IsVisible = true;
            _border.Stroke = new SolidColorBrush(theme.Error);
        }
        else if (!string.IsNullOrEmpty(HelperText))
        {
            _helperLabel.Text = HelperText;
            _helperLabel.TextColor = theme.TextSecondary;
            _helperLabel.IsVisible = true;
            _border.Stroke = new SolidColorBrush(theme.Border);
        }
        else
        {
            _helperLabel.IsVisible = false;
        }

        // Counter
        UpdateCounter();

        // Clear button
        UpdateClearButton();

        // Enabled state
        _entry.IsEnabled = IsEnabled;
        _border.Opacity = IsEnabled ? 1.0 : 0.5;
    }

    private void UpdateClearButton()
    {
        _clearButton.IsVisible = ShowClearButton && !string.IsNullOrEmpty(Text) && IsEnabled;
    }

    private void UpdateCounter()
    {
        if (MaxLength > 0)
        {
            var theme = FluentThemeManager.Current.CurrentTheme;
            var length = Text?.Length ?? 0;
            _counterLabel.Text = $"{length}/{MaxLength}";
            _counterLabel.TextColor = length >= MaxLength ? theme.Error : theme.TextSecondary;
            _counterLabel.IsVisible = true;
        }
        else
        {
            _counterLabel.IsVisible = false;
        }
    }

    private void ApplyTheme()
    {
        ApplyVisualState();
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsEnabled))
        {
            ApplyVisualState();
        }
    }
}
