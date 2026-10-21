using PlatiniumUI.Fluent.Controls;
using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Feedback;

/// <summary>
/// Fluent Design linear progress bar (determinate and indeterminate).
/// </summary>
public class FlProgressBar : ContentView
{
    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(FlProgressBar), 0.0,
            propertyChanged: (b, _, _) => ((FlProgressBar)b).UpdateProgress());

    public static readonly BindableProperty IsIndeterminateProperty =
        BindableProperty.Create(nameof(IsIndeterminate), typeof(bool), typeof(FlProgressBar), false,
            propertyChanged: (b, _, _) => ((FlProgressBar)b).ToggleIndeterminate());

    public double Progress { get => (double)GetValue(ProgressProperty); set => SetValue(ProgressProperty, value); }
    public bool IsIndeterminate { get => (bool)GetValue(IsIndeterminateProperty); set => SetValue(IsIndeterminateProperty, value); }

    private readonly Border _track;
    private readonly Border _fill;

    public FlProgressBar()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _fill = new Border
        {
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Accent,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(2) },
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 0
        };

        _track = new Border
        {
            HeightRequest = 4,
            Stroke = Colors.Transparent,
            BackgroundColor = theme.BorderSubtle,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(2) },
            Content = _fill,
            HorizontalOptions = LayoutOptions.Fill
        };

        Content = _track;

        this.ObserveThemeChanged(() =>
        {
            var t = FluentThemeManager.Current.CurrentTheme;
            _track.BackgroundColor = t.BorderSubtle;
            _fill.BackgroundColor = t.Accent;
        });
    }

    private void UpdateProgress()
    {
        if (!IsIndeterminate && _track.Width > 0)
        {
            _fill.WidthRequest = Math.Max(0, Math.Min(1, Progress)) * _track.Width;
        }
    }

    private void ToggleIndeterminate()
    {
        if (IsIndeterminate)
        {
            _fill.WidthRequest = _track.Width * 0.3;
            AnimateIndeterminate();
        }
    }

    private async void AnimateIndeterminate()
    {
        while (IsIndeterminate && IsVisible)
        {
            _fill.TranslationX = 0;
            await _fill.TranslateToAsync(_track.Width, 0, FluentAnimations.ProgressIndeterminateDuration, FluentAnimations.Smooth);
        }
    }
}

/// <summary>
/// Fluent Design circular progress indicator.
/// </summary>
public class FlCircularProgress : ContentView
{
    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(FlCircularProgress), 0.0);

    public static readonly BindableProperty IsIndeterminateProperty =
        BindableProperty.Create(nameof(IsIndeterminate), typeof(bool), typeof(FlCircularProgress), false);

    public static readonly BindableProperty SizeProperty =
        BindableProperty.Create(nameof(Size), typeof(double), typeof(FlCircularProgress), 40.0);

    public double Progress { get => (double)GetValue(ProgressProperty); set => SetValue(ProgressProperty, value); }
    public bool IsIndeterminate { get => (bool)GetValue(IsIndeterminateProperty); set => SetValue(IsIndeterminateProperty, value); }
    public double Size { get => (double)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }

    private readonly ActivityIndicator _indicator;

    public FlCircularProgress()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        _indicator = new ActivityIndicator
        {
            Color = theme.Accent,
            IsRunning = true,
            IsVisible = IsIndeterminate
        };
        Content = _indicator;

        this.ObserveThemeChanged(() => _indicator.Color = FluentThemeManager.Current.CurrentTheme.Accent);
    }
}

/// <summary>
/// Fluent Design shimmer/skeleton loading indicator.
/// </summary>
public class FlShimmer : ContentView
{
    public static readonly BindableProperty IsActiveProperty =
        BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(FlShimmer), true,
            propertyChanged: (b, _, _) => ((FlShimmer)b).ToggleAnimation());

    public static readonly BindableProperty ShimmerTypeProperty =
        BindableProperty.Create(nameof(ShimmerType), typeof(FlShimmerType), typeof(FlShimmer), FlShimmerType.Text);

    public bool IsActive { get => (bool)GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public FlShimmerType ShimmerType { get => (FlShimmerType)GetValue(ShimmerTypeProperty); set => SetValue(ShimmerTypeProperty, value); }

    private readonly Border _shimmer;
    private readonly Border _wave;

    public FlShimmer()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;
        var bgColor = theme.IsDark ? Color.FromArgb("#383838") : Color.FromArgb("#EBEBEB");

        _wave = new Border
        {
            Stroke = Colors.Transparent,
            BackgroundColor = theme.IsDark ? Color.FromArgb("#444444") : Color.FromArgb("#F5F5F5"),
            WidthRequest = 60,
            Opacity = 0.7
        };

        _shimmer = new Border
        {
            Stroke = Colors.Transparent,
            BackgroundColor = bgColor,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(4) },
            HeightRequest = 16,
            HorizontalOptions = LayoutOptions.Fill,
            Content = _wave
        };

        Content = _shimmer;
        ToggleAnimation();
    }

    private async void ToggleAnimation()
    {
        if (IsActive)
        {
            while (IsActive && IsVisible)
            {
                _wave.TranslationX = -_wave.Width;
                await _wave.TranslateToAsync(_shimmer.Width, 0, FluentAnimations.ShimmerDuration, FluentAnimations.Linear);
            }
        }
    }
}

public enum FlShimmerType { Text, Circle, Rectangle }

/// <summary>
/// Fluent Design toast/snackbar notification.
/// </summary>
public class FlToast : ContentView
{
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(FlToast), string.Empty);

    public static readonly BindableProperty ToastTypeProperty =
        BindableProperty.Create(nameof(ToastType), typeof(FlToastType), typeof(FlToast), FlToastType.Info);

    public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
    public FlToastType ToastType { get => (FlToastType)GetValue(ToastTypeProperty); set => SetValue(ToastTypeProperty, value); }

    private readonly Label _messageLabel;
    private readonly Border _border;

    public FlToast()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _messageLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextOnAccent,
            VerticalOptions = LayoutOptions.Center
        };

        _border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Medium) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.TextPrimary,
            Padding = new Thickness(16, 12),
            Content = _messageLabel,
            Shadow = FluentElevation.Level3,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(16, 0, 16, 16)
        };

        Content = _border;
    }
}

public enum FlToastType { Info, Success, Warning, Error }
