using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Miscellaneous;

/// <summary>
/// Fluent Design calendar with multiple view modes.
/// </summary>
public class FlCalendar : ContentView
{
    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(FlCalendar), DateTime.Today, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlCalendar)b).UpdateSelection());

    public static readonly BindableProperty DisplayDateProperty =
        BindableProperty.Create(nameof(DisplayDate), typeof(DateTime), typeof(FlCalendar), DateTime.Today,
            propertyChanged: (b, _, _) => ((FlCalendar)b).Rebuild());

    public static readonly BindableProperty ViewModeProperty =
        BindableProperty.Create(nameof(ViewMode), typeof(FlCalendarViewMode), typeof(FlCalendar), FlCalendarViewMode.Month,
            propertyChanged: (b, _, _) => ((FlCalendar)b).Rebuild());

    public DateTime SelectedDate { get => (DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
    public DateTime DisplayDate { get => (DateTime)GetValue(DisplayDateProperty); set => SetValue(DisplayDateProperty, value); }
    public FlCalendarViewMode ViewMode { get => (FlCalendarViewMode)GetValue(ViewModeProperty); set => SetValue(ViewModeProperty, value); }

    public event EventHandler<DateTime>? DateSelected;

    private readonly Grid _header;
    private readonly Label _headerLabel;
    private readonly Grid _daysGrid;
    private readonly VerticalStackLayout _root;

    public FlCalendar()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _headerLabel = new Label
        {
            FontSize = FluentTypography.SubtitleSize,
            TextColor = theme.TextPrimary,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var prevButton = new Label
        {
            Text = "<",
            FontSize = FluentTypography.BodySize,
            TextColor = theme.Accent,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(8)
        };
        var prevTap = new TapGestureRecognizer();
        prevTap.Tapped += (_, _) => NavigateBack();
        prevButton.GestureRecognizers.Add(prevTap);

        var nextButton = new Label
        {
            Text = ">",
            FontSize = FluentTypography.BodySize,
            TextColor = theme.Accent,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(8)
        };
        var nextTap = new TapGestureRecognizer();
        nextTap.Tapped += (_, _) => NavigateForward();
        nextButton.GestureRecognizers.Add(nextTap);

        _header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Children = { prevButton, _headerLabel, nextButton }
        };
        Grid.SetColumn(prevButton, 0);
        Grid.SetColumn(_headerLabel, 1);
        Grid.SetColumn(nextButton, 2);

        _daysGrid = new Grid
        {
            ColumnSpacing = 4,
            RowSpacing = 4
        };

        _root = new VerticalStackLayout
        {
            Spacing = 8,
            Children = { _header, _daysGrid }
        };

        Content = _root;
        Rebuild();
    }

    private void NavigateBack()
    {
        DisplayDate = ViewMode switch
        {
            FlCalendarViewMode.Month => DisplayDate.AddMonths(-1),
            FlCalendarViewMode.Year => DisplayDate.AddYears(-1),
            FlCalendarViewMode.Decade => DisplayDate.AddYears(-10),
            _ => DisplayDate.AddMonths(-1)
        };
    }

    private void NavigateForward()
    {
        DisplayDate = ViewMode switch
        {
            FlCalendarViewMode.Month => DisplayDate.AddMonths(1),
            FlCalendarViewMode.Year => DisplayDate.AddYears(1),
            FlCalendarViewMode.Decade => DisplayDate.AddYears(10),
            _ => DisplayDate.AddMonths(1)
        };
    }

    private void Rebuild()
    {
        _daysGrid.Children.Clear();

        switch (ViewMode)
        {
            case FlCalendarViewMode.Month:
                BuildMonthView();
                break;
            case FlCalendarViewMode.Year:
                BuildYearView();
                break;
        }

        UpdateHeader();
    }

    private void UpdateHeader()
    {
        _headerLabel.Text = ViewMode switch
        {
            FlCalendarViewMode.Month => DisplayDate.ToString("MMMM yyyy"),
            FlCalendarViewMode.Year => DisplayDate.ToString("yyyy"),
            FlCalendarViewMode.Decade => $"{DisplayDate.Year - (DisplayDate.Year % 10)}-{DisplayDate.Year - (DisplayDate.Year % 10) + 9}",
            _ => DisplayDate.ToString("MMMM yyyy")
        };
    }

    private void BuildMonthView()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        // Day headers
        string[] dayNames = { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };
        for (int col = 0; col < 7; col++)
        {
            _daysGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            var header = new Label
            {
                Text = dayNames[col],
                FontSize = FluentTypography.CaptionSize,
                TextColor = theme.TextSecondary,
                HorizontalTextAlignment = TextAlignment.Center
            };
            _daysGrid.Children.Add(header);
            Grid.SetRow(header, 0);
            Grid.SetColumn(header, col);
        }

        // Days
        var firstDay = new DateTime(DisplayDate.Year, DisplayDate.Month, 1);
        int startDayOfWeek = (int)firstDay.DayOfWeek;
        int daysInMonth = DateTime.DaysInMonth(DisplayDate.Year, DisplayDate.Month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            int row = (startDayOfWeek + day - 1) / 7 + 1;
            int col = (startDayOfWeek + day - 1) % 7;

            var currentDate = new DateTime(DisplayDate.Year, DisplayDate.Month, day);
            var dayLabel = new Label
            {
                Text = day.ToString(),
                FontSize = FluentTypography.BodySize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = 32
            };

            var dayBorder = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Small) },
                Stroke = Colors.Transparent,
                Content = dayLabel,
                HeightRequest = 32
            };

            if (currentDate.Date == SelectedDate.Date)
            {
                dayBorder.BackgroundColor = theme.Accent;
                dayLabel.TextColor = theme.TextOnAccent;
            }
            else
            {
                dayLabel.TextColor = theme.TextPrimary;
            }

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => { SelectedDate = currentDate; DateSelected?.Invoke(this, currentDate); };
            dayBorder.GestureRecognizers.Add(tap);

            _daysGrid.Children.Add(dayBorder);
            Grid.SetRow(dayBorder, row);
            Grid.SetColumn(dayBorder, col);
        }

        _daysGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Header row
        for (int r = 0; r < 6; r++)
        {
            _daysGrid.RowDefinitions.Add(new RowDefinition(new GridLength(36)));
        }
    }

    private void BuildYearView()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        for (int i = 0; i < 12; i++)
        {
            int row = i / 4;
            int col = i % 4;

            if (_daysGrid.ColumnDefinitions.Count <= col)
                _daysGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            if (_daysGrid.RowDefinitions.Count <= row)
                _daysGrid.RowDefinitions.Add(new RowDefinition(new GridLength(48)));

            var monthLabel = new Label
            {
                Text = monthNames[i],
                FontSize = FluentTypography.BodySize,
                TextColor = theme.TextPrimary,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            var monthBorder = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Small) },
                Stroke = Colors.Transparent,
                Content = monthLabel,
                HeightRequest = 40
            };

            var month = i + 1;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                DisplayDate = new DateTime(DisplayDate.Year, month, 1);
                ViewMode = FlCalendarViewMode.Month;
            };
            monthBorder.GestureRecognizers.Add(tap);

            _daysGrid.Children.Add(monthBorder);
            Grid.SetRow(monthBorder, row);
            Grid.SetColumn(monthBorder, col);
        }
    }

    private void UpdateSelection()
    {
        Rebuild();
    }
}

