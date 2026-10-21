using PlatiniumUI.Sample.Pages;

namespace PlatiniumUI.Sample;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("ButtonsPage", typeof(ButtonsPage));
		Routing.RegisterRoute("InputsPage", typeof(InputsPage));
		Routing.RegisterRoute("SelectionPage", typeof(SelectionPage));
		Routing.RegisterRoute("NavigationPage", typeof(NavigationDemoPage));
		Routing.RegisterRoute("LayoutPage", typeof(LayoutPage));
		Routing.RegisterRoute("FeedbackPage", typeof(FeedbackPage));
		Routing.RegisterRoute("DataDisplayPage", typeof(DataDisplayPage));
		Routing.RegisterRoute("ChartsPage", typeof(ChartsPage));
		Routing.RegisterRoute("TabBarPage", typeof(TabBarPage));
	}
}
