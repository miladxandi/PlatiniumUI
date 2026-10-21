using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Feedback;

/// <summary>
/// Snackbar position options.
/// </summary>
public enum FlSnackbarPosition
{
    Bottom,
    Top
}

/// <summary>
/// Fluent Design snackbar control for toast-like notifications with auto-dismiss and action button.
/// </summary>
public class FlSnackbar : ContentView
{
    // ─── Bindable Properties ────────────────────────────────────────────────

    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(FlSnackbar), string.Empty,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty ActionTextProperty =
        BindableProperty.Create(nameof(ActionText), typeof(string), typeof(FlSnackbar), null,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty ActionCommandProperty =
        BindableProperty.Create(nameof(ActionCommand), typeof(System.Windows.Input.ICommand), typeof(FlSnackbar), null);

    public static readonly BindableProperty DurationProperty =
        BindableProperty.Create(nameof(Duration), typeof(int), typeof(FlSnackbar), 3000);

    public static readonly BindableProperty PositionProperty =
        BindableProperty.Create(nameof(Position), typeof(FlSnackbarPosition), typeof(FlSnackbar), FlSnackbarPosition.Bottom,
            propertyChanged: OnVisualPropertyChanged);

    // ─── Properties ─────────────────────────────────────────────────────────

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string? ActionText
    {
        get => (string?)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public System.Windows.Input.ICommand? ActionCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public int Duration
    {
        get => (int)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public FlSnackbarPosition Position
    {
        get => (FlSnackbarPosition)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    // ─── Private Fields ─────────────────────────────────────────────────────

    private readonly Border _container;
    private readonly Label _messageLabel;
    private readonly Button _actionButton;
    private readonly Grid _content;
    private CancellationTokenSource? _dismissCts;

    // ─── Constructor ────────────────────────────────────────────────────────

    public FlSnackbar()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _messageLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill
        };

        _actionButton = new Button
        {
            BackgroundColor = Colors.Transparent,
            TextColor = theme.AccentLight,
            FontSize = FluentTypography.BodyStrongSize,
            FontAttributes = FontAttributes.Bold,
            BorderWidth = 0,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            IsVisible = false
        };
        _actionButton.Clicked += OnActionButtonClicked;

        _content = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            Children = { _messageLabel, _actionButton }
        };
        Grid.SetColumn(_actionButton, 1);

        _container = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(FluentCornerRadius.Medium)
            },
            Stroke = Colors.Transparent,
            BackgroundColor = Color.FromArgb("#323232"), // Dark gray
            Padding = new Thickness(FluentSpacing.M, FluentSpacing.S),
            Content = _content,
            Shadow = FluentElevation.Level3,
            Margin = new Thickness(FluentSpacing.M),
            Opacity = 0
        };

        Content = _container;

        this.ObserveThemeChanged(ApplyTheme);
    }

    // ─── Property Changed Handlers ──────────────────────────────────────────

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((FlSnackbar)bindable).ApplyVisualState();
    }

    // ─── Visual State ───────────────────────────────────────────────────────

    private void ApplyVisualState()
    {
        _messageLabel.Text = Message;

        if (!string.IsNullOrEmpty(ActionText))
        {
            _actionButton.Text = ActionText;
            _actionButton.IsVisible = true;
        }
        else
        {
            _actionButton.IsVisible = false;
        }
    }

    private void ApplyTheme()
    {
        // Snackbar uses a fixed dark background for contrast
    }

    // ─── Actions ────────────────────────────────────────────────────────────

    private void OnActionButtonClicked(object? sender, EventArgs e)
    {
        ActionCommand?.Execute(null);
        Dismiss();
    }

    // ─── Public Methods ─────────────────────────────────────────────────────

    /// <summary>
    /// Shows the snackbar with animation and auto-dismiss.
    /// </summary>
    public async Task ShowAsync()
    {
        // Cancel any existing dismiss timer
        _dismissCts?.Cancel();
        _dismissCts = new CancellationTokenSource();

        // Animate in
        _container.Opacity = 0;
        _container.TranslationY = Position == FlSnackbarPosition.Bottom ? 50 : -50;
        _container.IsVisible = true;

        await Task.WhenAll(
            _container.FadeToAsync(1, FluentAnimations.Gentle),
            _container.TranslateToAsync(0, 0, FluentAnimations.Gentle, FluentAnimations.Entrance)
        );

        // Auto-dismiss after duration
        if (Duration > 0)
        {
            _ = Task.Delay(Duration, _dismissCts.Token).ContinueWith(async t =>
            {
                if (!t.IsCanceled)
                {
                    await DismissAsync();
                }
            });
        }
    }

    /// <summary>
    /// Dismisses the snackbar with animation.
    /// </summary>
    public void Dismiss()
    {
        _ = DismissAsync();
    }

    private async Task DismissAsync()
    {
        _dismissCts?.Cancel();

        await Task.WhenAll(
            _container.FadeToAsync(0, FluentAnimations.Gentle),
            _container.TranslateToAsync(0, Position == FlSnackbarPosition.Bottom ? 50 : -50, FluentAnimations.Gentle, FluentAnimations.Exit)
        );

        _container.IsVisible = false;
    }

    /// <summary>
    /// Static helper to show a snackbar message.
    /// </summary>
    public static async Task Show(string message, string? actionText = null, System.Windows.Input.ICommand? actionCommand = null, int duration = 3000)
    {
        var snackbar = new FlSnackbar
        {
            Message = message,
            ActionText = actionText,
            ActionCommand = actionCommand,
            Duration = duration
        };

        await snackbar.ShowAsync();
    }
}
