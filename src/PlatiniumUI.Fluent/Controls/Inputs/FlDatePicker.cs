using System.Collections;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Platform;

namespace PlatiniumUI.Fluent.Controls.Inputs;

/// <summary>
/// Fluent Design date picker with floating label.
/// </summary>
public class FlDatePicker : ContentView
{
    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(FlDatePicker), DateTime.Today, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlDatePicker)b).UpdateDisplay());

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlDatePicker), "Select Date",
            propertyChanged: (b, _, _) => ((FlDatePicker)b).UpdateLabel());

    public static readonly BindableProperty MinimumDateProperty =
        BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(FlDatePicker), new DateTime(1900, 1, 1));

    public static readonly BindableProperty MaximumDateProperty =
        BindableProperty.Create(nameof(MaximumDate), typeof(DateTime), typeof(FlDatePicker), new DateTime(2100, 12, 31));

    public DateTime Date { get => (DateTime)GetValue(DateProperty); set => SetValue(DateProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public DateTime MinimumDate { get => (DateTime)GetValue(MinimumDateProperty); set => SetValue(MinimumDateProperty, value); }
    public DateTime MaximumDate { get => (DateTime)GetValue(MaximumDateProperty); set => SetValue(MaximumDateProperty, value); }

    public event EventHandler<DateTime>? DateSelected;

    private readonly DatePicker _picker;
    private readonly Border _border;
    private readonly Label _label;

    public FlDatePicker()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = theme.TextSecondary,
            Text = Label
        };

        _picker = new DatePicker
        {
            BackgroundColor = Colors.Transparent,
            TextColor = theme.TextPrimary,
            FontSize = FluentTypography.BodySize,
            MinimumDate = MinimumDate,
            MaximumDate = MaximumDate,
            Format = "MMM dd, yyyy"
        };
        FluentInputHelper.StripNativeDecorations(_picker);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Input)
            },
            Stroke = theme.Border,
            StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = theme.Surface,
            Content = _picker
        };

        var layout = new VerticalStackLayout { Spacing = 4, Children = { _label, _border } };
        Content = layout;

        _picker.Focused += (s, e) => { _border.Stroke = theme.Accent; _border.StrokeThickness = 2; };
        _picker.Unfocused += (s, e) => { _border.Stroke = theme.Border; _border.StrokeThickness = 1; };
        _picker.DateSelected += (s, e) => { if (e.NewDate.HasValue) { Date = e.NewDate.Value; DateSelected?.Invoke(this, e.NewDate.Value); } };

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _picker.TextColor = t.TextPrimary;
            _label.TextColor = t.TextSecondary;
            _border.Stroke = t.Border;
            _border.BackgroundColor = t.Surface;
        });
    }

    private void UpdateDisplay() { }
    private void UpdateLabel() { _label.Text = Label; }
}

/// <summary>
/// Fluent Design time picker with floating label.
/// </summary>
public class FlTimePicker : ContentView
{
    public static readonly BindableProperty TimeProperty =
        BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(FlTimePicker), TimeSpan.FromHours(12), BindingMode.TwoWay);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlTimePicker), "Select Time",
            propertyChanged: (b, _, _) => ((FlTimePicker)b).UpdateLabel());

    public TimeSpan Time { get => (TimeSpan)GetValue(TimeProperty); set => SetValue(TimeProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

    private readonly TimePicker _picker;
    private readonly Border _border;
    private readonly Label _label;

    public FlTimePicker()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label { FontSize = FluentTypography.CaptionSize, TextColor = theme.TextSecondary, Text = Label };

        _picker = new TimePicker
        {
            BackgroundColor = Colors.Transparent,
            TextColor = theme.TextPrimary,
            FontSize = FluentTypography.BodySize,
            Format = "hh:mm tt"
        };
        FluentInputHelper.StripNativeDecorations(_picker);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Input) },
            Stroke = theme.Border, StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = theme.Surface, Content = _picker
        };

        Content = new VerticalStackLayout { Spacing = 4, Children = { _label, _border } };

        _picker.Focused += (s, e) => { _border.Stroke = theme.Accent; _border.StrokeThickness = 2; };
        _picker.Unfocused += (s, e) => { _border.Stroke = theme.Border; _border.StrokeThickness = 1; };
        _picker.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(TimePicker.Time) && _picker.Time.HasValue) Time = _picker.Time.Value; };

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _picker.TextColor = t.TextPrimary; _label.TextColor = t.TextSecondary;
            _border.Stroke = t.Border; _border.BackgroundColor = t.Surface;
        });
    }

    private void UpdateLabel() { _label.Text = Label; }
}