public enum FlCalendarViewMode { Month, Year, Decade, Century }

/// <summary>
/// Fluent Design popup/dialog.
/// </summary>
/// <summary>
/// Fluent Design popup/dialog with shadows, animations, backdrop, and full data passing support.
/// Supports sending data to the popup, receiving data back, close events, and dismiss on backdrop tap.
/// </summary>
public class FlPopup : ContentView
{
    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(FlPopup), false,
            propertyChanged: (b, _, _) => ((FlPopup)b).ToggleVisibility());

    public static readonly BindableProperty PopupContentProperty =
        BindableProperty.Create(nameof(PopupContent), typeof(View), typeof(FlPopup), null,
            propertyChanged: (b, _, _) => ((FlPopup)b).UpdateContent());

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(FlPopup), string.Empty,
            propertyChanged: (b, _, _) => ((FlPopup)b).UpdateTitle());

    public static readonly BindableProperty DismissOnBackdropTapProperty =
        BindableProperty.Create(nameof(DismissOnBackdropTap), typeof(bool), typeof(FlPopup), true);

    public static readonly BindableProperty DataProperty =
        BindableProperty.Create(nameof(Data), typeof(object), typeof(FlPopup), null);

    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public View? PopupContent { get => (View?)GetValue(PopupContentProperty); set => SetValue(PopupContentProperty, value); }
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public bool DismissOnBackdropTap { get => (bool)GetValue(DismissOnBackdropTapProperty); set => SetValue(DismissOnBackdropTapProperty, value); }
    public object? Data { get => (object?)GetValue(DataProperty); set => SetValue(DataProperty, value); }

    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler<FlPopupClosedEventArgs>? ClosedWithResult;

    private readonly Border _popup;
    private readonly Label _titleLabel;
    private readonly VerticalStackLayout _contentStack;
    private readonly Grid _overlay;
    private TaskCompletionSource<FlPopupResult>? _tcs;

    public FlPopup()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _titleLabel = new Label
        {
            FontSize = FluentTypography.TitleSize,
            TextColor = theme.TextPrimary,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 12),
            IsVisible = false
        };

        _contentStack = new VerticalStackLayout
        {
            Spacing = 12,
            Children = { _titleLabel }
        };

        _popup = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Large) },
            Stroke = theme.BorderSubtle,
            StrokeThickness = 1,
            BackgroundColor = theme.Surface,
            Padding = new Thickness(24),
            Content = _contentStack,
            Shadow = FluentElevation.Dialog,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            MaximumWidthRequest = 400
        };

        _overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#00000000"),
            IsVisible = false,
            Children = { _popup }
        };

        // Backdrop tap
        var backdropTap = new TapGestureRecognizer();
        backdropTap.Tapped += OnBackdropTapped;
        _overlay.GestureRecognizers.Add(backdropTap);

        Content = _overlay;

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _popup.BackgroundColor = t.Surface;
            _popup.Stroke = t.BorderSubtle;
            _titleLabel.TextColor = t.TextPrimary;
        });
    }

    private void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        if (DismissOnBackdropTap)
        {
            Close(null);
        }
    }

    private void UpdateTitle()
    {
        _titleLabel.Text = Title;
        _titleLabel.IsVisible = !string.IsNullOrEmpty(Title);
    }

    private void UpdateContent()
    {
        if (PopupContent != null)
        {
            _contentStack.Children.Clear();
            _contentStack.Children.Add(_titleLabel);
            _contentStack.Children.Add(PopupContent);
        }
    }

    /// <summary>
    /// Shows the popup and waits for a result asynchronously.
    /// </summary>
    /// <param name="data">Optional data to send to the popup.</param>
    /// <returns>A task that completes when the popup is closed, returning the result.</returns>
    public Task<FlPopupResult> ShowAsync(object? data = null)
    {
        _tcs = new TaskCompletionSource<FlPopupResult>();
        Data = data;
        IsOpen = true;
        return _tcs.Task;
    }

    /// <summary>
    /// Closes the popup with an optional result.
    /// </summary>
    /// <param name="resultData">Optional result data to return.</param>
    public void Close(object? resultData = null)
    {
        var result = new FlPopupResult
        {
            Success = resultData != null,
            Data = resultData
        };

        ClosedWithResult?.Invoke(this, new FlPopupClosedEventArgs(result));
        _tcs?.TrySetResult(result);
        IsOpen = false;
    }

    /// <summary>
    /// Closes the popup with success result.
    /// </summary>
    /// <param name="data">Result data.</param>
    public void CloseWithSuccess(object? data = null)
    {
        var result = new FlPopupResult
        {
            Success = true,
            Data = data
        };

        ClosedWithResult?.Invoke(this, new FlPopupClosedEventArgs(result));
        _tcs?.TrySetResult(result);
        IsOpen = false;
    }

    /// <summary>
    /// Closes the popup with cancellation.
    /// </summary>
    public void Cancel()
    {
        var result = new FlPopupResult
        {
            Success = false,
            Data = null
        };

        ClosedWithResult?.Invoke(this, new FlPopupClosedEventArgs(result));
        _tcs?.TrySetResult(result);
        IsOpen = false;
    }

    private async void ToggleVisibility()
    {
        if (IsOpen)
        {
            _overlay.IsVisible = true;
            _popup.Scale = 0.85;
            _popup.Opacity = 0;
            _overlay.BackgroundColor = Color.FromArgb("#00000000");

            await Task.WhenAll(
                _popup.ScaleToAsync(1.0, FluentAnimations.Gentle, FluentAnimations.Entrance),
                _popup.FadeToAsync(1, FluentAnimations.Gentle),
                _overlay.FadeToAsync(1, FluentAnimations.Gentle)
            );

            _overlay.BackgroundColor = FluentThemeManager.Current.CurrentTheme.Scrim;
            Opened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            await Task.WhenAll(
                _popup.ScaleToAsync(0.85, FluentAnimations.Gentle, FluentAnimations.Exit),
                _popup.FadeToAsync(0, FluentAnimations.Gentle),
                _overlay.FadeToAsync(0, FluentAnimations.Gentle)
            );

            _overlay.IsVisible = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}

/// <summary>
/// Result data from a popup.
/// </summary>
public class FlPopupResult
{
    public bool Success { get; set; }
    public object? Data { get; set; }
}

/// <summary>
/// Event args for popup closed event.
/// </summary>
public class FlPopupClosedEventArgs : EventArgs
{
    public FlPopupResult Result { get; }

    public FlPopupClosedEventArgs(FlPopupResult result)
    {
        Result = result;
    }
}

/// <summary>
/// Fluent Design effects view for shadows, blur, and highlights.
/// </summary>
public class FlEffectsView : ContentView
{
    public static readonly BindableProperty EffectContentProperty =
        BindableProperty.Create(nameof(EffectContent), typeof(View), typeof(FlEffectsView), null,
            propertyChanged: (b, _, _) => ((FlEffectsView)b).UpdateContent());

    public static readonly BindableProperty ShadowLevelProperty =
        BindableProperty.Create(nameof(ShadowLevel), typeof(int), typeof(FlEffectsView), 0,
            propertyChanged: (b, _, _) => ((FlEffectsView)b).ApplyEffects());

    public View? EffectContent { get => (View?)GetValue(EffectContentProperty); set => SetValue(EffectContentProperty, value); }
    public int ShadowLevel { get => (int)GetValue(ShadowLevelProperty); set => SetValue(ShadowLevelProperty, value); }

    private readonly Border _border;

    public FlEffectsView()
    {
        _border = new Border
        {
            Stroke = Colors.Transparent,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Medium) }
        };

        Content = _border;
        ApplyEffects();
    }

    private void UpdateContent()
    {
        _border.Content = EffectContent;
    }

    private void ApplyEffects()
    {
        _border.Shadow = ShadowLevel switch
        {
            1 => FluentElevation.Level1,
            2 => FluentElevation.Level2,
            3 => FluentElevation.Level3,
            4 => FluentElevation.Level4,
            _ => null
        };
    }
}

