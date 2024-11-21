namespace TruthOrDrink;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void Login(object sender, EventArgs e)
	{
		string email = EmailEntry.Text;
		string password = PasswordEntry.Text;

		if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
		{
			await DisplayAlert("Ongeldige invoer", "Voer een geldig e-mailadres in.", "OK");
			return;
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			await DisplayAlert("Ongeldig wachtwoord", "Voer je wachtwoord in.", "OK");
			return;
		}

		User user = new User(email, password);
		SupabaseService supabase = new SupabaseService();
		bool correctCredentials = await supabase.ValidateCredentialsAsync(user);

		if (correctCredentials)
		{
			await DisplayAlert("Inloggen", "Je bent succesvol ingelogd!", "OK");
			await Navigation.PushModalAsync(new GamePage());
		}
		else
		{
			await DisplayAlert("Inloggen mislukt", "E-mailadres of wachtwoord is onjuist.", "OK");
		}
	}

	private bool IsValidEmail(string email)
	{
		try
		{
			var addr = new System.Net.Mail.MailAddress(email);
			return addr.Address == email;
		}
		catch
		{
			return false;
		}
	}
}
