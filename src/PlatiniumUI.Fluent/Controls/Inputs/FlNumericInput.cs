using PlatiniumUI.Fluent.DesignSystem.Globalization;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Platform;

namespace PlatiniumUI.Fluent.Controls.Inputs;

/// <summary>
/// Fluent Design numeric input for entering numbers with format support.
/// Culture-safe: normalizes Persian/Arabic digits before parsing, keeps decimals
/// (milk ۲۵٫۵ never rounds to ۲۶), never clamps mid-typing, and displays value
/// separately from the committed numeric <see cref="Value"/>.
/// </summary>
public class FlNumericInput : ContentView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double?), typeof(FlNumericInput), null, BindingMode.TwoWay,
            propertyChanged: (b, _, n) => ((FlNumericInput)b).OnValueChanged(n as double?));

    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(double), typeof(FlNumericInput), double.MinValue);

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(double), typeof(FlNumericInput), double.MaxValue);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlNumericInput), string.Empty,
            propertyChanged: (b, _, _) => ((FlNumericInput)b).UpdateLabel());

    public static readonly BindableProperty FormatStringProperty =
        BindableProperty.Create(nameof(FormatString), typeof(string), typeof(FlNumericInput), null);

    public static readonly BindableProperty AllowDecimalsProperty =
        BindableProperty.Create(nameof(AllowDecimals), typeof(bool), typeof(FlNumericInput), true);

    public static readonly BindableProperty DecimalsProperty =
        BindableProperty.Create(nameof(Decimals), typeof(int), typeof(FlNumericInput), 2);

    public static readonly BindableProperty ShowPersianDigitsProperty =
        BindableProperty.Create(nameof(ShowPersianDigits), typeof(bool), typeof(FlNumericInput), false);

    public double? Value { get => (double?)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    /// <summary>Explicit display format; when null it is derived from <see cref="AllowDecimals"/>/<see cref="Decimals"/>.</summary>
    public string? FormatString { get => (string?)GetValue(FormatStringProperty); set => SetValue(FormatStringProperty, value); }
    /// <summary>When false, the value is rounded to an integer on commit (currency/Rials fields).</summary>
    public bool AllowDecimals { get => (bool)GetValue(AllowDecimalsProperty); set => SetValue(AllowDecimalsProperty, value); }
    /// <summary>Fraction digits kept when <see cref="AllowDecimals"/> is true.</summary>
    public int Decimals { get => (int)GetValue(DecimalsProperty); set => SetValue(DecimalsProperty, value); }
    /// <summary>Render committed digits as Persian (۰-۹) instead of Latin.</summary>
    public bool ShowPersianDigits { get => (bool)GetValue(ShowPersianDigitsProperty); set => SetValue(ShowPersianDigitsProperty, value); }

    public event EventHandler<double?>? ValueChanged;

    private readonly Entry _entry;
    private readonly Border _border;
    private readonly Label _label;
    private bool _suppressTextSync;   // guards programmatic text writes against the TextChanged handler
    private EventHandler<FluentThemeChangedEventArgs>? _themeHandler;

    public FlNumericInput()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = theme.TextSecondary,
            IsVisible = false
        };

        _entry = new Entry
        {
            Keyboard = Keyboard.Numeric,
            BackgroundColor = Colors.Transparent,
            TextColor = theme.TextPrimary,
            FontSize = FluentTypography.BodySize,
            HorizontalTextAlignment = TextAlignment.End
        };
        FluentInputHelper.StripNativeDecorations(_entry);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Input)
            },
            Stroke = theme.Border,
            StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = theme.Surface
        };

        var layout = new VerticalStackLayout { Spacing = 4, Children = { _label, _border } };
        _border.Content = _entry;
        Content = layout;

        _entry.TextChanged += OnEntryTextChanged;
        _entry.Focused += (_, _) => { _border.Stroke = FluentThemeManager.Current.CurrentTheme.Accent; _border.StrokeThickness = 2; };
        _entry.Unfocused += (_, _) => { _border.Stroke = FluentThemeManager.Current.CurrentTheme.Border; _border.StrokeThickness = 1; CommitAndFormat(); };

        // Subscribe on attach, unsubscribe on detach — no singleton leak across ~44 pages.
        Loaded += OnAttached;
        Unloaded += OnDetached;
    }

    private void OnAttached(object? sender, EventArgs e)
    {
        _themeHandler = (_, _) => Dispatcher.Dispatch(ApplyTheme);
        FluentThemeManager.Current.ThemeChanged += _themeHandler;
        ApplyTheme();
        if (!_entry.IsFocused) SyncTextFromValue();
    }

    private void OnDetached(object? sender, EventArgs e)
    {
        if (_themeHandler is not null) FluentThemeManager.Current.ThemeChanged -= _themeHandler;
        _themeHandler = null;
    }

    private void ApplyTheme()
    {
        var t = FluentThemeManager.Current.CurrentTheme;
        _entry.TextColor = t.TextPrimary;
        _label.TextColor = t.TextSecondary;
        _border.Stroke = t.Border;
        _border.BackgroundColor = t.Surface;
    }

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_suppressTextSync) return;

        if (string.IsNullOrEmpty(e.NewTextValue))
        {
            Value = null;
            return;
        }
        // Normalize Persian/Arabic digits + separators, then invariant parse. No clamp mid-typing.
        if (PersianFormat.TryParseDecimal(e.NewTextValue, out var result))
            Value = (double)result;   // partial input like "25." parses to 25; user keeps typing
    }

    private void OnValueChanged(double? newValue)
    {
        ValueChanged?.Invoke(this, newValue);
        // Reflect an externally-set value into the field, but never fight the user while focused.
        if (!_entry.IsFocused)
            SyncTextFromValue();
    }

    /// <summary>On blur: clamp, round per policy, and rewrite the field from the committed value.</summary>
    private void CommitAndFormat()
    {
        if (Value.HasValue)
        {
            var clamped = Math.Clamp(Value.Value, Minimum, Maximum);
            clamped = AllowDecimals
                ? Math.Round(clamped, Math.Max(0, Decimals), MidpointRounding.AwayFromZero)
                : Math.Round(clamped, 0, MidpointRounding.AwayFromZero);
            if (clamped != Value) Value = clamped;
        }
        SyncTextFromValue();
    }

    private void SyncTextFromValue()
    {
        _suppressTextSync = true;
        _entry.Text = Value.HasValue ? FormatForDisplay(Value.Value) : string.Empty;
        _suppressTextSync = false;
    }

    private string FormatForDisplay(double value)
    {
        var fmt = FormatString ?? (AllowDecimals ? $"N{Math.Max(0, Decimals)}" : "N0");
        var text = value.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);
        return ShowPersianDigits ? PersianFormat.ToPersianDigits(text) : text;
    }

    private void UpdateLabel()
    {
        _label.Text = Label;
        _label.IsVisible = !string.IsNullOrEmpty(Label);
    }
}

