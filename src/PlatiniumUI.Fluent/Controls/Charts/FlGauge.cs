using System.Collections.ObjectModel;
using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.Charts;

/// <summary>
/// Fluent Design polar/radar chart.
/// </summary>
public class FlPolarChart : ContentView
{
    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(nameof(Series), typeof(ObservableCollection<FlChartSeries>), typeof(FlPolarChart), null,
            propertyChanged: (b, _, _) => ((FlPolarChart)b).Redraw());

    public ObservableCollection<FlChartSeries>? Series { get => (ObservableCollection<FlChartSeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }

    private readonly Grid _canvas;

    public FlPolarChart()
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
        var colors = new[] { theme.Accent, theme.Success, theme.Warning, theme.Error };

        int colorIndex = 0;
        foreach (var series in Series)
        {
            var color = colors[colorIndex % colors.Length];
            var maxVal = series.Values.Max();
            if (maxVal == 0) maxVal = 1;

            for (int i = 0; i < series.Values.Count; i++)
            {
                double angle = (2 * Math.PI * i) / series.Values.Count;
                double radius = (series.Values[i] / maxVal) * 80;
                double x = 100 + radius * Math.Cos(angle) - 4;
                double y = 100 + radius * Math.Sin(angle) - 4;

                var dot = new Border
                {
                    Stroke = Colors.White,
                    StrokeThickness = 2,
                    BackgroundColor = color,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(4, 4), RadiusX = 4, RadiusY = 4 },
                    WidthRequest = 8,
                    HeightRequest = 8,
                    Margin = new Thickness(x, y, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                };

                _canvas.Children.Add(dot);
            }
            colorIndex++;
        }
    }
}

/// <summary>
/// Fluent Design pyramid chart.
/// </summary>
public class FlPyramidChart : ContentView
{
    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(nameof(Series), typeof(ObservableCollection<FlChartSeries>), typeof(FlPyramidChart), null,
            propertyChanged: (b, _, _) => ((FlPyramidChart)b).Redraw());

    public ObservableCollection<FlChartSeries>? Series { get => (ObservableCollection<FlChartSeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }

    private readonly VerticalStackLayout _stack;

    public FlPyramidChart()
    {
        _stack = new VerticalStackLayout { Spacing = 2 };
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
        int totalItems = allValues.Count;
        for (int idx = 0; idx < totalItems; idx++)
        {
            double value = allValues[idx];
            double widthPercent = 1.0 - ((double)idx / totalItems) * 0.7;
            var color = colors[colorIndex % colors.Length];

            var bar = new Border
            {
                Stroke = Colors.Transparent,
                BackgroundColor = color,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(4) },
                HeightRequest = 28,
                WidthRequest = 280 * widthPercent,
                HorizontalOptions = LayoutOptions.Center
            };

            _stack.Children.Add(bar);
            colorIndex++;
        }
    }
}

/// <summary>
/// Fluent Design spark/micro chart.
/// </summary>
public class FlSparkChart : ContentView
{
    public static readonly BindableProperty ValuesProperty =
        BindableProperty.Create(nameof(Values), typeof(IList<double>), typeof(FlSparkChart), null,
            propertyChanged: (b, _, _) => ((FlSparkChart)b).Redraw());

    public static readonly BindableProperty SparkTypeProperty =
        BindableProperty.Create(nameof(SparkType), typeof(FlSparkType), typeof(FlSparkChart), FlSparkType.Line);

    public IList<double>? Values { get => (IList<double>?)GetValue(ValuesProperty); set => SetValue(ValuesProperty, value); }
    public FlSparkType SparkType { get => (FlSparkType)GetValue(SparkTypeProperty); set => SetValue(SparkTypeProperty, value); }

    private readonly HorizontalStackLayout _stack;

    public FlSparkChart()
    {
        _stack = new HorizontalStackLayout { Spacing = 1, VerticalOptions = LayoutOptions.End };
        Content = _stack;
        HeightRequest = 32;
    }

    private void Redraw()
    {
        _stack.Children.Clear();
        if (Values == null || Values.Count == 0) return;

        var theme = FluentThemeManager.Current.CurrentTheme;
        var maxVal = Values.Max();
        if (maxVal == 0) maxVal = 1;

        foreach (var value in Values)
        {
            double height = (value / maxVal) * 28;
            var bar = new Border
            {
                Stroke = Colors.Transparent,
                BackgroundColor = theme.Accent,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(1) },
                WidthRequest = 3,
                HeightRequest = height,
                VerticalOptions = LayoutOptions.End
            };
            _stack.Children.Add(bar);
        }
    }
}

