using System.Windows.Input;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;
using PlatiniumUI.Fluent.DesignSystem.Platform;

namespace PlatiniumUI.Fluent.Controls.Inputs;

/// <summary>
/// Fluent Design password input with reveal toggle button.
/// </summary>
public class FlPasswordInput : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlPasswordInput), string.Empty, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlPasswordInput)b).OnTextChanged());

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FlPasswordInput), "Password",
            propertyChanged: (b, _, _) => ((FlPasswordInput)b).UpdateLabels());

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlPasswordInput), string.Empty,
            propertyChanged: (b, _, _) => ((FlPasswordInput)b).UpdateLabels());

    public static readonly BindableProperty HasErrorProperty =
        BindableProperty.Create(nameof(HasError), typeof(bool), typeof(FlPasswordInput), false,
            propertyChanged: (b, _, _) => ((FlPasswordInput)b).ApplyVisualState());

    public static readonly BindableProperty ReturnTypeProperty =
        BindableProperty.Create(nameof(ReturnType), typeof(ReturnType), typeof(FlPasswordInput), ReturnType.Done,
            propertyChanged: (b, _, _) => ((FlPasswordInput)b).ApplyVisualState());

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FlPasswordInput), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FlPasswordInput), null);

    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(FlPasswordInput), null,
            propertyChanged: (b, _, _) => ((FlPasswordInput)b).ApplyVisualState());

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public bool HasError { get => (bool)GetValue(HasErrorProperty); set => SetValue(HasErrorProperty, value); }
    /// <summary>Keyboard return key; with <see cref="Command"/> this submits login from the password field.</summary>
    public ReturnType ReturnType { get => (ReturnType)GetValue(ReturnTypeProperty); set => SetValue(ReturnTypeProperty, value); }
    /// <summary>Command invoked when the keyboard return key is pressed.</summary>
    public ICommand? Command { get => (ICommand?)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
    public object? CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }
    /// <summary>Explicit font family passed through to the inner <see cref="Entry"/>.</summary>
    public string? FontFamily { get => (string?)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }

    public event EventHandler? TextChanged;
    public event EventHandler? Completed;

    private readonly Border _border;
    private readonly Entry _entry;
    private readonly Label _floatingLabel;
    private readonly Label _revealLabel;
    private bool _isRevealed;
    private bool _isFocused;
    private bool _suppressSync;
    private EventHandler<FluentThemeChangedEventArgs>? _themeHandler;

    public FlPasswordInput()
    {
        _entry = new Entry
        {
            IsPassword = true,
            BackgroundColor = Colors.Transparent,
            FontSize = FluentTypography.BodySize
        };
        FluentInputHelper.StripNativeDecorations(_entry);

        _floatingLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            InputTransparent = true
        };

        _revealLabel = new Label
        {
            Text = "\u25C9",
            FontSize = 16,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        var revealTap = new TapGestureRecognizer();
        revealTap.Tapped += OnRevealTapped;
        _revealLabel.GestureRecognizers.Add(revealTap);

        var inputGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(32)
            },
            Children = { _entry, _floatingLabel }
        };
        Grid.SetColumn(_entry, 0);
        Grid.SetColumn(_floatingLabel, 0);
        Grid.SetColumn(_revealLabel, 1);
        inputGrid.Children.Add(_revealLabel);

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
            Content = inputGrid
        };

        Content = _border;

        _entry.Focused += (s, e) => { _isFocused = true; ApplyVisualState(); AnimateLabel(true); };
        _entry.Unfocused += (s, e) => { _isFocused = false; ApplyVisualState(); if (string.IsNullOrEmpty(_entry.Text)) AnimateLabel(false); };
        _entry.TextChanged += OnEntryTextChanged;
        _entry.Completed += OnEntryCompleted;

        ApplyVisualState();
        UpdateLabels();

        // Subscribe on attach, unsubscribe on detach — no singleton leak.
        Loaded += OnAttached;
        Unloaded += OnDetached;
    }

    private void OnAttached(object? sender, EventArgs e)
    {
        _themeHandler = (_, _) => Dispatcher.Dispatch(() => { ApplyVisualState(); UpdateLabels(); });
        FluentThemeManager.Current.ThemeChanged += _themeHandler;
        ApplyVisualState();
        UpdateLabels();
    }

    private void OnDetached(object? sender, EventArgs e)
    {
        if (_themeHandler is not null) FluentThemeManager.Current.ThemeChanged -= _themeHandler;
        _themeHandler = null;
    }

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
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnEntryCompleted(object? sender, EventArgs e)
    {
        Completed?.Invoke(this, EventArgs.Empty);
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    private void OnRevealTapped(object? sender, TappedEventArgs e)
    {
        _isRevealed = !_isRevealed;
        _entry.IsPassword = !_isRevealed;
        _revealLabel.Text = _isRevealed ? "\u25C8" : "\u25C9";
    }

    private void OnTextChanged()
    {
        if (!_suppressSync && _entry.Text != Text)
        {
            _suppressSync = true;
            _entry.Text = Text;
            _suppressSync = false;
        }
        if (!string.IsNullOrEmpty(Text) && !_isFocused) AnimateLabel(true);
        else if (string.IsNullOrEmpty(Text) && !_isFocused) AnimateLabel(false);
    }

    private void AnimateLabel(bool isFloating)
    {
        if (isFloating)
        {
            _floatingLabel.TranslateToAsync(0, -22, FluentAnimations.Fast, FluentAnimations.Entrance);
            _floatingLabel.FontSize = FluentTypography.CaptionSize;
            _floatingLabel.TextColor = _isFocused
                ? (HasError ? FluentThemeManager.Current.CurrentTheme.Error : FluentThemeManager.Current.CurrentTheme.Accent)
                : FluentThemeManager.Current.CurrentTheme.TextSecondary;
        }
        else
        {
            _floatingLabel.TranslateToAsync(0, 0, FluentAnimations.Fast, FluentAnimations.Entrance);
            _floatingLabel.FontSize = FluentTypography.BodySize;
            _floatingLabel.TextColor = FluentThemeManager.Current.CurrentTheme.TextTertiary;
        }
    }

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        if (HasError) { _border.Stroke = theme.Error; _border.StrokeThickness = 2; }
        else if (_isFocused) { _border.Stroke = theme.Accent; _border.StrokeThickness = 2; }
        else { _border.Stroke = theme.Border; _border.StrokeThickness = 1; }
        _border.BackgroundColor = theme.Surface;
        _entry.TextColor = theme.TextPrimary;
        _entry.ReturnType = ReturnType;
        _entry.FlowDirection = FlowDirection;
        if (!string.IsNullOrEmpty(FontFamily)) _entry.FontFamily = FontFamily;
        _revealLabel.TextColor = theme.TextSecondary;
    }

    private void UpdateLabels()
    {
        _floatingLabel.Text = !string.IsNullOrEmpty(Label) ? Label : Placeholder;
        _entry.Placeholder = string.IsNullOrEmpty(Label) ? Placeholder : string.Empty;
    }
}
