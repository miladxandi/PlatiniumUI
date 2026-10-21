using System.Collections.ObjectModel;
using PlatiniumUI.Fluent.Controls.Navigation;
using PlatiniumUI.Fluent.DesignSystem.Icons;

namespace PlatiniumUI.Sample.Pages;

public partial class TabBarPage : ContentPage
{
    public TabBarPage()
    {
        InitializeComponent();
        SetupTopTabBar();
        SetupBottomTabBar();
        SetupBadgeTabBar();
    }

    private void SetupTopTabBar()
    {
        var tabBar = new FlTabBar
        {
            TabBarPosition = FlTabBarPosition.Top,
            IndicatorStyle = FlTabBarIndicatorStyle.Pill,
            Items = new ObservableCollection<FlTabBarItem>
            {
                new FlTabBarItem
                {
                    Title = "Home",
                    Icon = FluentIcons.home_24_regular,
                    Content = new Label
                    {
                        Text = "Home tab content - Top positioned tab bar with pill indicator",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                },
                new FlTabBarItem
                {
                    Title = "Search",
                    Icon = FluentIcons.search_24_regular,
                    Content = new Label
                    {
                        Text = "Search tab content",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                },
                new FlTabBarItem
                {
                    Title = "Favorites",
                    Icon = FluentIcons.star_24_regular,
                    Content = new Label
                    {
                        Text = "Favorites tab content",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                },
                new FlTabBarItem
                {
                    Title = "Settings",
                    Icon = FluentIcons.settings_24_regular,
                    Content = new Label
                    {
                        Text = "Settings tab content",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                }
            }
        };

        tabBar.SelectionChanged += (s, e) =>
        {
            selectionInfoLabel.Text = $"Top TabBar: Selected index {e}";
        };

        topTabBarContainer.Content = tabBar;
    }

    private void SetupBottomTabBar()
    {
        var tabBar = new FlTabBar
        {
            TabBarPosition = FlTabBarPosition.Bottom,
            IndicatorStyle = FlTabBarIndicatorStyle.Underline,
            Items = new ObservableCollection<FlTabBarItem>
            {
                new FlTabBarItem
                {
                    Title = "Feed",
                    Icon = FluentIcons.feed_20_regular,
                    Content = new Label
                    {
                        Text = "Feed content - Bottom positioned tab bar with underline indicator",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                },
                new FlTabBarItem
                {
                    Title = "Messages",
                    Icon = FluentIcons.mail_24_regular,
                    Content = new Label
                    {
                        Text = "Messages content",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                },
                new FlTabBarItem
                {
                    Title = "Notifications",
                    Icon = FluentIcons.alert_24_regular,
                    Content = new Label
                    {
                        Text = "Notifications content",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                }
            }
        };

        tabBar.SelectionChanged += (s, e) =>
        {
            selectionInfoLabel.Text = $"Bottom TabBar: Selected index {e}";
        };

        bottomTabBarContainer.Content = tabBar;
    }

    private void SetupBadgeTabBar()
    {
        var tabBar = new FlTabBar
        {
            TabBarPosition = FlTabBarPosition.Top,
            IndicatorStyle = FlTabBarIndicatorStyle.Background,
            Items = new ObservableCollection<FlTabBarItem>
            {
                new FlTabBarItem
                {
                    Title = "Inbox",
                    Icon = FluentIcons.mail_inbox_24_regular,
                    BadgeText = "5",
                    BadgeColor = Color.FromArgb("#E74C3C"),
                    Content = new Label
                    {
                        Text = "Inbox with 5 unread messages",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                },
                new FlTabBarItem
                {
                    Title = "Social",
                    Icon = FluentIcons.people_24_regular,
                    BadgeText = "12",
                    BadgeColor = Color.FromArgb("#3498DB"),
                    Content = new Label
                    {
                        Text = "Social notifications",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                },
                new FlTabBarItem
                {
                    Title = "Promos",
                    Icon = FluentIcons.tag_24_regular,
                    BadgeText = "New",
                    BadgeColor = Color.FromArgb("#27AE60"),
                    Content = new Label
                    {
                        Text = "Promotional content",
                        Padding = new Thickness(16),
                        TextColor = Application.Current?.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
                    }
                }
            }
        };

        tabBar.SelectionChanged += (s, e) =>
        {
            selectionInfoLabel.Text = $"Badge TabBar: Selected index {e}";
        };

        badgeTabBarContainer.Content = tabBar;
    }
}