/// <summary>
/// Fluent Design combined date and time picker.
/// </summary>
public class FlDateTimePicker : ContentView
{
    public static readonly BindableProperty DateTimeProperty =
        BindableProperty.Create(nameof(DateTime), typeof(DateTime), typeof(FlDateTimePicker), DateTime.Now, BindingMode.TwoWay);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlDateTimePicker), "Select Date & Time",
            propertyChanged: (b, _, _) => ((FlDateTimePicker)b).UpdateLabel());

    public DateTime DateTime { get => (DateTime)GetValue(DateTimeProperty); set => SetValue(DateTimeProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

    private readonly DatePicker _datePicker;
    private readonly TimePicker _timePicker;
    private readonly Label _label;

    public FlDateTimePicker()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label { FontSize = FluentTypography.CaptionSize, TextColor = theme.TextSecondary, Text = Label };

        _datePicker = new DatePicker
        {
            BackgroundColor = Colors.Transparent, TextColor = theme.TextPrimary,
            FontSize = FluentTypography.BodySize, Format = "MMM dd, yyyy"
        };

        _timePicker = new TimePicker
        {
            BackgroundColor = Colors.Transparent, TextColor = theme.TextPrimary,
            FontSize = FluentTypography.BodySize, Format = "hh:mm tt"
        };

        var dateBorder = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Input) },
            Stroke = theme.Border, StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = theme.Surface, Content = _datePicker
        };

        var timeBorder = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Input) },
            Stroke = theme.Border, StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = theme.Surface, Content = _timePicker
        };

        var row = new HorizontalStackLayout { Spacing = 8, Children = { dateBorder, timeBorder } };
        Content = new VerticalStackLayout { Spacing = 4, Children = { _label, row } };

        _datePicker.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(DatePicker.Date)) UpdateDateTime(); };
        _timePicker.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(TimePicker.Time)) UpdateDateTime(); };

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _datePicker.TextColor = t.TextPrimary; _timePicker.TextColor = t.TextPrimary;
            _label.TextColor = t.TextSecondary;
            dateBorder.Stroke = t.Border; dateBorder.BackgroundColor = t.Surface;
            timeBorder.Stroke = t.Border; timeBorder.BackgroundColor = t.Surface;
        });
    }

    private void UpdateDateTime()
    {
        var date = _datePicker.Date ?? DateTime.Today;
        var time = _timePicker.Time ?? TimeSpan.Zero;
        DateTime = date.Date + time;
    }

    private void UpdateLabel() { _label.Text = Label; }
}

