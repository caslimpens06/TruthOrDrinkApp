using TruthOrDrink.Model;
using TruthOrDrink.Pages;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink;

public partial class SignUpPage : ContentPage
{
	private readonly SQLiteService sqlliteservice = new SQLiteService();

	public SignUpPage()
	{
		InitializeComponent();
	}
	private async void CreateAccount(object sender, EventArgs e)
	{
		string name = NameEntry.Text;
		string email = EmailEntry.Text.ToLower();
		string password = PasswordEntry.Text;
		string confirmPassword = ConfirmPasswordEntry.Text;

		if (string.IsNullOrEmpty(name))
		{
			await DisplayAlert("Onjuiste invoer", "Geef een geldige naam.", "OK");
			return;
		}

		if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
		{
			await DisplayAlert("Onjuiste invoer", "Geef een geldig emailadres.", "OK");
			return;
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			await DisplayAlert("Onjuiste invoer", "Wachtwoord mag niet leeg zijn.", "OK");
			return;
		}

		if (password.Length < 8)
		{
			await DisplayAlert("Onjuiste invoer", "Wachtwoord moet minstens 8 tekens lang zijn.", "OK");
			return;
		}

		bool hasSpecialChar = false;

		// Check for special characters
		foreach (char ch in password)
		{
			if ("@#$%&!".Contains(ch)) // Special characters check
			{
				hasSpecialChar = true;
				break;  // Stop once a special character is found
			}
		}

		if (!hasSpecialChar)
		{
			await DisplayAlert("Onjuiste invoer", "Wachtwoord moet minstens 1 speciaal teken bevatten.", "OK");
			return;
		}

		if (password != confirmPassword)
		{
			await DisplayAlert("Wachtwoord Mismatch", "Wachtwoorden komen niet overeen.", "OK");
			return;
		}
		OverlayGrid.IsVisible = true;
		NameEntry.IsEnabled = false;
		PasswordEntry.IsEnabled = false;
		ConfirmPasswordEntry.IsEnabled = false;
		EmailEntry.IsEnabled = false;

		Host _host = new Host(email);
		bool exists = await _host.CheckIfHostExistsAsync();

		if (exists)
		{
			await DisplayAlert("Account maken mislukt", "Dit emailadres bestaat al. Log in met je account.", "OK");
			OverlayGrid.IsVisible = false;
			NameEntry.IsEnabled = true;
			PasswordEntry.IsEnabled = true;
			ConfirmPasswordEntry.IsEnabled = true;
			EmailEntry.IsEnabled = true;
		}
		else 
		{
			PasswordHasher passwordhasher =	new PasswordHasher(password);
			string hashedpassword = passwordhasher.HashPassword();

			_host = new Host(name, email, hashedpassword);
			await _host.AddHostAsync();
			int hostid = await _host.GetHostPrimaryKey();

			Host savedHost = new Host(hostid, name, email, password);

			await sqlliteservice.SaveHostAsync(savedHost);
			
			await SecureStorage.SetAsync("host_id", hostid.ToString());

			await DisplayAlert("Account", "Je account is gemaakt! Je wordt meteen ingelogd.", "OK");

			OverlayGrid.IsVisible = false;

			await Navigation.PushAsync(new HostMainPage(savedHost));
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


