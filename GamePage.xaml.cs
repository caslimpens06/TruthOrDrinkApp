namespace TruthOrDrink;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();

	}


	private async void NavigateTo1(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameHostPage());
	}

	private async void NavigateTo2(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameHostPage());
	}

	private async void NavigateTo3(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameHostPage());
	}

	private async void NavigateTo4(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameHostPage());
	}

	private async void NavigateTo5(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GameHostPage());
	}

	private void LeaveGame(object sender, EventArgs e)
	{
		Application.Current.MainPage = new WelcomePage();

	}
}