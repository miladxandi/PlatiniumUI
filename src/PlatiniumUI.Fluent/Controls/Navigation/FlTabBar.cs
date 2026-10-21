using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;
using PlatiniumUI.Fluent.DesignSystem.Platform;
using PlatiniumUI.Fluent.DesignSystem.Icons;

namespace PlatiniumUI.Fluent.Controls.Navigation;

/// <summary>
/// Tab bar position options.
/// </summary>
public enum FlTabBarPosition
{
    Top,
    Bottom
}

/// <summary>
/// Tab bar indicator style options.
/// </summary>
public enum FlTabBarIndicatorStyle
{
    Pill,
    Underline,
    Background
}

/// <summary>
/// Represents a single tab item in the FlTabBar.
/// </summary>
public class FlTabBarItem : BindableObject
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(FlTabBarItem), string.Empty);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(FlTabBarItem), null);

    public static readonly BindableProperty BadgeTextProperty =
        BindableProperty.Create(nameof(BadgeText), typeof(string), typeof(FlTabBarItem), null);

    public static readonly BindableProperty BadgeColorProperty =
        BindableProperty.Create(nameof(BadgeColor), typeof(Color), typeof(FlTabBarItem), null);

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(View), typeof(FlTabBarItem), null);

    public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(FlTabBarItem), true);

    public static readonly BindableProperty TagProperty =
        BindableProperty.Create(nameof(Tag), typeof(object), typeof(FlTabBarItem), null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string? BadgeText
    {
        get => (string?)GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    public Color? BadgeColor
    {
        get => (Color?)GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
    }

    public View? Content
    {
        get => (View?)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public object? Tag
    {
        get => GetValue(TagProperty);
        set => SetValue(TagProperty, value);
    }
}

/// <summary>
/// Modern customizable tab bar control that can replace Maui.Shell.
/// Supports top/bottom positioning, icons, badges, animated indicators, and multiple indicator styles.
/// </summary>
public class FlTabBar : ContentView
{
    // ─── Bindable Properties ────────────────────────────────────────────────

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(ObservableCollection<FlTabBarItem>), typeof(FlTabBar), null,
            propertyChanged: OnItemsChanged);

    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(FlTabBar), 0,
            BindingMode.TwoWay, propertyChanged: OnSelectedIndexChanged);

    public static readonly BindableProperty TabBarPositionProperty =
        BindableProperty.Create(nameof(TabBarPosition), typeof(FlTabBarPosition), typeof(FlTabBar), FlTabBarPosition.Bottom,
            propertyChanged: OnLayoutPropertyChanged);

    public static readonly BindableProperty IndicatorStyleProperty =
        BindableProperty.Create(nameof(IndicatorStyle), typeof(FlTabBarIndicatorStyle), typeof(FlTabBar), FlTabBarIndicatorStyle.Pill,
            propertyChanged: OnLayoutPropertyChanged);

    public static readonly BindableProperty IndicatorColorProperty =
        BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(FlTabBar), null,
            propertyChanged: OnLayoutPropertyChanged);

    public static readonly BindableProperty ShowIconsProperty =
        BindableProperty.Create(nameof(ShowIcons), typeof(bool), typeof(FlTabBar), true,
            propertyChanged: OnLayoutPropertyChanged);

    public static readonly BindableProperty ShowLabelsProperty =
        BindableProperty.Create(nameof(ShowLabels), typeof(bool), typeof(FlTabBar), true,
            propertyChanged: OnLayoutPropertyChanged);

    public static readonly BindableProperty IsScrollableProperty =
        BindableProperty.Create(nameof(IsScrollable), typeof(bool), typeof(FlTabBar), false,
            propertyChanged: OnLayoutPropertyChanged);

    public static readonly BindableProperty TabSpacingProperty =
        BindableProperty.Create(nameof(TabSpacing), typeof(double), typeof(FlTabBar), FluentSpacing.M,
            propertyChanged: OnLayoutPropertyChanged);

    // ─── Events ─────────────────────────────────────────────────────────────

    public event EventHandler<int>? SelectionChanged;
    public event EventHandler<int>? TabTapped;

    // ─── Properties ─────────────────────────────────────────────────────────

    public ObservableCollection<FlTabBarItem>? Items
    {
        get => (ObservableCollection<FlTabBarItem>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public FlTabBarPosition TabBarPosition
    {
        get => (FlTabBarPosition)GetValue(TabBarPositionProperty);
        set => SetValue(TabBarPositionProperty, value);
    }

    public FlTabBarIndicatorStyle IndicatorStyle
    {
        get => (FlTabBarIndicatorStyle)GetValue(IndicatorStyleProperty);
        set => SetValue(IndicatorStyleProperty, value);
    }

    public Color? IndicatorColor
    {
        get => (Color?)GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    public bool ShowIcons
    {
        get => (bool)GetValue(ShowIconsProperty);
        set => SetValue(ShowIconsProperty, value);
    }

    public bool ShowLabels
    {
        get => (bool)GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    public bool IsScrollable
    {
        get => (bool)GetValue(IsScrollableProperty);
        set => SetValue(IsScrollableProperty, value);
    }

    public double TabSpacing
    {
        get => (double)GetValue(TabSpacingProperty);
        set => SetValue(TabSpacingProperty, value);
    }

    // ─── Private Fields ─────────────────────────────────────────────────────

    private readonly Grid _root;
    private readonly ContentView _tabBarContainer;
    private readonly ContentView _contentContainer;
    private readonly Border _indicator;
    private readonly HorizontalStackLayout _tabStrip;
    private readonly List<View> _tabViews = new();

    // ─── Constructor ────────────────────────────────────────────────────────

    public FlTabBar()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _tabStrip = new HorizontalStackLayout
        {
            Spacing = TabSpacing,
            HorizontalOptions = LayoutOptions.Center
        };

        _indicator = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Small)
            },
            Stroke = Colors.Transparent,
            BackgroundColor = IndicatorColor ?? theme.Accent,
            HeightRequest = 3,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.End
        };

        _tabBarContainer = new ContentView
        {
            Content = _tabStrip,
            BackgroundColor = theme.Surface,
            Padding = new Thickness(FluentSpacing.S, 0)
        };

        _contentContainer = new ContentView
        {
            BackgroundColor = theme.Background
        };

        _root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }
            },
            Children = { _tabBarContainer, _contentContainer }
        };

        Content = _root;

        // Subscribe to theme changes
        this.ObserveThemeChanged(() => OnThemeChanged());
    }

    // ─── Property Changed Handlers ──────────────────────────────────────────

    private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var tabBar = (FlTabBar)bindable;
        
        if (oldValue is ObservableCollection<FlTabBarItem> oldCollection)
        {
            oldCollection.CollectionChanged -= tabBar.OnCollectionChanged;
        }

        if (newValue is ObservableCollection<FlTabBarItem> newCollection)
        {
            newCollection.CollectionChanged += tabBar.OnCollectionChanged;
        }

        tabBar.RebuildTabs();
    }

    private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var tabBar = (FlTabBar)bindable;
        tabBar.UpdateSelection();
    }

    private static void OnLayoutPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var tabBar = (FlTabBar)bindable;
        tabBar.ApplyLayout();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildTabs();
    }

    private void OnThemeChanged()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _tabBarContainer.BackgroundColor = theme.Surface;
        _contentContainer.BackgroundColor = theme.Background;
        _indicator.BackgroundColor = IndicatorColor ?? theme.Accent;
    }

    // ─── Tab Building ───────────────────────────────────────────────────────

    private void RebuildTabs()
    {
        _tabStrip.Children.Clear();
        _tabViews.Clear();

        if (Items == null || Items.Count == 0)
            return;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var tabView = CreateTabView(item, i);
            _tabStrip.Children.Add(tabView);
            _tabViews.Add(tabView);
        }

        ApplyLayout();
        UpdateSelection();
    }

    private View CreateTabView(FlTabBarItem item, int index)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        var content = new VerticalStackLayout
        {
            Spacing = FluentSpacing.XS,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(FluentSpacing.M, FluentSpacing.S)
        };

        if (ShowIcons && !string.IsNullOrEmpty(item.Icon))
        {
            var iconLabel = new Label
            {
                Text = item.Icon,
                FontFamily = FluentIcons.FontFamily,
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = theme.TextSecondary
            };
            content.Children.Add(iconLabel);
        }

        if (ShowLabels && !string.IsNullOrEmpty(item.Title))
        {
            var titleLabel = new Label
            {
                Text = item.Title,
                FontSize = FluentTypography.CaptionSize,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = theme.TextSecondary
            };
            content.Children.Add(titleLabel);
        }

        if (!string.IsNullOrEmpty(item.BadgeText))
        {
            var badge = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(10)
                },
                Stroke = Colors.Transparent,
                BackgroundColor = item.BadgeColor ?? theme.Error,
                Padding = new Thickness(6, 2),
                HorizontalOptions = LayoutOptions.End,
                Content = new Label
                {
                    Text = item.BadgeText,
                    FontSize = 10,
                    TextColor = Colors.White
                }
            };
            content.Children.Add(badge);
        }

        var container = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Small)
            },
            Stroke = Colors.Transparent,
            BackgroundColor = Colors.Transparent,
            Content = content,
            StyleId = index.ToString()
        };

        // Add tap gesture
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => OnTabTapped(index);
        container.GestureRecognizers.Add(tapGesture);

        // Apply ripple effect
        FluentRippleEffect.Apply(container);

        return container;
    }

    private void OnTabTapped(int index)
    {
        if (Items == null || index < 0 || index >= Items.Count)
            return;

        var item = Items[index];
        if (!item.IsEnabled)
            return;

        TabTapped?.Invoke(this, index);
        SelectedIndex = index;
    }

    // ─── Selection ──────────────────────────────────────────────────────────

    private void UpdateSelection()
    {
        if (Items == null || Items.Count == 0 || _tabViews.Count == 0)
            return;

        var theme = FluentThemeManager.Current.CurrentTheme;
        var selectedIndex = SelectedIndex;

        // Update tab appearance
        for (int i = 0; i < _tabViews.Count; i++)
        {
            var tabView = _tabViews[i] as Border;
            if (tabView?.Content is VerticalStackLayout stack)
            {
                var isSelected = i == selectedIndex;
                var textColor = isSelected ? theme.Accent : theme.TextSecondary;

                foreach (var child in stack.Children)
                {
                    if (child is Label label)
                    {
                        label.TextColor = textColor;
                    }
                }

                // Apply background for Background indicator style
                if (IndicatorStyle == FlTabBarIndicatorStyle.Background && isSelected)
                {
                    tabView.BackgroundColor = (IndicatorColor ?? theme.Accent).WithAlpha(0.1f);
                }
                else
                {
                    tabView.BackgroundColor = Colors.Transparent;
                }
            }
        }

        // Update content
        if (selectedIndex >= 0 && selectedIndex < Items.Count)
        {
            var selectedItem = Items[selectedIndex];
            _contentContainer.Content = selectedItem.Content;
        }

        // Animate indicator
        AnimateIndicator();

        SelectionChanged?.Invoke(this, selectedIndex);
    }

    private void AnimateIndicator()
    {
        if (IndicatorStyle == FlTabBarIndicatorStyle.Background || _tabViews.Count == 0)
            return;

        var selectedIndex = SelectedIndex;
        if (selectedIndex < 0 || selectedIndex >= _tabViews.Count)
            return;

        var selectedTab = _tabViews[selectedIndex] as Border;
        if (selectedTab == null)
            return;

        // Calculate target position
        var targetX = selectedTab.X;
        var targetWidth = selectedTab.Width;

        if (targetWidth <= 0)
        {
            // Tab not measured yet, defer animation
            selectedTab.SizeChanged += OnTabSizeChanged;
            return;
        }

        // Animate indicator
        if (IndicatorStyle == FlTabBarIndicatorStyle.Pill || IndicatorStyle == FlTabBarIndicatorStyle.Underline)
        {
            _indicator.WidthRequest = targetWidth;
            _indicator.TranslateToAsync(targetX, 0, FluentAnimations.TabSwitchDuration, FluentAnimations.Standard);
        }
    }

    private void OnTabSizeChanged(object? sender, EventArgs e)
    {
        if (sender is View tab)
        {
            tab.SizeChanged -= OnTabSizeChanged;
            AnimateIndicator();
        }
    }

    // ─── Layout ─────────────────────────────────────────────────────────────

    private void ApplyLayout()
    {
        // Update tab strip spacing
        _tabStrip.Spacing = TabSpacing;

        // Update indicator appearance
        var theme = FluentThemeManager.Current.CurrentTheme;
        _indicator.BackgroundColor = IndicatorColor ?? theme.Accent;

        switch (IndicatorStyle)
        {
            case FlTabBarIndicatorStyle.Pill:
                _indicator.HeightRequest = 3;
                _indicator.IsVisible = true;
                break;
            case FlTabBarIndicatorStyle.Underline:
                _indicator.HeightRequest = 2;
                _indicator.IsVisible = true;
                break;
            case FlTabBarIndicatorStyle.Background:
                _indicator.IsVisible = false;
                break;
        }

        // Update root layout based on position
        _root.Children.Clear();
        _root.RowDefinitions.Clear();

        if (TabBarPosition == FlTabBarPosition.Top)
        {
            _root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            Grid.SetRow(_tabBarContainer, 0);
            Grid.SetRow(_contentContainer, 1);
        }
        else // Bottom
        {
            _root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            _root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(_contentContainer, 0);
            Grid.SetRow(_tabBarContainer, 1);
        }

        _root.Children.Add(_tabBarContainer);
        _root.Children.Add(_contentContainer);

        // Add indicator to tab bar
        if (IndicatorStyle != FlTabBarIndicatorStyle.Background)
        {
            var tabBarGrid = new Grid
            {
                Children = { _tabStrip, _indicator }
            };
            _tabBarContainer.Content = tabBarGrid;
        }
        else
        {
            _tabBarContainer.Content = _tabStrip;
        }
    }
}
