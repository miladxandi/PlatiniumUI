using System.Collections.ObjectModel;
using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.Buttons;

/// <summary>
/// Fluent Design segmented control for quickly switching between views or categories.
/// </summary>
public class FlSegmentedControl : ContentView
{
    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(FlSegmentedControl), 0,
            propertyChanged: (b, _, _) => ((FlSegmentedControl)b).ApplySelection());

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FlSegmentedControl), FluentCornerRadius.Button,
            propertyChanged: (b, _, _) => ((FlSegmentedControl)b).Rebuild());

    public event EventHandler<int>? SelectionChanged;

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private readonly ObservableCollection<string> _items = new();
    private readonly Border _container;
    private readonly HorizontalStackLayout _stack;

    public ObservableCollection<string> Items => _items;

    public FlSegmentedControl()
    {
        _stack = new HorizontalStackLayout
        {
            Spacing = 0
        };

        _container = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Button)
            },
            Stroke = Colors.Transparent,
            StrokeThickness = 1,
            Content = _stack,
            HorizontalOptions = LayoutOptions.Start
        };

        Content = _container;

        _items.CollectionChanged += (_, _) => Rebuild();

        this.ObserveThemeChanged(() =>
        {
            ApplySelection();
            ApplyContainerStyle();
        });
    }

    private void Rebuild()
    {
        _stack.Children.Clear();

        for (int i = 0; i < _items.Count; i++)
        {
            int index = i;
            var label = new Label
            {
                Text = _items[i],
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var segment = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(CornerRadius)
                },
                Stroke = Colors.Transparent,
                Padding = new Thickness(16, 8),
                Content = label,
                MinimumHeightRequest = 40
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                SelectedIndex = index;
                SelectionChanged?.Invoke(this, index);
            };
            segment.GestureRecognizers.Add(tap);

            _stack.Children.Add(segment);
        }

        ApplyContainerStyle();
        ApplySelection();
    }

    private void ApplyContainerStyle()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _container.BackgroundColor = theme.IsDark
            ? Color.FromArgb("#383838")
            : Color.FromArgb("#F5F5F5");
        _container.Stroke = theme.BorderSubtle;
        _container.StrokeThickness = 1;
        _container.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(CornerRadius)
        };
    }

    private void ApplySelection()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        for (int i = 0; i < _stack.Children.Count; i++)
        {
            if (_stack.Children[i] is Border segment && segment.Content is Label label)
            {
                if (i == SelectedIndex)
                {
                    segment.BackgroundColor = theme.Accent;
                    label.TextColor = theme.TextOnAccent;
                }
                else
                {
                    segment.BackgroundColor = Colors.Transparent;
                    label.TextColor = theme.TextSecondary;
                }
            }
        }
    }
}
