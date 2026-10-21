using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Globalization;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.Miscellaneous;

/// <summary>
/// Jalali (Persian) month calendar — the RTL/Persian counterpart of <see cref="FlCalendar"/>.
/// Renders a Saturday-first week grid whose month lengths and Esfand leap days come from the
/// verified <see cref="Jalali"/> engine, drills month→year for fast navigation, and commits a
/// Gregorian <see cref="DateTime"/> (midnight) so a picked day round-trips cleanly to the server.
/// Display strings are Persian (Vazirmatn digits + month names). Theme recolour is leak-safe via
/// <see cref="FluentThemeObservation.ObserveThemeChanged"/> — nothing is captured at construction.
/// </summary>
public class FlJalaliCalendar : ContentView
{
    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(FlJalaliCalendar), null,
            BindingMode.TwoWay, propertyChanged: (b, _, _) => ((FlJalaliCalendar)b).Refresh());

    public static readonly BindableProperty ViewModeProperty =
        BindableProperty.Create(nameof(ViewMode), typeof(FlJalaliCalendarViewMode), typeof(FlJalaliCalendar),
            FlJalaliCalendarViewMode.Month, propertyChanged: (b, _, _) => ((FlJalaliCalendar)b).Rebuild());

    public DateTime? SelectedDate { get => (DateTime?)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
    public FlJalaliCalendarViewMode ViewMode { get => (FlJalaliCalendarViewMode)GetValue(ViewModeProperty); set => SetValue(ViewModeProperty, value); }

    /// <summary>Raised when the user taps a day (the value is a Gregorian midnight).</summary>
    public event EventHandler<DateTime?>? DateSelected;

    // The visible Jalali period; starts on today (or the selected date) on the Iran wall clock.
    private int _displayYear;
    private int _displayMonth = 1;

    private readonly Grid _header;
    private readonly Label _headerLabel;
    private readonly Grid _body;
    private readonly Label _prev;
    private readonly Label _next;
    private bool _initialised;

    // Persian weekday abbreviations, Saturday-first (the Iranian week start).
    private static readonly string[] WeekdayNames = ["ش", "ی", "م", "چ", "پ", "ج", "ک"];

    public FlJalaliCalendar()
    {
        FlowDirection = FlowDirection.RightToLeft;
        var t = FluentThemeManager.Current.CurrentTheme;

        _prev = MakeNavGlyph("‹");
        _next = MakeNavGlyph("›");
        var prevTap = new TapGestureRecognizer();
        prevTap.Tapped += (_, _) => Navigate(-1);
        _prev.GestureRecognizers.Add(prevTap);
        var nextTap = new TapGestureRecognizer();
        nextTap.Tapped += (_, _) => Navigate(1);
        _next.GestureRecognizers.Add(nextTap);

        _headerLabel = new Label
        {
            FontSize = FluentTypography.SubtitleSize,
            FontAttributes = FontAttributes.Bold,
            TextColor = t.TextPrimary,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        var titleTap = new TapGestureRecognizer();
        titleTap.Tapped += (_, _) => ViewMode =
            ViewMode == FlJalaliCalendarViewMode.Month
                ? FlJalaliCalendarViewMode.Year
                : FlJalaliCalendarViewMode.Month;
        _headerLabel.GestureRecognizers.Add(titleTap);

        _header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
            },
        };
        _header.Add(_prev, 0);
        _header.Add(_headerLabel, 1);
        _header.Add(_next, 2);

        _body = new Grid { ColumnSpacing = 2, RowSpacing = 2 };

        Content = new VerticalStackLayout { Spacing = 6, Children = { _header, _body } };

        var today = Jalali.FromUtc(DateTime.UtcNow);
        _displayYear = today.Year;
        _displayMonth = today.Month;
        Rebuild();

        this.ObserveThemeChanged(Rebuild);
    }

    private static Label MakeNavGlyph(string glyph) => new()
    {
        Text = glyph,
        FontSize = FluentTypography.SubtitleSize,
        TextColor = FluentThemeManager.Current.CurrentTheme.Accent,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
        Padding = new Thickness(10, 4),
    };

    /// <summary>Today / the selected date when absent, on the Iran wall clock.</summary>
    private JalaliDate EffectiveJalali
    {
        get
        {
            if (SelectedDate is { } d)
                return Jalali.FromIranWallClock(IranClock.ToIranWallClock(d));
            return Jalali.FromUtc(DateTime.UtcNow);
        }
    }

    private void Navigate(int delta)
    {
        if (ViewMode == FlJalaliCalendarViewMode.Year)
        {
            _displayYear += delta;
        }
        else
        {
            _displayMonth += delta;
            if (_displayMonth < 1) { _displayMonth = 12; _displayYear--; }
            else if (_displayMonth > 12) { _displayMonth = 1; _displayYear++; }
        }
        Rebuild();
    }

    /// <summary>Point the visible period at the effective date (used on first refresh/selection).</summary>
    private void Refresh()
    {
        var j = EffectiveJalali;
        _displayYear = j.Year;
        _displayMonth = j.Month;
        Rebuild();
    }

    private void Rebuild()
    {
        if (!_initialised)
        {
            _initialised = true;
            var j = EffectiveJalali;
            _displayYear = j.Year;
            _displayMonth = j.Month;
        }

        _body.Children.Clear();
        _body.ColumnDefinitions.Clear();
        _body.RowDefinitions.Clear();

        var t = FluentThemeManager.Current.CurrentTheme;
        _headerLabel.TextColor = t.TextPrimary;

        if (ViewMode == FlJalaliCalendarViewMode.Month)
        {
            _headerLabel.Text = $"{Jalali.MonthName(_displayMonth)} {_displayYear:0000}";
            BuildMonthGrid(t);
        }
        else
        {
            _headerLabel.Text = _displayYear.ToString("0000");
            BuildYearGrid(t);
        }
    }

    private void BuildMonthGrid(IFluentTheme t)
    {
        for (var c = 0; c < 7; c++)
            _body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _body.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // weekday header
        for (var r = 0; r < 6; r++)
            _body.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

        for (var c = 0; c < 7; c++)
        {
            var wd = new Label
            {
                Text = WeekdayNames[c],
                FontSize = FluentTypography.CaptionSize,
                TextColor = t.TextSecondary,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 4),
            };
            _body.Add(wd, c, 0);
        }

        var days = Jalali.DaysInMonth(_displayYear, _displayMonth);
        // Saturday-first column index of day 1 (DayOfWeek: Sunday=0 … Saturday=6 → Sat=0).
        var firstWeekday = Jalali.ToGregorian(new JalaliDate(_displayYear, _displayMonth, 1)).DayOfWeek;
        var startCol = ((int)firstWeekday + 1) % 7;

        for (var day = 1; day <= days; day++)
        {
            var pos = startCol + day - 1;
            var row = pos / 7 + 1;
            var col = pos % 7;

            var isSel = SelectedDate is { } s &&
                        Jalali.FromIranWallClock(IranClock.ToIranWallClock(s))
                        == new JalaliDate(_displayYear, _displayMonth, day);
            var isToday = Jalali.FromUtc(DateTime.UtcNow) == new JalaliDate(_displayYear, _displayMonth, day);

            var cell = new Label
            {
                Text = PersianFormat.ToPersianDigits(day.ToString()),
                FontSize = FluentTypography.BodySize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = isSel ? t.TextOnAccent : t.TextPrimary,
                FontAttributes = isToday ? FontAttributes.Bold : FontAttributes.None,
            };
            var border = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Small) },
                Stroke = isSel ? Colors.Transparent : (isToday ? t.Accent : Colors.Transparent),
                StrokeThickness = isSel ? 0 : 1,
                BackgroundColor = isSel ? t.Accent : Colors.Transparent,
                Content = cell,
                HeightRequest = 34,
            };

            var picked = new JalaliDate(_displayYear, _displayMonth, day);
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => SelectDay(picked);
            border.GestureRecognizers.Add(tap);

            _body.Add(border, col, row);
        }
    }

    private void BuildYearGrid(IFluentTheme t)
    {
        for (var c = 0; c < 4; c++)
            _body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        for (var r = 0; r < 3; r++)
            _body.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

        for (var i = 0; i < 12; i++)
        {
            var month = i + 1;
            var row = i / 4;
            var col = i % 4;

            var isCurrentMonth = _displayYear == EffectiveJalali.Year && month == EffectiveJalali.Month;
            var label = new Label
            {
                Text = Jalali.MonthName(month),
                FontSize = FluentTypography.BodySize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = isCurrentMonth ? t.TextOnAccent : t.TextPrimary,
            };
            var border = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Small) },
                Stroke = Colors.Transparent,
                BackgroundColor = isCurrentMonth ? t.Accent : Colors.Transparent,
                Content = label,
                HeightRequest = 40,
            };
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                _displayMonth = month;
                ViewMode = FlJalaliCalendarViewMode.Month;
            };
            border.GestureRecognizers.Add(tap);

            _body.Add(border, col, row);
        }
    }

    private void SelectDay(JalaliDate j)
    {
        var gregorian = DateTime.SpecifyKind(Jalali.ToGregorian(j), DateTimeKind.Unspecified);
        SelectedDate = gregorian;
        DateSelected?.Invoke(this, gregorian);
    }
}

public enum FlJalaliCalendarViewMode { Month, Year }
