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
		string email = EmailEntry.Text;
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

		Host _host = new Host(name, email, password);
		bool exists = await _host.CheckIfHostExistsAsync();

		if (exists)
		{
			await DisplayAlert("Account maken mislukt", "Dit emailadres bestaat al. Log in met je account.", "OK");
		}
		else 
		{
			await _host.AddHostAsync();
			int hostid = await _host.GetHostPrimaryKey();

			Host savedHost = new Host(hostid, name, email, password);

			await sqlliteservice.SaveHostAsync(savedHost);
			
			await SecureStorage.SetAsync("host_id", hostid.ToString());

			await DisplayAlert("Account", "Je account is gemaakt! Je wordt meteen ingelogd.", "OK");

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


