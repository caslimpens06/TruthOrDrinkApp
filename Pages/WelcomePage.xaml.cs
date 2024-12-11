using Microsoft.Maui.ApplicationModel.Communication;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
		CheckLoginStatus();
	}

	private async void CheckLoginStatus()
	{
		var authToken = await SecureStorage.GetAsync("host_id");

		if (!string.IsNullOrEmpty(authToken))
		{
			Host host = new Host(Int32.Parse(authToken));

			await Navigation.PushAsync(new HostMainPage(host));
		}
	}

	private async void NavigateToSignup(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SignUpPage());
	}

	private async void NavigateToHost(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new LoginPage());
	}

	private async void NavigateToParticipant(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new GuestIdentifierPage());
	}
}