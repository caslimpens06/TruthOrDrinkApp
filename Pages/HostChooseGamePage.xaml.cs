using TruthOrDrink.Model;

namespace TruthOrDrink;

public partial class HostChooseGamePage : ContentPage
{
	private Host _host;

	public HostChooseGamePage(Host host)
	{
		InitializeComponent();
		_host = host;
	}

	private async void NavigateTo1(object sender, EventArgs e)
	{
		Session _session = new Session(_host.HostId, 1);
		if (_session != null)
		{
			await Navigation.PushModalAsync(new GameJoinPage(_session));
		}
		else
		{
			await DisplayAlert("Error", "Session not initialized.", "OK");
		}
	}

	private async void NavigateTo2(object sender, EventArgs e)
	{
		Session _session = new Session(_host.HostId, 2);
		if (_session != null)
		{
			await Navigation.PushModalAsync(new GameJoinPage(_session));
		}
		else
		{
			await DisplayAlert("Error", "Session not initialized.", "OK");
		}
	}

	private async void NavigateTo3(object sender, EventArgs e)
	{
		Session _session = new Session(_host.HostId, 3);
		if (_session != null)
		{
			await Navigation.PushModalAsync(new GameJoinPage(_session));
		}
		else
		{
			await DisplayAlert("Error", "Session not initialized.", "OK");
		}
	}

	private async void NavigateTo4(object sender, EventArgs e)
	{
		Session _session = new Session(_host.HostId, 4);
		if (_session != null)
		{
			await Navigation.PushModalAsync(new GameJoinPage(_session));
		}
		else
		{
			await DisplayAlert("Error", "Session not initialized.", "OK");
		}
	}

	private async void NavigateTo5(object sender, EventArgs e)
	{
		Session _session = new Session(_host.HostId, 5);
		if (_session != null)
		{
			await Navigation.PushModalAsync(new GameJoinPage(_session));
		}
		else
		{
			await DisplayAlert("Error", "Session not initialized.", "OK");
		}
	}

	private async void LeaveGame(object sender, EventArgs e)
	{
		Application.Current.MainPage = new WelcomePage();
	}
}
