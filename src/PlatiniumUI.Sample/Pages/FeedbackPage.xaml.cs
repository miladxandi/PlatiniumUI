using PlatiniumUI.Fluent.Controls.Feedback;

namespace PlatiniumUI.Sample.Pages;

public partial class FeedbackPage : ContentPage
{
    public FeedbackPage() => InitializeComponent();

    private async void OnShowToast(object? sender, EventArgs e)
    {
        var toast = new FlToast
        {
            Message = "This is a Fluent Design toast notification!",
            ToastType = FlToastType.Success
        };

        var overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#00000000"),
            Children = { toast }
        };

        await Navigation.PushModalAsync(new ContentPage { Content = overlay });
        await Task.Delay(3000);
        await Navigation.PopModalAsync();
    }
}
