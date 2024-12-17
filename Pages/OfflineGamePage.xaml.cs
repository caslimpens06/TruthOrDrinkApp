using TruthOrDrink.Model;

namespace TruthOrDrink.Pages;

public partial class OfflineGamePage : ContentPage
{
	public OfflineGamePage()
	{
		InitializeComponent();
	}
	protected override bool OnBackButtonPressed()
	{
		return true;
	}

	private async void NavigateTo1(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new GameJoinPage(new Session(1)));
	}

	private async void NavigateTo2(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new GameJoinPage(new Session(2)));
	}

	private async void NavigateTo3(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new GameJoinPage(new Session(3)));
	}

	private async void NavigateTo4(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new GameJoinPage(new Session(4)));
	}

	private async void LeaveGameClicked(object sender, EventArgs e)
	{
		await Navigation.PopToRootAsync();
	}
}