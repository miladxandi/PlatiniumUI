using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Platform;

namespace PlatiniumUI.Fluent.Controls.Inputs;

/// <summary>
/// Fluent Design OTP input with customizable length and masking.
/// </summary>
public class FlOTPInput : ContentView
{
    public static readonly BindableProperty LengthProperty =
        BindableProperty.Create(nameof(Length), typeof(int), typeof(FlOTPInput), 6,
            propertyChanged: (b, _, _) => ((FlOTPInput)b).Rebuild());

    public static readonly BindableProperty IsMaskedProperty =
        BindableProperty.Create(nameof(IsMasked), typeof(bool), typeof(FlOTPInput), false,
            propertyChanged: (b, _, _) => ((FlOTPInput)b).Rebuild());

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(FlOTPInput), string.Empty, BindingMode.TwoWay);

    public int Length { get => (int)GetValue(LengthProperty); set => SetValue(LengthProperty, value); }
    public bool IsMasked { get => (bool)GetValue(IsMaskedProperty); set => SetValue(IsMaskedProperty, value); }
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    public event EventHandler<string>? Completed;

    private readonly HorizontalStackLayout _stack;
    private readonly List<Entry> _entries = new();

    public FlOTPInput()
    {
        _stack = new HorizontalStackLayout { Spacing = 8 };
        Content = _stack;
        Rebuild();
    }

    private void Rebuild()
    {
        _stack.Children.Clear();
        _entries.Clear();
        var theme = FluentThemeManager.Current.CurrentTheme;

        for (int i = 0; i < Length; i++)
        {
            int index = i;
            var entry = new Entry
            {
                Keyboard = Keyboard.Numeric,
                MaxLength = 1,
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = theme.TextPrimary,
                BackgroundColor = Colors.Transparent,
                IsPassword = IsMasked
            };
            FluentInputHelper.StripNativeDecorations(entry);

            var border = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(FluentCornerRadius.Input)
                },
                Stroke = theme.Border,
                StrokeThickness = 1,
                WidthRequest = 44,
                HeightRequest = 52,
                Padding = new Thickness(4),
                BackgroundColor = theme.Surface,
                Content = entry
            };

            entry.Focused += (s, e) => { border.Stroke = theme.Accent; border.StrokeThickness = 2; };
            entry.Unfocused += (s, e) => { border.Stroke = theme.Border; border.StrokeThickness = 1; };
            entry.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.NewTextValue) && index < Length - 1)
                {
                    _entries[index + 1].Focus();
                }
                UpdateValue();
            };
            entry.Completed += (s, e) =>
            {
                if (Value.Length == Length)
                    Completed?.Invoke(this, Value);
            };

            _entries.Add(entry);
            _stack.Children.Add(border);
        }
    }

    private void UpdateValue()
    {
        Value = string.Concat(_entries.Select(e => e.Text ?? string.Empty));
        if (Value is null) Value = string.Empty;
    }

    public void Clear()
    {
        foreach (var entry in _entries)
            entry.Text = string.Empty;
        _entries.FirstOrDefault()?.Focus();
    }
}

/// <summary>
/// Fluent Design search bar with clear button.
/// </summary>
public class FlSearchBar : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlSearchBar), string.Empty, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlSearchBar)b).OnTextChanged());

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FlSearchBar), "Search...",
            propertyChanged: (b, _, _) => ((FlSearchBar)b).UpdatePlaceholder());

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

    public event EventHandler<string>? SearchRequested;
    public event EventHandler? TextChanged;

    private readonly Entry _entry;
    private readonly Label _clearLabel;
    private readonly Border _border;

    public FlSearchBar()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        var searchIcon = new Label
        {
            Text = "\u2315",
            FontSize = 18,
            TextColor = theme.TextTertiary,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 8, 0)
        };

        _clearLabel = new Label
        {
            Text = "\u2715",
            FontSize = 14,
            TextColor = theme.TextTertiary,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };
        var clearTap = new TapGestureRecognizer();
        clearTap.Tapped += (_, _) => { Text = string.Empty; _entry.Text = string.Empty; _clearLabel!.IsVisible = false; };
        _clearLabel.GestureRecognizers.Add(clearTap);

        _entry = new Entry
        {
            BackgroundColor = Colors.Transparent,
            TextColor = theme.TextPrimary,
            PlaceholderColor = theme.TextTertiary,
            FontSize = FluentTypography.BodySize
        };
        FluentInputHelper.StripNativeDecorations(_entry);

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        Grid.SetColumn(searchIcon, 0);
        Grid.SetColumn(_entry, 1);
        Grid.SetColumn(_clearLabel, 2);
        grid.Children.Add(searchIcon);
        grid.Children.Add(_entry);
        grid.Children.Add(_clearLabel);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Circular)
            },
            Stroke = theme.Border,
            StrokeThickness = 1,
            Padding = new Thickness(12, 8),
            BackgroundColor = theme.SurfaceVariant,
            Content = grid,
            HeightRequest = 40
        };

        Content = _border;

        _entry.TextChanged += (s, e) =>
        {
            Text = e.NewTextValue ?? string.Empty;
            _clearLabel.IsVisible = !string.IsNullOrEmpty(Text);
            TextChanged?.Invoke(this, EventArgs.Empty);
        };
        _entry.Completed += (s, e) => SearchRequested?.Invoke(this, Text);
        _entry.Focused += (s, e) => { _border.Stroke = theme.Accent; _border.StrokeThickness = 2; };
        _entry.Unfocused += (s, e) => { _border.Stroke = theme.Border; _border.StrokeThickness = 1; };

                this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _entry.TextColor = t.TextPrimary;
            _entry.PlaceholderColor = t.TextTertiary;
            searchIcon.TextColor = t.TextTertiary;
            _border.Stroke = t.Border;
            _border.BackgroundColor = t.SurfaceVariant;
        });
    }

    private void OnTextChanged()
    {
        if (_entry.Text != Text) _entry.Text = Text;
        _clearLabel.IsVisible = !string.IsNullOrEmpty(Text);
    }

    private void UpdatePlaceholder()
    {
        _entry.Placeholder = Placeholder;
    }
}
