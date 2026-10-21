using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Layout;

/// <summary>
/// Fluent Design card with header, body, footer support and elevation.
/// </summary>
public class FlCard : ContentView
{
    public static readonly BindableProperty CardContentProperty =
        BindableProperty.Create(nameof(CardContent), typeof(View), typeof(FlCard), null,
            propertyChanged: (b, _, _) => ((FlCard)b).UpdateContent());

    public static readonly BindableProperty ElevationProperty =
        BindableProperty.Create(nameof(Elevation), typeof(int), typeof(FlCard), 1,
            propertyChanged: (b, _, _) => ((FlCard)b).ApplyElevation());

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(FlCard), FluentCornerRadius.Card,
            propertyChanged: (b, _, _) => ((FlCard)b).ApplyElevation());

    public View? CardContent { get => (View?)GetValue(CardContentProperty); set => SetValue(CardContentProperty, value); }
    public int Elevation { get => (int)GetValue(ElevationProperty); set => SetValue(ElevationProperty, value); }
    public double CornerRadius { get => (double)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

    private readonly Border _border;
    private readonly VerticalStackLayout _layout;

    public FlCard()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _layout = new VerticalStackLayout { Spacing = 0 };

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Card)
            },
            Stroke = theme.BorderSubtle,
            StrokeThickness = 1,
            Padding = new Thickness(FluentSpacing.CardPadding),
            BackgroundColor = theme.SurfaceCard,
            Content = _layout
        };

        Content = _border;
        ApplyElevation();

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _border.BackgroundColor = t.SurfaceCard;
            _border.Stroke = t.BorderSubtle;
            ApplyElevation();
        });
    }

    private void UpdateContent()
    {
        _layout.Children.Clear();
        if (CardContent != null)
            _layout.Children.Add(CardContent);
    }

    private void ApplyElevation()
    {
        _border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(CornerRadius)
        };

        _border.Shadow = Elevation switch
        {
            0 => FluentElevation.Level0,
            1 => FluentElevation.Level1,
            2 => FluentElevation.Level2,
            3 => FluentElevation.Level3,
            4 => FluentElevation.Level4,
            _ => FluentElevation.Level1
        };
    }
}

/// <summary>
/// Fluent Design expander with smooth expand/collapse animation.
/// </summary>
public class FlExpander : ContentView
{
    public static readonly BindableProperty HeaderProperty =
        BindableProperty.Create(nameof(Header), typeof(string), typeof(FlExpander), string.Empty,
            propertyChanged: (b, _, _) => ((FlExpander)b).UpdateHeader());

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(FlExpander), false,
            propertyChanged: (b, _, _) => ((FlExpander)b).ToggleContent());

    public static readonly BindableProperty ExpanderContentProperty =
        BindableProperty.Create(nameof(ExpanderContent), typeof(View), typeof(FlExpander), null,
            propertyChanged: (b, _, _) => ((FlExpander)b).UpdateContent());

    public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public bool IsExpanded { get => (bool)GetValue(IsExpandedProperty); set => SetValue(IsExpandedProperty, value); }
    public View? ExpanderContent { get => (View?)GetValue(ExpanderContentProperty); set => SetValue(ExpanderContentProperty, value); }

    private readonly Label _headerLabel;
    private readonly Label _arrow;
    private readonly Border _headerBorder;
    private readonly ContentView _contentView;
    private readonly VerticalStackLayout _root;

    public FlExpander()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _headerLabel = new Label
        {
            FontSize = FluentTypography.BodyStrongSize,
            TextColor = theme.TextPrimary,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };

        _arrow = new Label
        {
            Text = "\u276F",
            FontSize = 14,
            TextColor = theme.TextSecondary,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            Rotation = 0
        };

        _headerBorder = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(0) },
            Stroke = Colors.Transparent,
            BackgroundColor = Colors.Transparent,
            Padding = new Thickness(0, 12),
            Content = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                Children = { _headerLabel, _arrow }
            }
        };
        Grid.SetColumn(_headerLabel, 0);
        Grid.SetColumn(_arrow, 1);

        _contentView = new ContentView { IsVisible = false };

        _root = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { _headerBorder, _contentView }
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => IsExpanded = !IsExpanded;
        _headerBorder.GestureRecognizers.Add(tap);

        Content = _root;

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _headerLabel.TextColor = t.TextPrimary;
            _arrow.TextColor = t.TextSecondary;
        });
    }

    private void UpdateHeader() { _headerLabel.Text = Header; }

    private void UpdateContent()
    {
        _contentView.Content = ExpanderContent;
    }

    private async void ToggleContent()
    {
        if (IsExpanded)
        {
            _contentView.IsVisible = true;
            await _arrow.RotateToAsync(90, FluentAnimations.Fast, FluentAnimations.Standard);
        }
        else
        {
            await _arrow.RotateToAsync(0, FluentAnimations.Fast, FluentAnimations.Standard);
            _contentView.IsVisible = false;
        }
    }
}

/// <summary>
/// Fluent Design divider (horizontal or vertical).
/// </summary>
public class FlDivider : ContentView
{
    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(FlDivider), StackOrientation.Horizontal,
            propertyChanged: (b, _, _) => ((FlDivider)b).ApplyOrientation());

    public StackOrientation Orientation { get => (StackOrientation)GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

    private readonly Border _line;

    public FlDivider()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _line = new Border
        {
            Stroke = Colors.Transparent,
            BackgroundColor = theme.BorderSubtle,
            StrokeThickness = 0
        };
        Content = _line;
        ApplyOrientation();

        this.ObserveThemeChanged(() => _line.BackgroundColor = FluentThemeManager.Current.CurrentTheme.BorderSubtle);
    }

    private void ApplyOrientation()
    {
        if (Orientation == StackOrientation.Horizontal)
        {
            _line.HeightRequest = 1;
            _line.HorizontalOptions = LayoutOptions.Fill;
        }
        else
        {
            _line.WidthRequest = 1;
            _line.VerticalOptions = LayoutOptions.Fill;
        }
    }
}
