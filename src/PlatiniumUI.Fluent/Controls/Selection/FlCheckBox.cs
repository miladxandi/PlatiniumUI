using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Selection;

/// <summary>
/// Fluent Design checkbox with tri-state support (unchecked, checked, indeterminate).
/// </summary>
public class FlCheckBox : ContentView
{
    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(FlCheckBox), false, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlCheckBox)b).ApplyVisualState());

    public static readonly BindableProperty IsIndeterminateProperty =
        BindableProperty.Create(nameof(IsIndeterminate), typeof(bool), typeof(FlCheckBox), false,
            propertyChanged: (b, _, _) => ((FlCheckBox)b).ApplyVisualState());

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlCheckBox), string.Empty,
            propertyChanged: (b, _, _) => ((FlCheckBox)b).UpdateText());

    public bool IsChecked { get => (bool)GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }
    public bool IsIndeterminate { get => (bool)GetValue(IsIndeterminateProperty); set => SetValue(IsIndeterminateProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public event EventHandler<bool>? CheckedChanged;

    private readonly Border _box;
    private readonly Label _checkMark;
    private readonly Label _textLabel;
    private readonly HorizontalStackLayout _layout;

    public FlCheckBox()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _checkMark = new Label
        {
            FontSize = 12,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _box = new Border
        {
            WidthRequest = 20,
            HeightRequest = 20,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(3)
            },
            Stroke = theme.BorderStrong,
            StrokeThickness = 1,
            BackgroundColor = Colors.Transparent,
            Content = _checkMark,
            VerticalOptions = LayoutOptions.Center
        };

        _textLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextPrimary,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(8, 0, 0, 0)
        };

        _layout = new HorizontalStackLayout
        {
            Spacing = 0,
            VerticalOptions = LayoutOptions.Center,
            Children = { _box, _textLabel }
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        _layout.GestureRecognizers.Add(tap);

        Content = _layout;
        ApplyVisualState();

        this.ObserveThemeChanged(ApplyVisualState);
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled) return;

        if (IsIndeterminate)
        {
            IsIndeterminate = false;
            IsChecked = true;
        }
        else
        {
            IsChecked = !IsChecked;
        }
        CheckedChanged?.Invoke(this, IsChecked);
    }

    private void UpdateText()
    {
        _textLabel.Text = Text;
        _textLabel.IsVisible = !string.IsNullOrEmpty(Text);
    }

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        if (IsIndeterminate)
        {
            _box.BackgroundColor = theme.Accent;
            _box.Stroke = Colors.Transparent;
            _checkMark.Text = "\u2014"; // dash
            _checkMark.IsVisible = true;
            _checkMark.TextColor = theme.TextOnAccent;
        }
        else if (IsChecked)
        {
            _box.BackgroundColor = theme.Accent;
            _box.Stroke = Colors.Transparent;
            _checkMark.Text = "\u2713"; // checkmark
            _checkMark.IsVisible = true;
            _checkMark.TextColor = theme.TextOnAccent;
        }
        else
        {
            _box.BackgroundColor = Colors.Transparent;
            _box.Stroke = theme.BorderStrong;
            _checkMark.IsVisible = false;
        }

        _textLabel.TextColor = IsEnabled ? theme.TextPrimary : theme.TextDisabled;
    }
}

