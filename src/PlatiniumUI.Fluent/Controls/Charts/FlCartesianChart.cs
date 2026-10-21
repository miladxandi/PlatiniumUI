using System.Collections.ObjectModel;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.Charts;

/// <summary>
/// Fluent Design cartesian chart supporting Line, Bar, Area, and Column types.
/// </summary>
public class FlCartesianChart : ContentView
{
    public static readonly BindableProperty ChartTypeProperty =
        BindableProperty.Create(nameof(ChartType), typeof(FlCartesianChartType), typeof(FlCartesianChart), FlCartesianChartType.Bar,
            propertyChanged: (b, _, _) => ((FlCartesianChart)b).Redraw());

    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(nameof(Series), typeof(ObservableCollection<FlChartSeries>), typeof(FlCartesianChart), null,
            propertyChanged: (b, _, _) => ((FlCartesianChart)b).Redraw());

    public FlCartesianChartType ChartType { get => (FlCartesianChartType)GetValue(ChartTypeProperty); set => SetValue(ChartTypeProperty, value); }
    public ObservableCollection<FlChartSeries>? Series { get => (ObservableCollection<FlChartSeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }

    private readonly Grid _canvas;
    private readonly Label _title;
    private readonly VerticalStackLayout _root;

    public FlCartesianChart()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _title = new Label
        {
            FontSize = FluentTypography.SubtitleSize,
            TextColor = theme.TextPrimary,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 8)
        };

        _canvas = new Grid
        {
            HeightRequest = 200,
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = theme.Surface
        };

        _root = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { _title, _canvas }
        };

        Content = _root;
    }

    private void Redraw()
    {
        _canvas.Children.Clear();
        if (Series == null || Series.Count == 0) return;

        var theme = FluentThemeManager.Current.CurrentTheme;
        var colors = new[] { theme.Accent, theme.Success, theme.Warning, theme.Error, theme.Info };

        switch (ChartType)
        {
            case FlCartesianChartType.Bar:
            case FlCartesianChartType.Column:
                DrawBars(colors, theme);
                break;
            case FlCartesianChartType.Line:
            case FlCartesianChartType.Area:
                DrawLines(colors, theme);
                break;
        }
    }

    private void DrawBars(Color[] colors, IFluentTheme theme)
    {
        var maxVal = Series!.Max(s => s.Values.Max());
        if (maxVal == 0) maxVal = 1;

        int totalBars = Series.Sum(s => s.Values.Count);
        int seriesCount = Series.Count;
        double barWidth = Math.Max(8, (_canvas.Width - 40) / Math.Max(1, totalBars) - 4);

        int barIndex = 0;
        for (int s = 0; s < seriesCount; s++)
        {
            var series = Series[s];
            var color = colors[s % colors.Length];

            for (int i = 0; i < series.Values.Count; i++)
            {
                double value = series.Values[i];
                double height = (value / maxVal) * (_canvas.Height - 40);
                double xOffset = 20 + barIndex * (barWidth + 4);

                var bar = new Border
                {
                    Stroke = Colors.Transparent,
                    BackgroundColor = color,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = new CornerRadius(4, 4, 0, 0)
                    },
                    HeightRequest = height,
                    WidthRequest = barWidth,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    Margin = new Thickness(xOffset, 0, 0, 20)
                };

                _canvas.Children.Add(bar);
                barIndex++;
            }
        }
    }

    private void DrawLines(Color[] colors, IFluentTheme theme)
    {
        // Simplified line/area representation using dots
        for (int s = 0; s < Series!.Count; s++)
        {
            var series = Series[s];
            var color = colors[s % colors.Length];
            var maxVal = series.Values.Max();
            if (maxVal == 0) maxVal = 1;

            for (int i = 0; i < series.Values.Count; i++)
            {
                double value = series.Values[i];
                double x = 20 + (i * ((_canvas.Width - 40) / Math.Max(1, series.Values.Count - 1)));
                double y = _canvas.Height - 20 - ((value / maxVal) * (_canvas.Height - 40));

                var dot = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(4, 4), RadiusX = 4, RadiusY = 4 },
                    Stroke = Colors.White,
                    StrokeThickness = 2,
                    BackgroundColor = color,
                    WidthRequest = 8,
                    HeightRequest = 8,
                    Margin = new Thickness(x - 4, y - 4, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                };

                _canvas.Children.Add(dot);
            }
        }
    }
}

