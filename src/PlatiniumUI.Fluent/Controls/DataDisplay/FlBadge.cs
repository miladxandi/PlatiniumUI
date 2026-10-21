using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;

namespace PlatiniumUI.Fluent.Controls.DataDisplay;

/// <summary>
/// Fluent Design badge with dot, count, and label modes.
/// </summary>
public class FlBadge : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlBadge), string.Empty,
            propertyChanged: (b, _, _) => ((FlBadge)b).UpdateContent());

    public static readonly BindableProperty BadgeTypeProperty =
        BindableProperty.Create(nameof(BadgeType), typeof(FlBadgeType), typeof(FlBadge), FlBadgeType.Dot,
            propertyChanged: (b, _, _) => ((FlBadge)b).UpdateContent());

    public static readonly BindableProperty BadgeContentProperty =
        BindableProperty.Create(nameof(BadgeContent), typeof(View), typeof(FlBadge), null,
            propertyChanged: (b, _, _) => ((FlBadge)b).UpdateLayout());

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public FlBadgeType BadgeType { get => (FlBadgeType)GetValue(BadgeTypeProperty); set => SetValue(BadgeTypeProperty, value); }
    public View? BadgeContent { get => (View?)GetValue(BadgeContentProperty); set => SetValue(BadgeContentProperty, value); }

    private readonly Border _badge;
    private readonly Label _label;
    private readonly Grid _root;

    public FlBadge()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label
        {
            FontSize = 10,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        _badge = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Badge) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Error,
            Content = _label,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            ZIndex = 1
        };

        _root = new Grid();
        _root.Children.Add(_badge);

        Content = _root;
        UpdateContent();
    }

    private void UpdateLayout()
    {
        if (BadgeContent != null)
        {
            _root.Children.Insert(0, BadgeContent);
        }
    }

    private void UpdateContent()
    {
        switch (BadgeType)
        {
            case FlBadgeType.Dot:
                _badge.WidthRequest = 8;
                _badge.HeightRequest = 8;
                _label.IsVisible = false;
                break;
            case FlBadgeType.Count:
                _badge.MinimumWidthRequest = 20;
                _badge.HeightRequest = 20;
                _label.Text = Text;
                _label.IsVisible = true;
                break;
            case FlBadgeType.Label:
                _badge.HeightRequest = 20;
                _label.Text = Text;
                _label.IsVisible = true;
                _badge.Padding = new Thickness(8, 0);
                break;
        }
    }
}

public enum FlBadgeType { Dot, Count, Label }

/// <summary>
/// Fluent Design avatar with image, initials, or icon modes.
/// </summary>
public class FlAvatar : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlAvatar), string.Empty,
            propertyChanged: (b, _, _) => ((FlAvatar)b).UpdateContent());

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(FlAvatar), null,
            propertyChanged: (b, _, _) => ((FlAvatar)b).UpdateContent());

    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(double), typeof(FlAvatar), 40.0,
            propertyChanged: (b, _, _) => ((FlAvatar)b).ApplySize());

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public ImageSource? ImageSource { get => (ImageSource?)GetValue(ImageSourceProperty); set => SetValue(ImageSourceProperty, value); }
    public double Size { get => (double)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }

    private readonly Border _border;
    private readonly Label _label;
    private readonly Image _image;

    public FlAvatar()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label
        {
            FontSize = 16,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        _image = new Image
        {
            Aspect = Aspect.AspectFill,
            IsVisible = false
        };

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Circular) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Accent,
            Content = _label,
            WidthRequest = 40,
            HeightRequest = 40
        };

        Content = _border;
        ApplySize();
    }

    private void UpdateContent()
    {
        if (ImageSource != null)
        {
            _image.Source = ImageSource;
            _image.IsVisible = true;
            _label.IsVisible = false;
            _border.Content = _image;
        }
        else if (!string.IsNullOrEmpty(Text))
        {
            _image.IsVisible = false;
            _label.IsVisible = true;
            _label.Text = GetInitials(Text);
            _border.Content = _label;
        }
    }

    private void ApplySize()
    {
        _border.WidthRequest = Size;
        _border.HeightRequest = Size;
        _label.FontSize = Size * 0.4;
    }

    private static string GetInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpperInvariant();
        return name.Length > 0 ? name[0].ToString().ToUpperInvariant() : "?";
    }
}

/// <summary>
/// Fluent Design avatar group showing overlapping avatars.
/// </summary>
public class FlAvatarGroup : ContentView
{
    private readonly HorizontalStackLayout _stack;

    public FlAvatarGroup()
    {
        _stack = new HorizontalStackLayout { Spacing = -8 };
        Content = _stack;
    }

    public void AddAvatar(FlAvatar avatar)
    {
        avatar.Margin = new Thickness(-4, 0, 0, 0);
        _stack.Children.Add(avatar);
    }
}

/// <summary>
/// Fluent Design tooltip.
/// </summary>
public class FlTooltip : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FlTooltip), string.Empty);

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    private readonly Border _border;
    private readonly Label _label;

    public FlTooltip()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _label = new Label
        {
            FontSize = FluentTypography.CaptionSize,
            TextColor = theme.TextOnAccent
        };

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Tooltip) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.TextPrimary,
            Padding = new Thickness(8, 4),
            Content = _label,
            Shadow = FluentElevation.Tooltip
        };

        Content = _border;
    }
}