/// <summary>
/// Fluent Design numeric up/down control with increment/decrement buttons.
/// </summary>
public class FlNumericUpDown : ContentView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double), typeof(FlNumericUpDown), 0.0, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlNumericUpDown)b).UpdateDisplay());

    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(double), typeof(FlNumericUpDown), 0.0);

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(double), typeof(FlNumericUpDown), 100.0);

    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(nameof(Step), typeof(double), typeof(FlNumericUpDown), 1.0);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlNumericUpDown), string.Empty,
            propertyChanged: (b, _, _) => ((FlNumericUpDown)b).UpdateLabel());

    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Step { get => (double)GetValue(StepProperty); set => SetValue(StepProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

    public event EventHandler<double>? ValueChanged;

    private readonly Label _valueLabel;
    private readonly Label _label;

    public FlNumericUpDown()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = theme.TextSecondary,
            IsVisible = false
        };

        var downBtn = CreateButton("-", OnDecrement);
        _valueLabel = new Label
        {
            Text = "0",
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextPrimary,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            MinimumWidthRequest = 48
        };
        var upBtn = CreateButton("+", OnIncrement);

        var content = new HorizontalStackLayout
        {
            Spacing = 0,
            VerticalOptions = LayoutOptions.Center,
            Children = { downBtn, _valueLabel, upBtn }
        };

        var border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Input)
            },
            Stroke = theme.Border,
            StrokeThickness = 1,
            BackgroundColor = theme.Surface,
            Content = content,
            HeightRequest = 40
        };

        var layout = new VerticalStackLayout { Spacing = 4, Children = { _label, border } };
        Content = layout;

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _valueLabel.TextColor = t.TextPrimary;
            _label.TextColor = t.TextSecondary;
            border.Stroke = t.Border;
            border.BackgroundColor = t.Surface;
        });
    }

    private Border CreateButton(string text, Action action)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        var label = new Label
        {
            Text = text,
            FontSize = 18,
            TextColor = theme.TextPrimary,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        var border = new Border
        {
            WidthRequest = 40,
            Stroke = Colors.Transparent,
            BackgroundColor = Colors.Transparent,
            Content = label
        };
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => action();
        border.GestureRecognizers.Add(tap);
        return border;
    }

    private void OnIncrement()
    {
        var newVal = Math.Min(Value + Step, Maximum);
        Value = newVal;
        ValueChanged?.Invoke(this, Value);
    }

    private void OnDecrement()
    {
        var newVal = Math.Max(Value - Step, Minimum);
        Value = newVal;
        ValueChanged?.Invoke(this, Value);
    }

    private void UpdateDisplay()
    {
        _valueLabel.Text = Value.ToString("N0");
    }

    private void UpdateLabel()
    {
        _label.Text = Label;
        _label.IsVisible = !string.IsNullOrEmpty(Label);
    }
}
