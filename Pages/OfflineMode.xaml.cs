
namespace TruthOrDrink.Pages;

public partial class OfflineMode : FlyoutPage
{
	public OfflineMode()
	{
		InitializeComponent();
	}

	protected override bool OnBackButtonPressed()
	{
		IsPresented = !IsPresented;
		return true;
	}

	private async void GoToOfflineModeClicked(object sender, EventArgs e)
	{
		Detail = new NavigationPage(new OfflineGamePage());
		IsPresented = false;
	}

	private async void CloseAppClicked(object sender, EventArgs e)
	{
		App.CloseApp();
	}
}