/// <summary>
/// Fluent Design picker (dropdown select) with floating label.
/// Accepts any item objects and maps a Persian display label to a bound value via
/// <see cref="DisplayMemberPath"/> / <see cref="ValueMemberPath"/> (e.g. feed-type
/// "کنسانتره" ↔ "concentrate"), so screens never juggle parallel string lists.
/// </summary>
public class FlPicker : ContentView
{
    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(FlPicker), -1, BindingMode.TwoWay,
            propertyChanged: (b, _, n) => ((FlPicker)b).OnSelectedIndexChangedProperty((int)n));

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(FlPicker), null, BindingMode.TwoWay,
            propertyChanged: (b, _, n) => ((FlPicker)b).OnSelectedItemChangedProperty(n));

    public static readonly BindableProperty SelectedValueProperty =
        BindableProperty.Create(nameof(SelectedValue), typeof(object), typeof(FlPicker), null, BindingMode.TwoWay);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlPicker), "Select an option",
            propertyChanged: (b, _, _) => ((FlPicker)b).UpdateLabel());

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(FlPicker), null,
            propertyChanged: (b, _, _) => ((FlPicker)b).UpdateItems());

    public static readonly BindableProperty DisplayMemberPathProperty =
        BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(FlPicker), string.Empty,
            propertyChanged: (b, _, _) => ((FlPicker)b).UpdateItems());

    public static readonly BindableProperty ValueMemberPathProperty =
        BindableProperty.Create(nameof(ValueMemberPath), typeof(string), typeof(FlPicker), string.Empty);

    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
    public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
    public object? SelectedValue { get => GetValue(SelectedValueProperty); set => SetValue(SelectedValueProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public IEnumerable? ItemsSource { get => (IEnumerable?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    /// <summary>Property path (simple or dotted) used as each item's visible label; empty → ToString().</summary>
    public string DisplayMemberPath { get => (string)GetValue(DisplayMemberPathProperty); set => SetValue(DisplayMemberPathProperty, value); }
    /// <summary>Property path used as each item's bound value; empty → the item itself.</summary>
    public string ValueMemberPath { get => (string)GetValue(ValueMemberPathProperty); set => SetValue(ValueMemberPathProperty, value); }

    public event EventHandler<int>? SelectedIndexChanged;
    public event EventHandler<object?>? SelectionChanged;

    private readonly Picker _picker;
    private readonly Border _border;
    private readonly Label _label;
    private readonly List<object> _items = new();
    private bool _suppress;
    private EventHandler<FluentThemeChangedEventArgs>? _themeHandler;

    public FlPicker()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label { FontSize = FluentTypography.CaptionSize, TextColor = theme.TextSecondary, Text = Label };

        _picker = new Picker
        {
            BackgroundColor = Colors.Transparent,
            TextColor = theme.TextPrimary,
            TitleColor = theme.TextTertiary,
            FontSize = FluentTypography.BodySize
        };
        FluentInputHelper.StripNativeDecorations(_picker);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Input) },
            Stroke = theme.Border, StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = theme.Surface, Content = _picker
        };

        Content = new VerticalStackLayout { Spacing = 4, Children = { _label, _border } };

        _picker.SelectedIndexChanged += OnPickerSelectedIndexChanged;
        _picker.Focused += (_, _) => { _border.Stroke = FluentThemeManager.Current.CurrentTheme.Accent; _border.StrokeThickness = 2; };
        _picker.Unfocused += (_, _) => { _border.Stroke = FluentThemeManager.Current.CurrentTheme.Border; _border.StrokeThickness = 1; };

        Loaded += OnAttached;
        Unloaded += OnDetached;
    }

    private void OnAttached(object? sender, EventArgs e)
    {
        _themeHandler = (_, _) => Dispatcher.Dispatch(ApplyTheme);
        FluentThemeManager.Current.ThemeChanged += _themeHandler;
        ApplyTheme();
    }

    private void OnDetached(object? sender, EventArgs e)
    {
        if (_themeHandler is not null) FluentThemeManager.Current.ThemeChanged -= _themeHandler;
        _themeHandler = null;
    }

    private void ApplyTheme()
    {
        var t = FluentThemeManager.Current.CurrentTheme;
        _picker.TextColor = t.TextPrimary; _picker.TitleColor = t.TextTertiary;
        _label.TextColor = t.TextSecondary; _border.Stroke = t.Border; _border.BackgroundColor = t.Surface;
    }

    private static object? GetPathValue(object? item, string path)
    {
        if (item is null || string.IsNullOrEmpty(path)) return item;
        object? cur = item;
        foreach (var seg in path.Split('.'))
        {
            var pi = cur?.GetType().GetProperty(seg);
            if (pi is null) return null;
            cur = pi.GetValue(cur);
        }
        return cur;
    }

    private string GetDisplay(object item)
    {
        var v = string.IsNullOrEmpty(DisplayMemberPath) ? null : GetPathValue(item, DisplayMemberPath);
        return (v ?? item)?.ToString() ?? string.Empty;
    }

    private object? GetVal(object? item) =>
        item is null || string.IsNullOrEmpty(ValueMemberPath) ? item : GetPathValue(item, ValueMemberPath);

    private void UpdateItems()
    {
        _picker.Items.Clear();
        _items.Clear();
        if (ItemsSource is null) return;
        foreach (var obj in ItemsSource)
        {
            var item = obj!;
            _items.Add(item);
            _picker.Items.Add(GetDisplay(item));
        }
        // Re-apply current selection after items change.
        if (_suppress) return;
        _suppress = true;
        if (SelectedItem is not null)
            _picker.SelectedIndex = IndexOfItem(SelectedItem);
        else if (SelectedValue is not null)
            _picker.SelectedIndex = IndexOfValue(SelectedValue);
        else
            _picker.SelectedIndex = SelectedIndex;
        _suppress = false;
    }

    private int IndexOfItem(object item) => _items.IndexOf(item);

    private int IndexOfValue(object value)
    {
        for (var i = 0; i < _items.Count; i++)
            if (Equals(GetVal(_items[i]), value)) return i;
        return -1;
    }

    private void OnPickerSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_suppress) return;
        var idx = _picker.SelectedIndex;
        _suppress = true;
        SelectedIndex = idx;
        SelectedItem = idx >= 0 && idx < _items.Count ? _items[idx] : null;
        SelectedValue = GetVal(SelectedItem);
        _suppress = false;
        SelectedIndexChanged?.Invoke(this, idx);
        SelectionChanged?.Invoke(this, SelectedValue);
    }

    private void OnSelectedIndexChangedProperty(int newIndex)
    {
        if (_suppress) return;
        _suppress = true;
        _picker.SelectedIndex = newIndex;
        SelectedItem = newIndex >= 0 && newIndex < _items.Count ? _items[newIndex] : null;
        SelectedValue = GetVal(SelectedItem);
        _suppress = false;
        SelectedIndexChanged?.Invoke(this, newIndex);
        SelectionChanged?.Invoke(this, SelectedValue);
    }

    private void OnSelectedItemChangedProperty(object? newItem)
    {
        if (_suppress) return;
        _suppress = true;
        var idx = newItem is null ? -1 : IndexOfItem(newItem);
        _picker.SelectedIndex = idx;
        SelectedIndex = idx;
        SelectedValue = GetVal(newItem);
        _suppress = false;
    }

    private void UpdateLabel() { _label.Text = Label; }
}

