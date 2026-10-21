using PlatiniumUI.Fluent.Controls.Buttons;

namespace PlatiniumUI.Sample.Pages;

public partial class ButtonsPage : ContentPage
{
    public ButtonsPage()
    {
        InitializeComponent();

        // Populate segmented control items
        segmentedControl.Items.Add("Day");
        segmentedControl.Items.Add("Week");
        segmentedControl.Items.Add("Month");
        segmentedControl.Items.Add("Year");
    }
}
