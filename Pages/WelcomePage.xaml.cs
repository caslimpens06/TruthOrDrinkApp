namespace TruthOrDrink;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
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