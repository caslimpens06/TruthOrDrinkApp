using Microsoft.Maui.ApplicationModel.Communication;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink;

public partial class WelcomePage : ContentPage
{
	private Host _host;
	private readonly SQLiteService sqliteservice = new SQLiteService();
	public WelcomePage()
	{
		InitializeComponent();
		CheckLoginStatus();
	}

	private async void CheckLoginStatus()
	{
		_host = await sqliteservice.GetHostAsync();

		if (_host != null)
		{
			await Navigation.PushAsync(new HostMainPage(_host));
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