public enum FlSparkType { Line, Bar }

/// <summary>
/// Fluent Design sunburst chart.
/// </summary>
public class FlSunburstChart : ContentView
{
    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(nameof(Series), typeof(ObservableCollection<FlChartSeries>), typeof(FlSunburstChart), null,
            propertyChanged: (b, _, _) => ((FlSunburstChart)b).Redraw());

    public ObservableCollection<FlChartSeries>? Series { get => (ObservableCollection<FlChartSeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }

    private readonly Grid _canvas;

    public FlSunburstChart()
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

        // Center circle
        var center = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(20, 20), RadiusX = 20, RadiusY = 20 },
            Stroke = Colors.White,
            StrokeThickness = 2,
            BackgroundColor = theme.TextPrimary,
            WidthRequest = 40,
            HeightRequest = 40,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        _canvas.Children.Add(center);

        // Outer rings
        int colorIndex = 0;
        foreach (var series in Series)
        {
            foreach (var value in series.Values)
            {
                var color = colors[colorIndex % colors.Length];
                double size = 60 + (colorIndex * 20);

                var ring = new Border
                {
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(size / 2, size / 2), RadiusX = size / 2, RadiusY = size / 2 },
                    Stroke = Colors.White,
                    StrokeThickness = 3,
                    BackgroundColor = color,
                    WidthRequest = size,
                    HeightRequest = size,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Opacity = 0.7
                };
                _canvas.Children.Add(ring);
                colorIndex++;
            }
        }
    }
}

/// <summary>
/// Fluent Design gauge (radial and linear).
/// </summary>
public class FlGauge : ContentView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double), typeof(FlGauge), 0.0,
            propertyChanged: (b, _, _) => ((FlGauge)b).UpdateValue());

    public static readonly BindableProperty MinProperty =
        BindableProperty.Create(nameof(Min), typeof(double), typeof(FlGauge), 0.0);

    public static readonly BindableProperty MaxProperty =
        BindableProperty.Create(nameof(Max), typeof(double), typeof(FlGauge), 100.0);

    public static readonly BindableProperty GaugeTypeProperty =
        BindableProperty.Create(nameof(GaugeType), typeof(FlGaugeType), typeof(FlGauge), FlGaugeType.Radial);

    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Min { get => (double)GetValue(MinProperty); set => SetValue(MinProperty, value); }
    public double Max { get => (double)GetValue(MaxProperty); set => SetValue(MaxProperty, value); }
    public FlGaugeType GaugeType { get => (FlGaugeType)GetValue(GaugeTypeProperty); set => SetValue(GaugeTypeProperty, value); }

    private readonly Border _fill;
    private readonly Border _track;
    private readonly Label _valueLabel;
    private readonly Grid _root;

    public FlGauge()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _valueLabel = new Label
        {
            FontSize = FluentTypography.TitleSize,
            TextColor = theme.TextPrimary,
            HorizontalTextAlignment = TextAlignment.Center
        };

        _fill = new Border
        {
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Accent,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(4) },
            HeightRequest = 8,
            HorizontalOptions = LayoutOptions.Start
        };

        _track = new Border
        {
            Stroke = Colors.Transparent,
            BackgroundColor = theme.BorderSubtle,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(4) },
            HeightRequest = 8,
            HorizontalOptions = LayoutOptions.Fill,
            Content = _fill
        };

        _root = new Grid
        {
            Children = { _track, _valueLabel }
        };

        Content = _root;

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _track.BackgroundColor = t.BorderSubtle;
            _fill.BackgroundColor = t.Accent;
            _valueLabel.TextColor = t.TextPrimary;
        });
    }

    private void UpdateValue()
    {
        var range = Max - Min;
        if (range <= 0) return;

        double percent = (Value - Min) / range;
        percent = Math.Max(0, Math.Min(1, percent));

        _valueLabel.Text = $"{Value:F0}";

        if (_track.Width > 0)
        {
            _fill.WidthRequest = percent * _track.Width;
        }
    }
}

public enum FlGaugeType { Radial, Linear }
