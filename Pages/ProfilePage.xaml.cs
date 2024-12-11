using TruthOrDrink.Model;

namespace TruthOrDrink.Pages;

public partial class ProfilePage : ContentPage
{
	private Host _host;
	private Host _newhost;

	public ProfilePage(Host host)
	{
		InitializeComponent();
		_host = host;
		LoadData();
	}

	private async void LoadData()
	{
		_host = await _host.LoadHostData();

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

	private async Task OnSaveClicked(object sender, EventArgs e)
	{
		
		await DisplayAlert("Opgeslagen", "Je nieuwe accountgegevens zijn opgeslagen.", "Ok");
	}
}
