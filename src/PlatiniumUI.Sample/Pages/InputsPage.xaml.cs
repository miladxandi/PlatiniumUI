namespace PlatiniumUI.Sample.Pages;

public partial class InputsPage : ContentPage
{
    public InputsPage()
    {
        InitializeComponent();

        // Populate picker items
        countryPicker.ItemsSource = new List<string>
        {
            "United States", "United Kingdom", "Germany", "France",
            "Japan", "Australia", "Canada", "Iran"
        };
    }
}
