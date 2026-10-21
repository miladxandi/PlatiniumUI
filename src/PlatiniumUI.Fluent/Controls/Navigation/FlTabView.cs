using System.Collections.ObjectModel;
using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Navigation;

/// <summary>
/// Fluent Design tab view with customizable tab bar.
/// </summary>
public class FlTabView : ContentView
{
    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(FlTabView), 0,
            propertyChanged: (b, _, _) => ((FlTabView)b).ApplySelection());

    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

    public event EventHandler<int>? SelectionChanged;

    private readonly ObservableCollection<FlTabItem> _tabs = new();
    private readonly Border _tabBar;
    private readonly HorizontalStackLayout _tabStack;
    private readonly ContentView _contentArea;
    private readonly VerticalStackLayout _root;

    public ObservableCollection<FlTabItem> Tabs => _tabs;

    public FlTabView()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _tabStack = new HorizontalStackLayout { Spacing = 0 };
        _tabBar = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(0) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Surface,
            Content = _tabStack
        };

        _contentArea = new ContentView();

        _root = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { _tabBar, _contentArea }
        };

        Content = _root;
        _tabs.CollectionChanged += (_, _) => Rebuild();

        this.ObserveThemeChanged(() => { _tabBar.BackgroundColor = FluentThemeManager.Current.CurrentTheme.Surface; ApplySelection(); });
    }

    private void Rebuild()
    {
        _tabStack.Children.Clear();
        var theme = FluentThemeManager.Current.CurrentTheme;

        for (int i = 0; i < _tabs.Count; i++)
        {
            int index = i;
            var tab = _tabs[i];

            var label = new Label
            {
                Text = tab.Title,
                FontSize = FluentTypography.BodySize,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var indicator = new Border
            {
                HeightRequest = 3,
                Stroke = Colors.Transparent,
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(16, 0)
            };

            var tabItem = new VerticalStackLayout
            {
                Spacing = 4,
                Padding = new Thickness(16, 12),
                Children = { label, indicator }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => { SelectedIndex = index; SelectionChanged?.Invoke(this, index); };
            tabItem.GestureRecognizers.Add(tap);

            _tabStack.Children.Add(tabItem);
        }

        ApplySelection();
    }

    private void ApplySelection()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        for (int i = 0; i < _tabStack.Children.Count; i++)
        {
            if (_tabStack.Children[i] is VerticalStackLayout tabLayout)
            {
                var label = tabLayout.Children.OfType<Label>().FirstOrDefault();
                var indicator = tabLayout.Children.OfType<Border>().LastOrDefault();

                if (i == SelectedIndex)
                {
                    label?.SetValue(Label.TextColorProperty, theme.Accent);
                    if (indicator != null) indicator.BackgroundColor = theme.Accent;
                    if (i < _tabs.Count) _contentArea.Content = _tabs[i].Content;
                }
                else
                {
                    label?.SetValue(Label.TextColorProperty, theme.TextSecondary);
                    if (indicator != null) indicator.BackgroundColor = Colors.Transparent;
                }
            }
        }
    }
}

public class FlTabItem
{
    public string Title { get; set; } = string.Empty;
    public View? Content { get; set; }
}