public enum FlCartesianChartType { Bar, Column, Line, Area }

public class FlChartSeries
{
    public string Name { get; set; } = string.Empty;
    public List<double> Values { get; set; } = new();
}

/// <summary>
/// Fluent Design circular chart (Pie and Doughnut).
/// </summary>
public class FlCircularChart : ContentView
{
    public static readonly BindableProperty ChartTypeProperty =
        BindableProperty.Create(nameof(ChartType), typeof(FlCircularChartType), typeof(FlCircularChart), FlCircularChartType.Pie,
            propertyChanged: (b, _, _) => ((FlCircularChart)b).Redraw());

    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(nameof(Series), typeof(ObservableCollection<FlChartSeries>), typeof(FlCircularChart), null,
            propertyChanged: (b, _, _) => ((FlCircularChart)b).Redraw());

    public FlCircularChartType ChartType { get => (FlCircularChartType)GetValue(ChartTypeProperty); set => SetValue(ChartTypeProperty, value); }
    public ObservableCollection<FlChartSeries>? Series { get => (ObservableCollection<FlChartSeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }

    private readonly Grid _canvas;

    public FlCircularChart()
    {
        _canvas = new Grid
        {
            WidthRequest = 200,
            HeightRequest = 200,
            HorizontalOptions = LayoutOptions.Center
        };
        Content = _canvas;
    }

    private void Redraw()
    {
        _canvas.Children.Clear();
        if (Series == null || Series.Count == 0) return;

        var theme = FluentThemeManager.Current.CurrentTheme;
        var colors = new[] { theme.Accent, theme.Success, theme.Warning, theme.Error, theme.Info };
        var allValues = Series.SelectMany(s => s.Values).ToList();
        var total = allValues.Sum();
        if (total == 0) return;

        // Simplified representation using colored circles
        int colorIndex = 0;
        foreach (var series in Series)
        {
            foreach (var value in series.Values)
            {
                double percentage = value / total;
                var color = colors[colorIndex % colors.Length];

                var segment = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(50, 50), RadiusX = 50, RadiusY = 50 },
                    Stroke = Colors.White,
                    StrokeThickness = 2,
                    BackgroundColor = color,
                    WidthRequest = 100 * Math.Sqrt(percentage),
                    HeightRequest = 100 * Math.Sqrt(percentage),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                _canvas.Children.Add(segment);
                colorIndex++;
            }
        }
    }
}

public enum FlCircularChartType { Pie, Doughnut }

/// <summary>
/// Fluent Design funnel chart.
/// </summary>
public class FlFunnelChart : ContentView
{
    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(nameof(Series), typeof(ObservableCollection<FlChartSeries>), typeof(FlFunnelChart), null,
            propertyChanged: (b, _, _) => ((FlFunnelChart)b).Redraw());

    public ObservableCollection<FlChartSeries>? Series { get => (ObservableCollection<FlChartSeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }

    private readonly VerticalStackLayout _stack;

    public FlFunnelChart()
    {
        _stack = new VerticalStackLayout { Spacing = 4 };
        Content = _stack;
    }

    private void Redraw()
    {
        _stack.Children.Clear();
        if (Series == null || Series.Count == 0) return;

        var theme = FluentThemeManager.Current.CurrentTheme;
        var colors = new[] { theme.Accent, theme.Success, theme.Warning, theme.Error, theme.Info };
        var allValues = Series.SelectMany(s => s.Values).ToList();
        var maxVal = allValues.Max();
        if (maxVal == 0) return;

        int colorIndex = 0;
        foreach (var series in Series)
        {
            foreach (var value in series.Values)
            {
                double widthPercent = value / maxVal;
                var color = colors[colorIndex % colors.Length];

                var bar = new Border
                {
                    Stroke = Colors.Transparent,
                    BackgroundColor = color,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(4) },
                    HeightRequest = 32,
                    HorizontalOptions = LayoutOptions.Fill,
                    WidthRequest = 300 * widthPercent,
                    Margin = new Thickness((300 - 300 * widthPercent) / 2, 0)
                };

                _stack.Children.Add(bar);
                colorIndex++;
            }
        }
    }
}
