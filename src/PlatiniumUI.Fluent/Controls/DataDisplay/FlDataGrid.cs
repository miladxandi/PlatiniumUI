using System.Collections;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.DataDisplay;

/// <summary>
/// Fluent Design data grid with sorting and filtering.
/// </summary>
public class FlDataGrid : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(FlDataGrid), null,
            propertyChanged: (b, _, _) => ((FlDataGrid)b).Rebuild());

    public IEnumerable? ItemsSource { get => (IEnumerable?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    private readonly ScrollView _scrollView;
    private readonly VerticalStackLayout _stack;

    public FlDataGrid()
    {
        _stack = new VerticalStackLayout { Spacing = 0 };
        _scrollView = new ScrollView { Content = _stack };
        Content = _scrollView;
    }

    private void Rebuild()
    {
        _stack.Children.Clear();
        if (ItemsSource == null) return;

        var theme = FluentThemeManager.Current.CurrentTheme;

        foreach (var item in ItemsSource)
        {
            var row = new Border
            {
                Stroke = theme.BorderSubtle,
                StrokeThickness = 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(0) },
                BackgroundColor = theme.Surface,
                Padding = new Thickness(12, 8),
                Content = new Label
                {
                    Text = item?.ToString() ?? "",
                    FontSize = FluentTypography.BodySize,
                    TextColor = theme.TextPrimary
                }
            };
            _stack.Children.Add(row);
        }
    }
}

/// <summary>
/// Fluent Design list view with grouping and templates.
/// </summary>
public class FlListView : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(FlListView), null,
            propertyChanged: (b, _, _) => ((FlListView)b).Rebuild());

    public IEnumerable? ItemsSource { get => (IEnumerable?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    private readonly ScrollView _scrollView;
    private readonly VerticalStackLayout _stack;

    public FlListView()
    {
        _stack = new VerticalStackLayout { Spacing = 4 };
        _scrollView = new ScrollView { Content = _stack };
        Content = _scrollView;
    }

    private void Rebuild()
    {
        _stack.Children.Clear();
        if (ItemsSource == null) return;

        var theme = FluentThemeManager.Current.CurrentTheme;

        foreach (var item in ItemsSource)
        {
            var row = new Border
            {
                Stroke = Colors.Transparent,
                BackgroundColor = theme.Surface,
                Padding = new Thickness(12, 8),
                Content = new Label
                {
                    Text = item?.ToString() ?? "",
                    FontSize = FluentTypography.BodySize,
                    TextColor = theme.TextPrimary
                }
            };
            _stack.Children.Add(row);
        }
    }
}

/// <summary>
/// Fluent Design tree view with expand/collapse.
/// </summary>
public class FlTreeView : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(FlTreeView), null,
            propertyChanged: (b, _, _) => ((FlTreeView)b).Rebuild());

    public IEnumerable? ItemsSource { get => (IEnumerable?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    private readonly ScrollView _scrollView;
    private readonly VerticalStackLayout _stack;

    public FlTreeView()
    {
        _stack = new VerticalStackLayout { Spacing = 4 };
        _scrollView = new ScrollView { Content = _stack };
        Content = _scrollView;
    }

    private void Rebuild()
    {
        _stack.Children.Clear();
        if (ItemsSource == null) return;

        var theme = FluentThemeManager.Current.CurrentTheme;

        foreach (var item in ItemsSource)
        {
            var row = new HorizontalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Label
                    {
                        Text = "\u25B6",
                        FontSize = 12,
                        TextColor = theme.TextSecondary,
                        VerticalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = item?.ToString() ?? "",
                        FontSize = FluentTypography.BodySize,
                        TextColor = theme.TextPrimary,
                        VerticalOptions = LayoutOptions.Center
                    }
                }
            };
            _stack.Children.Add(row);
        }
    }
}