/// <summary>
/// Fluent Design radio button.
/// </summary>
public class FlRadioButton : ContentView
{
    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(FlRadioButton), false, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlRadioButton)b).ApplyVisualState());

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlRadioButton), string.Empty,
            propertyChanged: (b, _, _) => ((FlRadioButton)b).UpdateText());

    public static readonly BindableProperty GroupNameProperty =
        BindableProperty.Create(nameof(GroupName), typeof(string), typeof(FlRadioButton), string.Empty);

    public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string GroupName { get => (string)GetValue(GroupNameProperty); set => SetValue(GroupNameProperty, value); }

    public event EventHandler<bool>? SelectedChanged;

    private readonly Border _circle;
    private readonly Border _dot;
    private readonly Label _textLabel;

    public FlRadioButton()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _dot = new Border
        {
            WidthRequest = 8,
            HeightRequest = 8,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(4) },
            Stroke = Colors.Transparent,
            BackgroundColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _circle = new Border
        {
            WidthRequest = 20,
            HeightRequest = 20,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(10) },
            Stroke = theme.BorderStrong,
            StrokeThickness = 1,
            BackgroundColor = Colors.Transparent,
            Content = _dot,
            VerticalOptions = LayoutOptions.Center
        };

        _textLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextPrimary,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(8, 0, 0, 0)
        };

        var layout = new HorizontalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            Children = { _circle, _textLabel }
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        layout.GestureRecognizers.Add(tap);

        Content = layout;
        ApplyVisualState();

        this.ObserveThemeChanged(ApplyVisualState);
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled) return;
        IsSelected = true;
        SelectedChanged?.Invoke(this, true);
    }

    private void UpdateText()
    {
        _textLabel.Text = Text;
        _textLabel.IsVisible = !string.IsNullOrEmpty(Text);
    }

    private void ApplyVisualState()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        if (IsSelected)
        {
            _circle.Stroke = theme.Accent;
            _circle.StrokeThickness = 2;
            _circle.BackgroundColor = theme.Accent;
            _dot.IsVisible = true;
        }
        else
        {
            _circle.Stroke = theme.BorderStrong;
            _circle.StrokeThickness = 1;
            _circle.BackgroundColor = Colors.Transparent;
            _dot.IsVisible = false;
        }

        _textLabel.TextColor = IsEnabled ? theme.TextPrimary : theme.TextDisabled;
    }
}

/// <summary>
/// Fluent Design toggle switch with smooth on/off animation.
/// </summary>
public class FlToggleSwitch : ContentView
{
    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(FlToggleSwitch), false, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlToggleSwitch)b).AnimateToggle());

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlToggleSwitch), string.Empty,
            propertyChanged: (b, _, _) => ((FlToggleSwitch)b).UpdateText());

    public bool IsToggled { get => (bool)GetValue(IsToggledProperty); set => SetValue(IsToggledProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public event EventHandler<bool>? Toggled;

    private readonly Border _track;
    private readonly Border _thumb;
    private readonly Label _textLabel;
    private readonly Grid _switchGrid;

    public FlToggleSwitch()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _thumb = new Border
        {
            WidthRequest = 16,
            HeightRequest = 16,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(8) },
            Stroke = Colors.Transparent,
            BackgroundColor = Colors.White,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(2, 0, 0, 0)
        };

        _track = new Border
        {
            WidthRequest = 40,
            HeightRequest = 20,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(10) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.BorderStrong,
            Content = _thumb,
            VerticalOptions = LayoutOptions.Center
        };

        _textLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextPrimary,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(8, 0, 0, 0)
        };

        _switchGrid = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
            Children = { _track, _textLabel }
        };
        Grid.SetColumn(_track, 0);
        Grid.SetColumn(_textLabel, 1);

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        _track.GestureRecognizers.Add(tap);

        Content = _switchGrid;
        ApplyVisualState(false);

        this.ObserveThemeChanged(() => ApplyVisualState(false));
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled) return;
        IsToggled = !IsToggled;
        Toggled?.Invoke(this, IsToggled);
    }

    private void UpdateText()
    {
        _textLabel.Text = Text;
        _textLabel.IsVisible = !string.IsNullOrEmpty(Text);
    }

    private void AnimateToggle()
    {
        ApplyVisualState(true);
    }

    private void ApplyVisualState(bool animate)
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        if (IsToggled)
        {
            _track.BackgroundColor = theme.Accent;
            if (animate)
                _thumb.TranslateToAsync(20, 0, FluentAnimations.Fast, FluentAnimations.Standard);
            else
                _thumb.TranslationX = 20;
        }
        else
        {
            _track.BackgroundColor = theme.BorderStrong;
            if (animate)
                _thumb.TranslateToAsync(0, 0, FluentAnimations.Fast, FluentAnimations.Standard);
            else
                _thumb.TranslationX = 0;
        }

        _textLabel.TextColor = IsEnabled ? theme.TextPrimary : theme.TextDisabled;
    }
}

