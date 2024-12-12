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
			PasswordEntry.Text = _host.Password;
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

	private void OnEditPasswordButtonClicked(object sender, EventArgs e)
	{
		PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
		EditPasswordButton.Text = PasswordEntry.IsPassword ? "Toon" : "Verberg";
		PasswordEntry.IsReadOnly = false;
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		string name = NameEntry.Text;
		string password = PasswordEntry.Text;

		if (_host.Name != name || _host.Password != password) {
			Host _newhost = new Host(_host.HostId, name, _host.Email, password);

			await sqlliteservice.UpdateHostAsync(_newhost);
			await _newhost.UpdateHostCredentials();
		}
		await DisplayAlert("Opgeslagen", "Je accountgegevens zijn opgeslagen.", "Ok");
	}
}
