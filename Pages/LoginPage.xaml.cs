using TruthOrDrink.Model;
using TruthOrDrink.Pages;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink;

public partial class LoginPage : ContentPage
{
	private readonly SQLiteService sqlliteservice = new SQLiteService();
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void Login(object sender, EventArgs e)
	{
		string email = EmailEntry.Text.ToLower();
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

		if (IsValidEmail(email) && !string.IsNullOrWhiteSpace(password))
		{
			Host host = new Host(email);
			string storedOnlineHash = await host.ValidateCredentialsAsync();

			PasswordHasher passwordhasher = new PasswordHasher(password, storedOnlineHash);

			bool correctCredentials = passwordhasher.VerifyPassword();

			if (correctCredentials)
			{
				int hostid = await host.GetHostPrimaryKey();
				string name = await host.GetHostName();
				Host newHost = new Host(hostid, name, email, password);
				await sqlliteservice.SaveHostAsync(newHost);
				await Navigation.PushAsync(new HostMainPage(newHost));
			}
			
			else
			{
				await DisplayAlert("Inloggen mislukt", "E-mailadres of wachtwoord is onjuist.", "OK");
				return;
			}
		}
	}

	private static  bool IsValidEmail(string email)
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
