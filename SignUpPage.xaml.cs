namespace TruthOrDrink;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}
	private async void CreateAccount(object sender, EventArgs e)
	{
		string email = EmailEntry.Text;
		string password = PasswordEntry.Text;
		string confirmPassword = ConfirmPasswordEntry.Text;

		// Valideer email format
		if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
		{
			await DisplayAlert("Onjuiste invoer", "Geef een geldig emailadres.", "OK");
			return;
		}

		if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
		{
			await DisplayAlert("Onjuiste invoer", "Wachtwoord moet minstens 6 tekens lang zijn.", "OK");
			return;
		}

		if (password != confirmPassword)
		{
			await DisplayAlert("Wachtwoord Mismatch", "Wachtwoorden komen niet overeen.", "OK");
			return;
		}

		await DisplayAlert("Account", "Je account is gemaakt! Je wordt teruggestuurd naar het menu.", "OK");
		await Navigation.PopModalAsync();
	}

	// Hulpmethode voor emailvalidatie
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


