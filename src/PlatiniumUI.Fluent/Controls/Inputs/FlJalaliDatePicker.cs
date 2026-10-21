using PlatiniumUI.Fluent.DesignSystem.Globalization;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.Inputs;

/// <summary>
/// Jalali (Persian) date picker — the RTL/Persian replacement for the Gregorian-only
/// <see cref="FlDatePicker"/>. Presents three linked pickers (year / month / day) that
/// respect Persian month lengths and Esfand leap days via the verified Jalali engine,
/// and commits a Gregorian DateTime (midnight) so it round-trips cleanly to the server.
/// </summary>
public class FlJalaliDatePicker : ContentView
{
    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(FlJalaliDatePicker), null,
            BindingMode.TwoWay, propertyChanged: (b, _, n) => ((FlJalaliDatePicker)b).OnSelectedDateChanged(n as DateTime?));

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FlJalaliDatePicker), "تاریخ",
            propertyChanged: (b, _, _) => ((FlJalaliDatePicker)b).UpdateLabel());

    public static readonly BindableProperty YearSpanBeforeProperty =
        BindableProperty.Create(nameof(YearSpanBefore), typeof(int), typeof(FlJalaliDatePicker), 100);

    public static readonly BindableProperty YearSpanAfterProperty =
        BindableProperty.Create(nameof(YearSpanAfter), typeof(int), typeof(FlJalaliDatePicker), 5);

    public DateTime? SelectedDate { get => (DateTime?)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public int YearSpanBefore { get => (int)GetValue(YearSpanBeforeProperty); set => SetValue(YearSpanBeforeProperty, value); }
    public int YearSpanAfter { get => (int)GetValue(YearSpanAfterProperty); set => SetValue(YearSpanAfterProperty, value); }

    /// <summary>Raised whenever the user commits a new date (the value is a Gregorian midnight).</summary>
    public event EventHandler<DateTime?>? DateSelected;

    private readonly Picker _yearPicker = new();
    private readonly Picker _monthPicker = new();
    private readonly Picker _dayPicker = new();
    private readonly Label _titleLabel;
    private readonly Border _border;
    private readonly List<int> _years = new();
    private bool _suppress;
    private bool _built;
    private EventHandler<FluentThemeChangedEventArgs>? _themeHandler;

    public FlJalaliDatePicker()
    {
        FlowDirection = FlowDirection.RightToLeft;

        _titleLabel = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = FluentThemeManager.Current.CurrentTheme.TextSecondary,
        };

        foreach (var p in new[] { _yearPicker, _monthPicker, _dayPicker })
        {
            p.BackgroundColor = Colors.Transparent;
            p.TextColor = FluentThemeManager.Current.CurrentTheme.TextPrimary;
            p.FontSize = FluentTypography.BodySize;
        }

        var grid = new Grid
        {
            ColumnSpacing = 6,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
                new ColumnDefinition(GridLength.Star),
            }
        };
        grid.Add(_yearPicker, 0);
        grid.Add(_monthPicker, 1);
        grid.Add(_dayPicker, 2);

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Input) },
            Stroke = FluentThemeManager.Current.CurrentTheme.Border,
            StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.InputPaddingHorizontal, FluentSpacing.InputPaddingVertical),
            BackgroundColor = FluentThemeManager.Current.CurrentTheme.Surface,
            Content = grid
        };

        Content = new VerticalStackLayout { Spacing = 4, Children = { _titleLabel, _border } };

        _yearPicker.SelectedIndexChanged += (_, _) => RebuildDaysAndCommit();
        _monthPicker.SelectedIndexChanged += (_, _) => RebuildDaysAndCommit();
        _dayPicker.SelectedIndexChanged += (_, _) => CommitFromPickers();

        UpdateLabel();

        Loaded += OnAttached;
        Unloaded += OnDetached;
    }

    private void OnAttached(object? sender, EventArgs e)
    {
        _themeHandler = (_, _) => Dispatcher.Dispatch(ApplyTheme);
        FluentThemeManager.Current.ThemeChanged += _themeHandler;
        ApplyTheme();
        EnsureBuilt();
    }

    private void OnDetached(object? sender, EventArgs e)
    {
        if (_themeHandler is not null) FluentThemeManager.Current.ThemeChanged -= _themeHandler;
        _themeHandler = null;
    }

    private void ApplyTheme()
    {
        var t = FluentThemeManager.Current.CurrentTheme;
        _titleLabel.TextColor = t.TextSecondary;
        foreach (var p in new[] { _yearPicker, _monthPicker, _dayPicker }) p.TextColor = t.TextPrimary;
        _border.Stroke = t.Border;
        _border.BackgroundColor = t.Surface;
    }

    private void UpdateLabel() => _titleLabel.Text = Label;

    private void EnsureBuilt()
    {
        if (_built) return;
        _built = true;
        var current = SelectedDate.HasValue
            ? Jalali.FromIranWallClock(IranClock.ToIranWallClock(SelectedDate.Value))
            : Jalali.FromUtc(DateTime.UtcNow);
        BuildColumns(current.Year, current.Month, current.Day);
    }

    private void OnSelectedDateChanged(DateTime? newValue)
    {
        if (_suppress) return;
        DateSelected?.Invoke(this, newValue);
        if (newValue.HasValue && _built)
        {
            var j = Jalali.FromIranWallClock(IranClock.ToIranWallClock(newValue.Value));
            BuildColumns(j.Year, j.Month, j.Day);
        }
    }

    private void BuildColumns(int selectYear, int selectMonth, int selectDay)
    {
        _suppress = true;

        var todayJy = Jalali.FromUtc(DateTime.UtcNow).Year;
        _years.Clear();
        _yearPicker.Items.Clear();
        for (var y = todayJy - YearSpanBefore; y <= todayJy + YearSpanAfter; y++)
        {
            _years.Add(y);
            _yearPicker.Items.Add(PersianFormat.ToPersianDigits(y.ToString()));
        }
        var yi = _years.IndexOf(selectYear);
        _yearPicker.SelectedIndex = yi < 0 ? Math.Max(0, Math.Min(_years.Count - 1, YearSpanBefore)) : yi;

        BuildMonthItems();
        _monthPicker.SelectedIndex = Math.Max(0, Math.Min(11, selectMonth - 1));

        BuildDayItems(selectYear, selectMonth);
        _dayPicker.SelectedIndex = Math.Max(0, Math.Min(_dayPicker.Items.Count - 1, selectDay - 1));

        _suppress = false;
    }

    private void BuildMonthItems()
    {
        _monthPicker.Items.Clear();
        foreach (var name in Jalali.MonthNames)
            _monthPicker.Items.Add(name);
    }

    private void BuildDayItems(int jy, int jm)
    {
        _dayPicker.Items.Clear();
        var count = Jalali.DaysInMonth(jy, jm);
        for (var d = 1; d <= count; d++)
            _dayPicker.Items.Add(PersianFormat.ToPersianDigits(d.ToString()));
    }

    private int CurrentYear => _yearPicker.SelectedIndex >= 0 ? _years[_yearPicker.SelectedIndex] : Jalali.FromUtc(DateTime.UtcNow).Year;
    private int CurrentMonth => _monthPicker.SelectedIndex + 1;
    private int CurrentDay => _dayPicker.SelectedIndex + 1;

    /// <summary>Year or month changed: rebuild the day column (Esfand 29/30, 31-day months 1-6), then commit.</summary>
    private void RebuildDaysAndCommit()
    {
        if (_suppress) return;
        var prevDay = _dayPicker.SelectedIndex + 1;
        _suppress = true;
        BuildDayItems(CurrentYear, CurrentMonth);
        _dayPicker.SelectedIndex = Math.Min(_dayPicker.Items.Count - 1, Math.Max(0, prevDay - 1));
        _suppress = false;
        CommitFromPickers();
    }

    private void CommitFromPickers()
    {
        if (_suppress) return;
        if (_yearPicker.SelectedIndex < 0 || _monthPicker.SelectedIndex < 0 || _dayPicker.SelectedIndex < 0)
            return;
        var j = new JalaliDate(CurrentYear, CurrentMonth, CurrentDay);
        var gregorian = DateTime.SpecifyKind(Jalali.ToGregorian(j), DateTimeKind.Unspecified);
        _suppress = true;
        SelectedDate = gregorian;
        _suppress = false;
        DateSelected?.Invoke(this, SelectedDate);
    }
}
