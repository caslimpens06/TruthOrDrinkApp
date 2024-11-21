namespace TruthOrDrink;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}

	private async void NavigateToSignup(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new SignUpPage());
	}

	private async void NavigateToHost(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new LoginPage());
	}

	private async void NavigateToParticipant(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new GuestIdentifierPage());
	}
}