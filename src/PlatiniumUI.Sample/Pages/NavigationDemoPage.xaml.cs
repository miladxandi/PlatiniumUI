using PlatiniumUI.Fluent.Controls.Navigation;

namespace PlatiniumUI.Sample.Pages;

public partial class NavigationDemoPage : ContentPage
{
    public NavigationDemoPage()
    {
        InitializeComponent();
        breadcrumb.Items = new List<string> { "Home", "Products", "Details" };
    }

    private async void OnOpenBottomSheet(object? sender, EventArgs e)
    {
        var sheet = new FlBottomSheet
        {
            SheetContent = new VerticalStackLayout
            {
                Spacing = 12,
                Children =
                {
                    new Label { Text = "Bottom Sheet Content", FontSize = 18, FontAttributes = FontAttributes.Bold },
                    new Label { Text = "This is a Fluent Design bottom sheet with slide-up animation." }
                }
            }
        };

        await Navigation.PushModalAsync(new ContentPage { Content = sheet });
        sheet.IsOpen = true;
    }
}
