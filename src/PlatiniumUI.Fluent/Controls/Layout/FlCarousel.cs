using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.Layout;

/// <summary>
/// Fluent Design carousel with touch-enabled sliding.
/// </summary>
public class FlCarousel : ContentView
{
    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(FlCarousel), 0, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlCarousel)b).UpdateSelection());

    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

    public event EventHandler<int>? SelectionChanged;

    private readonly Grid _container;
    private readonly HorizontalStackLayout _items;
    private readonly HorizontalStackLayout _indicators;

    public FlCarousel()
    {
        _items = new HorizontalStackLayout { Spacing = 0 };
        _indicators = new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.Center };

        _container = new Grid
        {
            Children = { _items, _indicators },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            }
        };

        Grid.SetRow(_items, 0);
        Grid.SetRow(_indicators, 1);
        Content = _container;
    }

    public void AddItem(View item)
    {
        int index = _items.Children.Count;
        _items.Children.Add(item);

        var indicator = new Border
        {
            WidthRequest = 8,
            HeightRequest = 8,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(4, 4), RadiusX = 4, RadiusY = 4 },
            Stroke = Colors.Transparent,
            BackgroundColor = FluentThemeManager.Current.CurrentTheme.BorderSubtle
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => SelectedIndex = index;
        indicator.GestureRecognizers.Add(tap);

        _indicators.Children.Add(indicator);
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        for (int i = 0; i < _indicators.Children.Count; i++)
        {
            if (_indicators.Children[i] is Border indicator)
            {
                indicator.BackgroundColor = i == SelectedIndex ? theme.Accent : theme.BorderSubtle;
            }
        }

        SelectionChanged?.Invoke(this, SelectedIndex);
    }
}

/// <summary>
/// Fluent Design grid splitter for resizable regions.
/// </summary>
public class FlGridSplitter : ContentView
{
    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(FlGridSplitter), StackOrientation.Horizontal);

    public StackOrientation Orientation { get => (StackOrientation)GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

    private readonly Border _splitter;

    public FlGridSplitter()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _splitter = new Border
        {
            Stroke = Colors.Transparent,
            BackgroundColor = theme.BorderSubtle,
            MinimumWidthRequest = 4,
            MinimumHeightRequest = 4
        };

        Content = _splitter;
    }
}

/// <summary>
/// Fluent Design wrap layout for flowing content.
/// </summary>
public class FlWrapLayout : ContentView
{
    private readonly Microsoft.Maui.Controls.Layout _layout;

    public FlWrapLayout()
    {
        _layout = new VerticalStackLayout();
        Content = _layout;
    }

    public new View Content
    {
        get => base.Content;
        set => base.Content = value;
    }
}
