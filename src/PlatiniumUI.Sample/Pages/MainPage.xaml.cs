namespace PlatiniumUI.Sample.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnButtonsTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ButtonsPage");
    }

    private async void OnInputsTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("InputsPage");
    }

    private async void OnSelectionTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("SelectionPage");
    }

    private async void OnNavigationTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("NavigationPage");
    }

    private async void OnLayoutTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("LayoutPage");
    }

    private async void OnFeedbackTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("FeedbackPage");
    }

    private async void OnDataDisplayTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("DataDisplayPage");
    }

    private async void OnChartsTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("ChartsPage");
    }
}
