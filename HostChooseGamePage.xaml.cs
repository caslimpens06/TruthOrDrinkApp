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
		await Navigation.PushModalAsync(new GameJoinPage(_host));
	}

	private async void NavigateTo2(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameJoinPage(_host));
	}

	private async void NavigateTo3(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameJoinPage(_host));
	}

	private async void NavigateTo4(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameJoinPage(_host));
	}

	private async void NavigateTo5(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameJoinPage(_host));
	}	

	private async void LeaveGame(object sender, EventArgs e)
	{
		Application.Current.MainPage = new WelcomePage();

	}
}