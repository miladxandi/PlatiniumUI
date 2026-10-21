using System.Windows.Input;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;
using PlatiniumUI.Fluent.DesignSystem.Platform;

namespace PlatiniumUI.Fluent.Controls.Inputs;

/// <summary>
/// Fluent Design text input with floating label, clean outlined border (no underscores),
/// helper text, error state, and character counter.
/// </summary>
public class FlTextInput : ContentView
{
    // ─── Bindable Properties ────────────────────────────────────────────────

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlTextInput), string.Empty,
            BindingMode.TwoWay, propertyChanged: (b, _, _) => ((FlTextInput)b).OnTextChanged());

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FlTextInput), string.Empty,
            propertyChanged: (b, _, _) => ((FlTextInput)b).UpdateLabels());

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlTextInput), string.Empty,
            propertyChanged: (b, _, _) => ((FlTextInput)b).UpdateLabels());

    public static readonly BindableProperty HelperTextProperty =
        BindableProperty.Create(nameof(HelperText), typeof(string), typeof(FlTextInput), null,
            propertyChanged: (b, _, _) => ((FlTextInput)b).UpdateHelperText());

    public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(FlTextInput), null,
            propertyChanged: (b, _, _) => ((FlTextInput)b).UpdateErrorState());

    public static readonly BindableProperty HasErrorProperty =
        BindableProperty.Create(nameof(HasError), typeof(bool), typeof(FlTextInput), false,
            propertyChanged: (b, _, _) => ((FlTextInput)b).UpdateErrorState());

    public static readonly BindableProperty MaxLengthProperty =
        BindableProperty.Create(nameof(MaxLength), typeof(int), typeof(FlTextInput), 0,
            propertyChanged: (b, _, _) => ((FlTextInput)b).UpdateCounter());

    public static readonly BindableProperty IsReadOnlyProperty =
        BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(FlTextInput), false);

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FlTextInput), Keyboard.Default,
            propertyChanged: (b, _, _) => ((FlTextInput)b).UpdateKeyboard());

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FlTextInput), FluentCornerRadius.Input,
            propertyChanged: (b, _, _) => ((FlTextInput)b).ApplyVisualState());

    public static readonly BindableProperty ReturnTypeProperty =
        BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(FlTextInput), ReturnType.Default,
            propertyChanged: (b, _, _) => ((FlTextInput)b).ApplyVisualState());

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FlTextInput), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FlTextInput), null);

    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(FlTextInput), null,
            propertyChanged: (b, _, _) => ((FlTextInput)b).ApplyVisualState());

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(FlTextInput), FluentTypography.BodySize,
            propertyChanged: (b, _, _) => ((FlTextInput)b).ApplyVisualState());

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

    public bool HasError
    {
        get => (bool)GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>Keyboard return key (e.g. <c>Go</c>/<c>Done</c>) — enables login/submit via keyboard.</summary>
    public ReturnType ReturnType
    {
        get => (ReturnType)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }

    /// <summary>Command invoked when the keyboard return key is pressed.</summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>Explicit font family passed through to the inner <see cref="Entry"/> (e.g. VazirmatnRegular).</summary>
    public string? FontFamily
    {
        get => (string?)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    // ─── Events ─────────────────────────────────────────────────────────────

    public event EventHandler? TextChanged;
    public event EventHandler? Completed;

    // ─── Private Fields ─────────────────────────────────────────────────────

    private readonly Grid _rootGrid;
    private readonly Border _border;
    private readonly Entry _entry;
    private readonly Label _floatingLabel;
    private readonly Label _helperLabel;
    private readonly Label _counterLabel;
    private readonly VerticalStackLayout _mainLayout;
    private readonly HorizontalStackLayout _bottomRow;
    private bool _isFocused;
    private bool _suppressSync;   // re-entrancy guard so Text<->Entry never loops / jumps the caret
    private EventHandler<FluentThemeChangedEventArgs>? _themeHandler;

    // ─── Constructor ────────────────────────────────────────────────────────

    public FlTextInput()
    {
        _entry = new Entry
        {
            BackgroundColor = Colors.Transparent,
            TextColor = FluentThemeManager.Current.CurrentTheme.TextPrimary,
            PlaceholderColor = FluentThemeManager.Current.CurrentTheme.TextTertiary,
            FontSize = FluentTypography.BodySize,
            Margin = new Thickness(0, 8, 0, 0)
        };
        FluentInputHelper.StripNativeDecorations(_entry);

        _floatingLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = FluentThemeManager.Current.CurrentTheme.TextSecondary,
            InputTransparent = true
        };

        _helperLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = FluentThemeManager.Current.CurrentTheme.TextTertiary,
            IsVisible = false,
            Margin = new Thickness(0, 4, 0, 0)
        };

        _counterLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = FluentThemeManager.Current.CurrentTheme.TextTertiary,
            HorizontalOptions = LayoutOptions.End,
            IsVisible = false,
            Margin = new Thickness(0, 4, 0, 0)
        };

        var inputArea = new Grid();
        inputArea.Children.Add(_entry);
        inputArea.Children.Add(_floatingLabel);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Input)
            },
            Stroke = FluentThemeManager.Current.CurrentTheme.Border,
            StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = FluentThemeManager.Current.CurrentTheme.Surface,
            Content = inputArea
        };

        _bottomRow = new HorizontalStackLayout
        {
            Spacing = 8,
            Children = { _helperLabel, _counterLabel }
        };

        _mainLayout = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { _border, _bottomRow }
        };

        _rootGrid = new Grid { Children = { _mainLayout } };
        Content = _rootGrid;

        // Events
        _entry.Focused += OnEntryFocused;
        _entry.Unfocused += OnEntryUnfocused;
        _entry.TextChanged += OnEntryTextChanged;
        _entry.Completed += OnEntryCompleted;

        ApplyVisualState();
        UpdateLabels();

        // Subscribe on attach, unsubscribe on detach — no singleton leak across ~44 pages.
        Loaded += OnAttached;
        Unloaded += OnDetached;
    }

    private void OnAttached(object? sender, EventArgs e)
    {
        _themeHandler = (_, _) => Dispatcher.Dispatch(ApplyAll);
        FluentThemeManager.Current.ThemeChanged += _themeHandler;
        ApplyAll();
    }

    private void OnDetached(object? sender, EventArgs e)
    {
        if (_themeHandler is not null) FluentThemeManager.Current.ThemeChanged -= _themeHandler;
        _themeHandler = null;
    }

    private void ApplyAll()
    {
        ApplyVisualState();
        UpdateLabels();
        UpdateHelperText();
        UpdateErrorState();
    }

    // ─── Focus Handling ─────────────────────────────────────────────────────

    private void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        _isFocused = true;
        ApplyVisualState();
        AnimateLabel(true);
    }

    private void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        _isFocused = false;
        ApplyVisualState();

        if (string.IsNullOrEmpty(_entry.Text))
        {
            AnimateLabel(false);
        }
    }

    // ─── Text Handling ───────────────────────────────────────────────────────

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_suppressSync) return;
        var newText = e.NewTextValue ?? string.Empty;
        if (Text != newText)
        {
            _suppressSync = true;
            Text = newText;
            _suppressSync = false;
        }
        UpdateCounter();
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnEntryCompleted(object? sender, EventArgs e)
    {
        Completed?.Invoke(this, EventArgs.Empty);
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    private void OnTextChanged()
    {
        if (!_suppressSync && _entry.Text != Text)
        {
            _suppressSync = true;
            _entry.Text = Text;
            _suppressSync = false;
        }

        bool hasText = !string.IsNullOrEmpty(Text);
        if (hasText && !_isFocused)
        {
            AnimateLabel(true);
        }
        else if (!hasText && !_isFocused)
        {
            AnimateLabel(false);
        }
    }

    // ─── Label Animation ────────────────────────────────────────────────────

    private void AnimateLabel(bool isFloating)
    {
        if (isFloating)
        {
            _floatingLabel.TranslateToAsync(0, -22, FluentAnimations.Fast, FluentAnimations.Entrance);
            _floatingLabel.FontSize = FluentTypography.CaptionSize;

            if (_isFocused)
            {
                _floatingLabel.TextColor = HasError
                    ? FluentThemeManager.Current.CurrentTheme.Error
                    : FluentThemeManager.Current.CurrentTheme.Accent;
            }
        }
        else
        {
            _floatingLabel.TranslateToAsync(0, 0, FluentAnimations.Fast, FluentAnimations.Entrance);
            _floatingLabel.FontSize = FluentTypography.BodySize;
            _floatingLabel.TextColor = FluentThemeManager.Current.CurrentTheme.TextTertiary;
        }
    }

    // ─── Visual State ───────────────────────────────────────────────────────

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(CornerRadius)
        };

        if (HasError)
        {
            _border.Stroke = theme.Error;
            _border.StrokeThickness = 2;
        }
        else if (_isFocused)
        {
            _border.Stroke = theme.Accent;
            _border.StrokeThickness = 2;
        }
        else
        {
            _border.Stroke = theme.Border;
            _border.StrokeThickness = 1;
        }

        _border.BackgroundColor = theme.Surface;
        _entry.TextColor = theme.TextPrimary;
        _entry.PlaceholderColor = theme.TextTertiary;
        _entry.IsReadOnly = IsReadOnly;
        _entry.Keyboard = Keyboard;
        _entry.ReturnType = ReturnType;
        _entry.FontSize = FontSize;
        if (!string.IsNullOrEmpty(FontFamily)) _entry.FontFamily = FontFamily;
        // Explicit RTL pass-through so the caret aligns right even if a parent override is missed.
        _entry.FlowDirection = FlowDirection;
    }

    // ─── Update Methods ─────────────────────────────────────────────────────

    private void UpdateLabels()
    {
        _floatingLabel.Text = !string.IsNullOrEmpty(Label) ? Label : Placeholder;
        _entry.Placeholder = string.IsNullOrEmpty(Label) ? Placeholder : string.Empty;

        if (!string.IsNullOrEmpty(Text))
        {
            AnimateLabel(true);
        }
    }

    private void UpdateHelperText()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        bool hasHelper = !string.IsNullOrEmpty(HelperText);
        _helperLabel.IsVisible = hasHelper && !HasError;
        _helperLabel.Text = HelperText ?? string.Empty;
        _helperLabel.TextColor = HasError ? theme.Error : theme.TextTertiary;
    }

    private void UpdateErrorState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        if (HasError)
        {
            _helperLabel.IsVisible = true;
            _helperLabel.Text = ErrorText ?? string.Empty;
            _helperLabel.TextColor = theme.Error;
        }
        else
        {
            UpdateHelperText();
        }

        ApplyVisualState();

        if (_isFocused)
        {
            _floatingLabel.TextColor = HasError ? theme.Error : theme.Accent;
        }
    }

    private void UpdateCounter()
    {
        if (MaxLength > 0)
        {
            _counterLabel.IsVisible = true;
            _counterLabel.Text = $"{(Text?.Length ?? 0)}/{MaxLength}";
        }
        else
        {
            _counterLabel.IsVisible = false;
        }
    }

    private void UpdateKeyboard()
    {
        _entry.Keyboard = Keyboard;
    }

    // ─── Public Methods ─────────────────────────────────────────────────────

    public new void Focus() => _entry.Focus();
    public new void Unfocus() => _entry.Unfocus();
}