/// <summary>
/// Fluent Design auto-complete input with suggestion dropdown.
/// </summary>
public class FlAutoComplete : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlAutoComplete), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlAutoComplete), string.Empty,
            propertyChanged: (b, _, _) => ((FlAutoComplete)b).UpdateLabel());

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FlAutoComplete), string.Empty,
            propertyChanged: (b, _, _) => ((FlAutoComplete)b).UpdatePlaceholder());

    public static readonly BindableProperty SuggestionsProperty =
        BindableProperty.Create(nameof(Suggestions), typeof(IList<string>), typeof(FlAutoComplete), null);

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public IList<string>? Suggestions { get => (IList<string>?)GetValue(SuggestionsProperty); set => SetValue(SuggestionsProperty, value); }

    public event EventHandler<string>? TextChanged;

    private readonly Entry _entry;
    private readonly Border _border;
    private readonly Label _label;

    public FlAutoComplete()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label { FontSize = FluentTypography.CaptionSize, TextColor = theme.TextSecondary, Text = Label };

        _entry = new Entry
        {
            BackgroundColor = Colors.Transparent,
            TextColor = theme.TextPrimary,
            FontSize = FluentTypography.BodySize
        };
        FluentInputHelper.StripNativeDecorations(_entry);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Input) },
            Stroke = theme.Border, StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = theme.Surface, Content = _entry
        };

        Content = new VerticalStackLayout { Spacing = 4, Children = { _label, _border } };

        _entry.TextChanged += (s, e) => { Text = e.NewTextValue ?? string.Empty; TextChanged?.Invoke(this, Text); };
        _entry.Focused += (s, e) => { _border.Stroke = theme.Accent; _border.StrokeThickness = 2; };
        _entry.Unfocused += (s, e) => { _border.Stroke = theme.Border; _border.StrokeThickness = 1; };

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _entry.TextColor = t.TextPrimary; _label.TextColor = t.TextSecondary;
            _border.Stroke = t.Border; _border.BackgroundColor = t.Surface;
        });
    }

    private void UpdateLabel() { _label.Text = Label; }
    private void UpdatePlaceholder() { _entry.Placeholder = Placeholder; }
}