/// <summary>
/// Fluent Design interactive viewer for zoom, pan, and rotate.
/// </summary>
public class FlInteractiveViewer : ContentView
{
    public static readonly BindableProperty ViewerContentProperty =
        BindableProperty.Create(nameof(ViewerContent), typeof(View), typeof(FlInteractiveViewer), null,
            propertyChanged: (b, _, _) => ((FlInteractiveViewer)b).UpdateContent());

    public static readonly BindableProperty MinZoomProperty =
        BindableProperty.Create(nameof(MinZoom), typeof(double), typeof(FlInteractiveViewer), 0.5);

    public static readonly BindableProperty MaxZoomProperty =
        BindableProperty.Create(nameof(MaxZoom), typeof(double), typeof(FlInteractiveViewer), 3.0);

    public View? ViewerContent { get => (View?)GetValue(ViewerContentProperty); set => SetValue(ViewerContentProperty, value); }
    public double MinZoom { get => (double)GetValue(MinZoomProperty); set => SetValue(MinZoomProperty, value); }
    public double MaxZoom { get => (double)GetValue(MaxZoomProperty); set => SetValue(MaxZoomProperty, value); }

    private readonly Grid _container;
    private double _currentScale = 1.0;
    private double _startScale = 1.0;
    private double _currentX = 0;
    private double _currentY = 0;
    private double _startX = 0;
    private double _startY = 0;

