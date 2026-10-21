using PlatiniumUI.Fluent.Controls.DataDisplay;

namespace PlatiniumUI.Sample.Pages;

public partial class DataDisplayPage : ContentPage
{
    public DataDisplayPage()
    {
        InitializeComponent();

        avatarGroup.AddAvatar(new FlAvatar { Text = "Alice Brown", Size = 36 });
        avatarGroup.AddAvatar(new FlAvatar { Text = "Charlie Davis", Size = 36 });
        avatarGroup.AddAvatar(new FlAvatar { Text = "Eve Martin", Size = 36 });
        avatarGroup.AddAvatar(new FlAvatar { Text = "Frank Lee", Size = 36 });
    }
}
