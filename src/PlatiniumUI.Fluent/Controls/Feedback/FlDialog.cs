using PlatiniumUI.Fluent.DesignSystem.Theme;
using PlatiniumUI.Fluent.DesignSystem.Tokens;
using PlatiniumUI.Fluent.DesignSystem.Motion;

namespace PlatiniumUI.Fluent.Controls.Feedback;

/// <summary>
/// Fluent Design dialog (alert, confirm, custom content).
/// </summary>
public class FlDialog : ContentView
{
    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(FlDialog), false,
            propertyChanged: (b, _, _) => ((FlDialog)b).ToggleVisibility());

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(FlDialog), string.Empty);

    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(FlDialog), string.Empty);

    public static readonly BindableProperty DialogContentProperty =
        BindableProperty.Create(nameof(DialogContent), typeof(View), typeof(FlDialog), null,
            propertyChanged: (b, _, _) => ((FlDialog)b).UpdateContent());

    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
    public View? DialogContent { get => (View?)GetValue(DialogContentProperty); set => SetValue(DialogContentProperty, value); }

    public event EventHandler? Accepted;
    public event EventHandler? Cancelled;

    private readonly Border _dialog;
    private readonly Label _titleLabel;
    private readonly Label _messageLabel;
    private readonly VerticalStackLayout _contentStack;
    private readonly HorizontalStackLayout _buttonStack;
    private readonly Grid _overlay;

    public FlDialog()
    {
        var theme = FluentThemeManager.Current.CurrentTheme;

        _titleLabel = new Label
        {
            FontSize = FluentTypography.TitleSize,
            TextColor = theme.TextPrimary,
            FontAttributes = FontAttributes.Bold
        };

        _messageLabel = new Label
        {
            FontSize = FluentTypography.BodySize,
            TextColor = theme.TextSecondary
        };

        _buttonStack = new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.End };

        var cancelButton = new Button
        {
            Text = "Cancel",
            BackgroundColor = Colors.Transparent,
            TextColor = theme.Accent,
            CornerRadius = (int)FluentCornerRadius.Small
        };
        cancelButton.Clicked += (_, _) => { IsOpen = false; Cancelled?.Invoke(this, EventArgs.Empty); };

        var acceptButton = new Button
        {
            Text = "OK",
            BackgroundColor = theme.Accent,
            TextColor = theme.TextOnAccent,
            CornerRadius = (int)FluentCornerRadius.Small
        };
        acceptButton.Clicked += (_, _) => { IsOpen = false; Accepted?.Invoke(this, EventArgs.Empty); };

        _buttonStack.Children.Add(cancelButton);
        _buttonStack.Children.Add(acceptButton);

        _contentStack = new VerticalStackLayout
        {
            Spacing = 12,
            Children = { _titleLabel, _messageLabel, _buttonStack }
        };

        _dialog = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(FluentCornerRadius.Large) },
            Stroke = Colors.Transparent,
            BackgroundColor = theme.Surface,
            Padding = new Thickness(24),
            Content = _contentStack,
            Shadow = FluentElevation.Dialog,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            MaximumWidthRequest = 400
        };

        _overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#00000000"),
            IsVisible = false,
            Children = { _dialog }
        };

        Content = _overlay;
    }

    private void UpdateContent()
    {
        if (DialogContent != null)
        {
            _contentStack.Children.Clear();
            _contentStack.Children.Add(_titleLabel);
            _contentStack.Children.Add(_messageLabel);
            _contentStack.Children.Add(DialogContent);
            _contentStack.Children.Add(_buttonStack);
        }
    }

    private async void ToggleVisibility()
    {
        if (IsOpen)
        {
            _overlay.IsVisible = true;
            _dialog.Scale = 0.8;
            _dialog.Opacity = 0;

            await Task.WhenAll(
                _dialog.ScaleToAsync(1, FluentAnimations.Gentle, FluentAnimations.Entrance),
                _dialog.FadeToAsync(1, FluentAnimations.Gentle),
                _overlay.FadeToAsync(1, FluentAnimations.Gentle)
            );

            _overlay.BackgroundColor = FluentThemeManager.Current.CurrentTheme.Scrim;
        }
        else
        {
            await Task.WhenAll(
                _dialog.ScaleToAsync(0.8, FluentAnimations.Gentle, FluentAnimations.Exit),
                _dialog.FadeToAsync(0, FluentAnimations.Gentle),
                _overlay.FadeToAsync(0, FluentAnimations.Gentle)
            );

            _overlay.IsVisible = false;
        }
    }
}

/// <summary>
/// Fluent Design pull-to-refresh container.
/// </summary>
public class FlPullToRefresh : ContentView
{
    public static readonly BindableProperty IsRefreshingProperty =
        BindableProperty.Create(nameof(IsRefreshing), typeof(bool), typeof(FlPullToRefresh), false);

    public bool IsRefreshing { get => (bool)GetValue(IsRefreshingProperty); set => SetValue(IsRefreshingProperty, value); }

    public event EventHandler? RefreshRequested;

    private readonly ScrollView _scrollView;
    private readonly VerticalStackLayout _content;
    private readonly FlCircularProgress _indicator;

    public FlPullToRefresh()
    {
        _indicator = new FlCircularProgress
        {
            IsIndeterminate = true,
            Size = 32,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _content = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { _indicator }
        };

        _scrollView = new ScrollView { Content = _content };
        Content = _scrollView;
    }

    public void SetContent(View content)
    {
        _content.Children.Clear();
        _content.Children.Add(_indicator);
        _content.Children.Add(content);
    }

    public void EndRefresh()
    {
        IsRefreshing = false;
        _indicator.IsVisible = false;
    }
}