    public FlInteractiveViewer()
    {
        _container = new Grid();
        Content = _container;

        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;
        _container.GestureRecognizers.Add(pinchGesture);

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        _container.GestureRecognizers.Add(panGesture);
    }

    private void UpdateContent()
    {
        _container.Children.Clear();
        if (ViewerContent != null)
        {
            _container.Children.Add(ViewerContent);
        }
    }

    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                _startScale = _currentScale;
                break;
            case GestureStatus.Running:
                _currentScale = Math.Max(MinZoom, Math.Min(MaxZoom, _startScale * e.Scale));
                _container.Scale = _currentScale;
                break;
            case GestureStatus.Completed:
                break;
            case GestureStatus.Canceled:
                _currentScale = _startScale;
                _container.Scale = _currentScale;
                break;
        }
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startX = _currentX;
                _startY = _currentY;
                break;
            case GestureStatus.Running:
                _currentX = _startX + e.TotalX;
                _currentY = _startY + e.TotalY;
                _container.TranslationX = _currentX;
                _container.TranslationY = _currentY;
                break;
            case GestureStatus.Completed:
                break;
        }
    }

    public void ResetTransform()
    {
        _currentScale = 1.0;
        _currentX = 0;
        _currentY = 0;
        _container.Scale = 1.0;
        _container.TranslationX = 0;
        _container.TranslationY = 0;
    }
}