/// <summary>
/// Fluent Design slider control.
/// </summary>
public class FlSlider : ContentView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double), typeof(FlSlider), 0.0, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlSlider)b).OnValueChanged());

    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(double), typeof(FlSlider), 0.0);

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(double), typeof(FlSlider), 1.0);

    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }

    public event EventHandler<double>? ValueChanged;

    private readonly Slider _slider;

    public FlSlider()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _slider = new Slider
        {
            Minimum = Minimum,
            Maximum = Maximum,
            Value = Value,
            MinimumTrackColor = theme.Accent,
            MaximumTrackColor = theme.Border,
            ThumbColor = theme.Accent
        };

        Content = _slider;

        _slider.ValueChanged += (s, e) => { Value = e.NewValue; ValueChanged?.Invoke(this, e.NewValue); };

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _slider.MinimumTrackColor = t.Accent;
            _slider.MaximumTrackColor = t.Border;
            _slider.ThumbColor = t.Accent;
        });
    }

    private void OnValueChanged()
    {
        if (Math.Abs(_slider.Value - Value) > 0.001)
            _slider.Value = Value;
    }
}

/// <summary>
/// Fluent Design rating control with star icons.
/// </summary>
public class FlRating : ContentView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(FlRating), 0, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlRating)b).ApplyStars());

    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.Create(nameof(MaxValue), typeof(int), typeof(FlRating), 5,
            propertyChanged: (b, _, _) => ((FlRating)b).Rebuild());

    public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public int MaxValue { get => (int)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }

    public event EventHandler<int>? ValueChanged;

    private readonly HorizontalStackLayout _stack;
    private readonly List<Label> _stars = new();

    public FlRating()
    {
        _stack = new HorizontalStackLayout { Spacing = 4 };
        Content = _stack;
        Rebuild();
    }

    private void Rebuild()
    {
        _stack.Children.Clear();
        _stars.Clear();
        var theme = FluentThemeManager.Current.CurrentTheme;

        for (int i = 1; i <= MaxValue; i++)
        {
            int starIndex = i;
            var star = new Label
            {
                Text = "\u2606", // empty star
                FontSize = 24,
                TextColor = theme.Accent
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                Value = starIndex;
                ValueChanged?.Invoke(this, starIndex);
            };
            star.GestureRecognizers.Add(tap);

            _stars.Add(star);
            _stack.Children.Add(star);
        }

        ApplyStars();
    }

    private void ApplyStars()
    {
        for (int i = 0; i < _stars.Count; i++)
        {
            _stars[i].Text = i < Value ? "\u2605" : "\u2606"; // filled vs empty
        }
    }
}

/// <summary>
/// Fluent Design radio button group for managing mutually exclusive radio buttons.
/// </summary>
public class FlRadioButtonGroup : ContentView
{
    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(FlRadioButtonGroup), -1, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlRadioButtonGroup)b).UpdateSelection());

    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }

    public event EventHandler<int>? SelectionChanged;

    private readonly VerticalStackLayout _stack;
    private readonly List<FlRadioButton> _radioButtons = new();

    public FlRadioButtonGroup()
    {
        _stack = new VerticalStackLayout { Spacing = 8 };
        Content = _stack;
    }

    public void AddRadioButton(FlRadioButton radioButton)
    {
        _radioButtons.Add(radioButton);
        _stack.Children.Add(radioButton);

        radioButton.SelectedChanged += (_, _) =>
        {
            int index = _radioButtons.IndexOf(radioButton);
            if (index >= 0)
            {
                SelectedIndex = index;
                SelectionChanged?.Invoke(this, index);
            }
        };
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < _radioButtons.Count; i++)
        {
            _radioButtons[i].IsSelected = (i == SelectedIndex);
        }
    }
}

