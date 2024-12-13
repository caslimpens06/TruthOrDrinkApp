using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Pages;

public partial class ProfilePage : ContentPage
{
	private Host _host;
	private readonly SQLiteService sqlliteservice = new SQLiteService();

	public ProfilePage(Host host)
	{
		InitializeComponent();
		LoadData(host);
	}

	private async Task LoadData(Host host)
	{
		_host = await sqlliteservice.GetHostAsync();

		if (_host != null)
		{
			NameEntry.Text = _host.Name;
			EmailEntry.Text = _host.Email;
		}
	}

	private void OnEditNameButtonClicked(object sender, EventArgs e)
	{
		if (NameEntry.IsReadOnly)
		{
			NameEntry.IsReadOnly = false;
			EditNameButton.Text = "Klaar";
		}
		else
		{
			NameEntry.IsReadOnly = true;
			EditNameButton.Text = "Bewerken";
		}
	}

	private void ShowPasswordClicked(object sender, EventArgs e)
	{
		if (ShowPasswordButton.Text == "Toon")
		{
			ShowPasswordButton.Text = "Verberg";
			PasswordEntry.IsPassword = false;
			ConfirmPasswordEntry.IsPassword = false;
		}
		else
		{
			ShowPasswordButton.Text = "Toon";
			PasswordEntry.IsPassword = true;
			ConfirmPasswordEntry.IsPassword = true;
		}
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		
		string name = NameEntry.Text;
		string password = PasswordEntry.Text;
		string confirmpassword = ConfirmPasswordEntry.Text;

		if (string.IsNullOrEmpty(name))
		{
			await DisplayAlert("Onjuiste invoer", "Geef een geldige naam.", "OK");
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

		foreach (char ch in password)
		{
			if ("@#$%&!".Contains(ch))
			{
				hasSpecialChar = true;
				break;
			}
		}

		if (!hasSpecialChar)
		{
			await DisplayAlert("Onjuiste invoer", "Wachtwoord moet minstens 1 speciaal teken bevatten.", "OK");
			return;
		}

		if (password != confirmpassword)
		{
			await DisplayAlert("Wachtwoord Mismatch", "Wachtwoorden komen niet overeen.", "OK");
			return;
		}

		OverlayGrid.IsVisible = true;
		NameEntry.IsEnabled = false;
		PasswordEntry.IsEnabled = false;
		ConfirmPasswordEntry.IsEnabled = false;
		EditNameButton.IsEnabled = false;

		if (_host.Name != name || _host.Password != password)
		{
			PasswordHasher passwordhasher = new PasswordHasher(password);
			string hashedpassword = passwordhasher.HashPassword();

			Host _newhost = new Host(_host.HostId, name, _host.Email, hashedpassword);

			await sqlliteservice.UpdateHostAsync(_newhost);
			await _newhost.UpdateHostCredentials();
			_host = _newhost;
		}
		else 
		{
			OverlayGrid.IsVisible = false;
			NameEntry.IsEnabled = true;
			PasswordEntry.IsEnabled = true;
			ConfirmPasswordEntry.IsEnabled = true;
			EditNameButton.IsEnabled = true;
			await DisplayAlert("Niet opgeslagen", "Je hebt je gegevens niet veranderd.", "Ok");
			return;
		}

		OverlayGrid.IsVisible = false;
		NameEntry.IsEnabled = true;
		PasswordEntry.IsEnabled = true;
		ConfirmPasswordEntry.IsEnabled = true;
		EditNameButton.IsEnabled = true;

		await DisplayAlert("Opgeslagen", "Je accountgegevens zijn opgeslagen.", "Ok");
	}
}
