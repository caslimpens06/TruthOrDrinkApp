using TruthOrDrink.Model;

namespace TruthOrDrink.Pages;

public partial class HostMainPage : FlyoutPage
{
	private readonly Host _host;
	public HostMainPage(Host host)
	{
		InitializeComponent();
		_host = host;
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}

	private void OnHostChoosesGamePageClicked(object sender, EventArgs e)
	{
		Detail = new NavigationPage(new HostChooseGamePage(_host));
		IsPresented = false;
	}

	private void OnProfilePageClicked(object sender, EventArgs e)
	{
		Detail = new NavigationPage(new ProfilePage(_host));
		IsPresented = false;
	}

	private async void LogoutClicked(object sender, EventArgs e)
	{
		SecureStorage.Remove("host_id");
		await Navigation.PopToRootAsync();
	}
}