/// <summary>
/// Fluent Design bottom sheet with full drag gesture support and smooth open/close animation.
/// Supports velocity-based snap decisions, threshold-based close, and real-time drag following.
/// </summary>
public class FlBottomSheet : ContentView
{
    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(FlBottomSheet), false,
            propertyChanged: (b, _, _) => ((FlBottomSheet)b).AnimateSheet());

    public static readonly BindableProperty SheetContentProperty =
        BindableProperty.Create(nameof(SheetContent), typeof(View), typeof(FlBottomSheet), null,
            propertyChanged: (b, _, _) => ((FlBottomSheet)b).UpdateContent());

    public static readonly BindableProperty CloseOnBackdropTapProperty =
        BindableProperty.Create(nameof(CloseOnBackdropTap), typeof(bool), typeof(FlBottomSheet), true);

    public static readonly BindableProperty CloseThresholdProperty =
        BindableProperty.Create(nameof(CloseThreshold), typeof(double), typeof(FlBottomSheet), 0.3);

    public static readonly BindableProperty VelocityThresholdProperty =
        BindableProperty.Create(nameof(VelocityThreshold), typeof(double), typeof(FlBottomSheet), 500.0);

    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public View? SheetContent { get => (View?)GetValue(SheetContentProperty); set => SetValue(SheetContentProperty, value); }
    public bool CloseOnBackdropTap { get => (bool)GetValue(CloseOnBackdropTapProperty); set => SetValue(CloseOnBackdropTapProperty, value); }
    public double CloseThreshold { get => (double)GetValue(CloseThresholdProperty); set => SetValue(CloseThresholdProperty, value); }
    public double VelocityThreshold { get => (double)GetValue(VelocityThresholdProperty); set => SetValue(VelocityThresholdProperty, value); }

    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler<FlBottomSheetDragState>? DragStateChanged;

    private readonly Border _sheet;
    private readonly Border _handle;
    private readonly VerticalStackLayout _sheetLayout;
    private readonly Grid _overlay;
    private double _sheetHeight;
    private bool _isDragging;

    public FlBottomSheet()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _handle = new Border
        {
            WidthRequest = 32,
            HeightRequest = 4,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(2) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.BorderStrong,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 8, 0, 8)
        };

        _sheetLayout = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { _handle }
        };

        _sheet = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.BottomSheet, FluentCornerRadius.BottomSheet, 0, 0)
            },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Surface,
            Padding = new Thickness(16, 0, 16, 16),
            Content = _sheetLayout,
            VerticalOptions = LayoutOptions.End,
            Shadow = FluentElevation.BottomSheet
        };

        _overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#00000000"),
            IsVisible = false,
            Children = { _sheet }
        };

        // Backdrop tap to close
        var scrimTap = new TapGestureRecognizer();
        scrimTap.Tapped += (_, _) => { if (CloseOnBackdropTap) IsOpen = false; };
        _overlay.GestureRecognizers.Add(scrimTap);

        // Drag gesture on the sheet
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnSheetPanUpdated;
        _sheet.GestureRecognizers.Add(panGesture);

        // Also add drag on handle specifically for better UX
        var handlePan = new PanGestureRecognizer();
        handlePan.PanUpdated += OnSheetPanUpdated;
        _handle.GestureRecognizers.Add(handlePan);

        Content = _overlay;

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _sheet.BackgroundColor = t.Surface;
            _handle.BackgroundColor = t.BorderStrong;
        });
    }

    private void UpdateContent()
    {
        if (SheetContent != null)
        {
            _sheetLayout.Children.Clear();
            _sheetLayout.Children.Add(_handle);
            _sheetLayout.Children.Add(SheetContent);
        }
    }

    private void OnSheetPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                OnDragStarted();
                break;
            case GestureStatus.Running:
                OnDragRunning(e.TotalY);
                break;
            case GestureStatus.Completed:
                OnDragCompleted(e.TotalY);
                break;
        }
    }

    private void OnDragStarted()
    {
        _isDragging = true;
        _sheetHeight = _sheet.Height > 0 ? _sheet.Height : 400;
        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Dragging);
    }

    private void OnDragRunning(double totalY)
    {
        if (!_isDragging) return;

        // Bidirectional drag support
        double targetY;
        double progress;

        if (IsOpen)
        {
            // When open: dragging down (positive Y) moves toward close
            targetY = Math.Max(0, totalY);
            progress = 1.0 - (targetY / _sheetHeight);
        }
        else
        {
            // When closed: dragging up (negative Y) moves toward open
            targetY = Math.Min(0, totalY);
            progress = Math.Abs(targetY) / _sheetHeight;
        }

        progress = Math.Max(0, Math.Min(1, progress));

        // Real-time drag following
        _sheet.TranslationY = IsOpen ? targetY : (_sheetHeight + targetY);

        // Fade backdrop proportionally
        var theme = FluentThemeManager.Current.CurrentTheme;
        byte alpha = (byte)(progress * 255);
        _overlay.BackgroundColor = Color.FromArgb($"#{alpha:X2}000000");

        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Dragging);
    }

    private async void OnDragCompleted(double totalY)
    {
        if (!_isDragging) return;
        _isDragging = false;

        double dragDistance = Math.Abs(totalY);
        double dragRatio = _sheetHeight > 0 ? dragDistance / _sheetHeight : 0;

        if (IsOpen)
        {
            // Currently open: decide whether to close or snap back
            bool shouldClose = totalY > 0 && dragRatio > CloseThreshold;
            
            if (shouldClose)
            {
                await AnimateClose();
            }
            else
            {
                await AnimateSnapBack();
            }
        }
        else
        {
            // Currently closed: decide whether to open or snap back
            bool shouldOpen = totalY < 0 && dragRatio > CloseThreshold;
            
            if (shouldOpen)
            {
                await AnimateOpen();
            }
            else
            {
                await AnimateSnapToClosed();
            }
        }
    }

    private async Task AnimateClose()
    {
        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Closing);

        await Task.WhenAll(
            _sheet.TranslateToAsync(0, _sheetHeight, FluentAnimations.Gentle, FluentAnimations.Exit),
            _overlay.FadeToAsync(0, FluentAnimations.Gentle)
        );

        _overlay.IsVisible = false;
        _sheet.TranslationY = 0;

        // Force-set IsOpen to false without re-triggering AnimateSheet
        SetValue(IsOpenProperty, false);
        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Closed);
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private async Task AnimateSnapBack()
    {
        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Snapping);

        await _sheet.TranslateToAsync(0, 0, FluentAnimations.Gentle, FluentAnimations.Entrance);

        var theme = FluentThemeManager.Current.CurrentTheme;
        _overlay.BackgroundColor = theme.Scrim;

        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Open);
    }

    private async Task AnimateOpen()
    {
        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Opening);

        _overlay.IsVisible = true;

        await Task.WhenAll(
            _sheet.TranslateToAsync(0, 0, FluentAnimations.Gentle, FluentAnimations.Entrance),
            _overlay.FadeToAsync(1, FluentAnimations.Gentle)
        );

        var theme = FluentThemeManager.Current.CurrentTheme;
        _overlay.BackgroundColor = theme.Scrim;

        // Force-set IsOpen to true without re-triggering AnimateSheet
        SetValue(IsOpenProperty, true);
        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Open);
        Opened?.Invoke(this, EventArgs.Empty);
    }

    private async Task AnimateSnapToClosed()
    {
        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Snapping);

        await _sheet.TranslateToAsync(0, _sheetHeight, FluentAnimations.Gentle, FluentAnimations.Exit);
        await _overlay.FadeToAsync(0, FluentAnimations.Gentle);

        _overlay.IsVisible = false;
        _sheet.TranslationY = 0;

        DragStateChanged?.Invoke(this, FlBottomSheetDragState.Closed);
    }

    private async void AnimateSheet()
    {
        if (_isDragging) return; // Don't interfere with active drag

        if (IsOpen)
        {
            _sheetHeight = _sheet.Height > 0 ? _sheet.Height : 400;
            _overlay.IsVisible = true;
            _sheet.TranslationY = _sheetHeight;
            _overlay.BackgroundColor = Color.FromArgb("#00000000");

            DragStateChanged?.Invoke(this, FlBottomSheetDragState.Opening);

            await Task.WhenAll(
                _sheet.TranslateToAsync(0, 0, FluentAnimations.Gentle, FluentAnimations.Entrance),
                _overlay.FadeToAsync(1, FluentAnimations.Gentle)
            );

            _overlay.BackgroundColor = FluentThemeManager.Current.CurrentTheme.Scrim;
            DragStateChanged?.Invoke(this, FlBottomSheetDragState.Open);
            Opened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (_overlay.IsVisible)
            {
                DragStateChanged?.Invoke(this, FlBottomSheetDragState.Closing);

                await Task.WhenAll(
                    _sheet.TranslateToAsync(0, _sheetHeight > 0 ? _sheetHeight : 400, FluentAnimations.Gentle, FluentAnimations.Exit),
                    _overlay.FadeToAsync(0, FluentAnimations.Gentle)
                );

                _overlay.IsVisible = false;
                _sheet.TranslationY = 0;
                DragStateChanged?.Invoke(this, FlBottomSheetDragState.Closed);
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

/// <summary>
/// Drag state of the bottom sheet.
/// </summary>
public enum FlBottomSheetDragState
{
    Open,
    Closed,
    Opening,
    Closing,
    Dragging,
    Snapping
}

/// <summary>
/// Fluent Design breadcrumb navigation.
/// </summary>
public class FlBreadcrumb : ContentView
{
    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(IList<string>), typeof(FlBreadcrumb), null,
            propertyChanged: (b, _, _) => ((FlBreadcrumb)b).Rebuild());

    public IList<string>? Items { get => (IList<string>?)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

    public event EventHandler<int>? ItemTapped;

    private readonly HorizontalStackLayout _stack;

    public FlBreadcrumb()
    {
        _stack = new HorizontalStackLayout { Spacing = 4 };
        Content = _stack;
    }

    private void Rebuild()
    {
        _stack.Children.Clear();
        if (Items == null) return;
        var theme = FluentThemeManager.Current.CurrentTheme;

        for (int i = 0; i < Items.Count; i++)
        {
            int index = i;
            bool isLast = i == Items.Count - 1;

            var label = new Label
            {
                Text = Items[i],
                FontSize = FluentTypography.BodySize,
                TextColor = isLast ? theme.TextPrimary : theme.Accent
            };

            if (!isLast)
            {
                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, _) => ItemTapped?.Invoke(this, index);
                label.GestureRecognizers.Add(tap);
            }

            _stack.Children.Add(label);

            if (!isLast)
            {
                _stack.Children.Add(new Label
                {
                    Text = "/",
                    FontSize = FluentTypography.BodySize,
                    TextColor = theme.TextTertiary
                });
            }
        }
    }
}

/// <summary>
/// Fluent Design pagination control.
/// </summary>
public class FlPagination : ContentView
{
    public static readonly BindableProperty CurrentPageProperty =
        BindableProperty.Create(nameof(CurrentPage), typeof(int), typeof(FlPagination), 1, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlPagination)b).Rebuild());

    public static readonly BindableProperty TotalPagesProperty =
        BindableProperty.Create(nameof(TotalPages), typeof(int), typeof(FlPagination), 1,
            propertyChanged: (b, _, _) => ((FlPagination)b).Rebuild());

    public int CurrentPage { get => (int)GetValue(CurrentPageProperty); set => SetValue(CurrentPageProperty, value); }
    public int TotalPages { get => (int)GetValue(TotalPagesProperty); set => SetValue(TotalPagesProperty, value); }

    public event EventHandler<int>? PageChanged;

    private readonly HorizontalStackLayout _stack;

    public FlPagination()
    {
        _stack = new HorizontalStackLayout { Spacing = 4 };
        Content = _stack;
        Rebuild();
    }

    private void Rebuild()
    {
        _stack.Children.Clear();
        var theme = FluentThemeManager.Current.CurrentTheme;

        AddPageButton("<", CurrentPage > 1, CurrentPage - 1);

        for (int i = 1; i <= TotalPages && i <= 7; i++)
        {
            AddPageButton(i.ToString(), true, i, i == CurrentPage);
        }

        AddPageButton(">", CurrentPage < TotalPages, CurrentPage + 1);
    }

    private void AddPageButton(string text, bool enabled, int targetPage, bool isSelected = false)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        var label = new Label
        {
            Text = text,
            FontSize = 14,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            TextColor = isSelected ? theme.TextOnAccent : (enabled ? theme.TextPrimary : theme.TextDisabled)
        };

        var border = new Border
        {
            WidthRequest = 36,
            HeightRequest = 36,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Small) },
            Stroke = Colors.Transparent,
            BackgroundColor = isSelected ? theme.Accent : Colors.Transparent,
            Content = label,
            VerticalOptions = LayoutOptions.Center
        };

        if (enabled)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => { CurrentPage = targetPage; PageChanged?.Invoke(this, targetPage); };
            border.GestureRecognizers.Add(tap);
        }

        _stack.Children.Add(border);
    }
}