/// <summary>
/// Fluent Design range slider for selecting a range of values.
/// </summary>
public class FlRangeSlider : ContentView
{
    public static readonly BindableProperty MinProperty =
        BindableProperty.Create(nameof(Min), typeof(double), typeof(FlRangeSlider), 0.0);

    public static readonly BindableProperty MaxProperty =
        BindableProperty.Create(nameof(Max), typeof(double), typeof(FlRangeSlider), 100.0);

    public static readonly BindableProperty LowerValueProperty =
        BindableProperty.Create(nameof(LowerValue), typeof(double), typeof(FlRangeSlider), 25.0, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlRangeSlider)b).UpdateRange());

    public static readonly BindableProperty UpperValueProperty =
        BindableProperty.Create(nameof(UpperValue), typeof(double), typeof(FlRangeSlider), 75.0, BindingMode.TwoWay,
            propertyChanged: (b, _, _) => ((FlRangeSlider)b).UpdateRange());

    public double Min { get => (double)GetValue(MinProperty); set => SetValue(MinProperty, value); }
    public double Max { get => (double)GetValue(MaxProperty); set => SetValue(MaxProperty, value); }
    public double LowerValue { get => (double)GetValue(LowerValueProperty); set => SetValue(LowerValueProperty, value); }
    public double UpperValue { get => (double)GetValue(UpperValueProperty); set => SetValue(UpperValueProperty, value); }

    public event EventHandler? RangeChanged;

    private readonly Border _track;
    private readonly Border _rangeFill;
    private readonly Border _lowerThumb;
    private readonly Border _upperThumb;
    private readonly Grid _root;

    public FlRangeSlider()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _track = new Border
        {
            HeightRequest = 4,
            Stroke = Colors.Transparent,
            BackgroundColor = theme.BorderSubtle,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(2) },
            VerticalOptions = LayoutOptions.Center
        };

        _rangeFill = new Border
        {
            HeightRequest = 4,
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Accent,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(2) },
            VerticalOptions = LayoutOptions.Center
        };

        _lowerThumb = new Border
        {
            WidthRequest = 20,
            HeightRequest = 20,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(10, 10), RadiusX = 10, RadiusY = 10 },
            Stroke = Colors.White,
            StrokeThickness = 2,
            BackgroundColor = theme.Accent,
            Shadow = FluentElevation.Level1
        };

        _upperThumb = new Border
        {
            WidthRequest = 20,
            HeightRequest = 20,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(10, 10), RadiusX = 10, RadiusY = 10 },
            Stroke = Colors.White,
            StrokeThickness = 2,
            BackgroundColor = theme.Accent,
            Shadow = FluentElevation.Level1
        };

        _root = new Grid
        {
            Children = { _track, _rangeFill, _lowerThumb, _upperThumb }
        };

        Content = _root;
        UpdateRange();

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _track.BackgroundColor = t.BorderSubtle;
            _rangeFill.BackgroundColor = t.Accent;
            _lowerThumb.BackgroundColor = t.Accent;
            _upperThumb.BackgroundColor = t.Accent;
        });
    }

    private void UpdateRange()
    {
        if (_root.Width <= 0) return;

        double range = Max - Min;
        if (range <= 0) return;

        double lowerPercent = (LowerValue - Min) / range;
        double upperPercent = (UpperValue - Min) / range;

        double trackWidth = _root.Width;
        _rangeFill.WidthRequest = (upperPercent - lowerPercent) * trackWidth;
        _rangeFill.Margin = new Thickness(lowerPercent * trackWidth, 0, 0, 0);

        _lowerThumb.Margin = new Thickness(lowerPercent * trackWidth - 10, 0, 0, 0);
        _upperThumb.Margin = new Thickness(upperPercent * trackWidth - 10, 0, 0, 0);
    }
}
