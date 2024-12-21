using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.View;
using TruthOrDrink;

public class LoginViewModel : ObservableObject
{
	private string _email;
	private string _password;
	private bool _isOverlayVisible;
	private bool _isBackButtonDisabled;

	public LoginViewModel()
	{
		LoginCommand = new AsyncRelayCommand(Login);
	}

	public string Email
	{
		get => _email;
		set => SetProperty(ref _email, value);
	}

	public string Password
	{
		get => _password;
		set => SetProperty(ref _password, value);
	}

	public bool IsOverlayVisible
	{
		get => _isOverlayVisible;
		set => SetProperty(ref _isOverlayVisible, value);
	}

	public bool IsBackButtonDisabled
	{
		get => _isBackButtonDisabled;
		set => SetProperty(ref _isBackButtonDisabled, value);
	}

	public IAsyncRelayCommand LoginCommand { get; }

	private async Task Login()
	{
		if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
		{
			await App.Current.MainPage.DisplayAlert("Ongeldige invoer", "Voer een geldig e-mailadres in.", "OK");
			return;
		}

		Email = Email.Trim().ToLower();

		if (string.IsNullOrWhiteSpace(Password))
		{
			await App.Current.MainPage.DisplayAlert("Ongeldig wachtwoord", "Voer je wachtwoord in.", "OK");
			return;
		}

		IsOverlayVisible = true;
		IsBackButtonDisabled = true; // Disable back button during login

		try
		{
			Host host = new Host(Email);
			string storedOnlineHash = await host.ValidateCredentialsAsync();

			PasswordHasher passwordHasher = new PasswordHasher(Password, storedOnlineHash);
			bool correctCredentials = passwordHasher.VerifyPassword();

			if (correctCredentials)
			{
				int hostId = await host.GetHostPrimaryKey();
				string name = await host.GetHostName();
				Host newHost = new Host(hostId, name, Email, Password);
				await newHost.SaveHostLocallyAsync();
				await App.Current.MainPage.Navigation.PushAsync(new HostMainPage(newHost));
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Login Mislukt", "Email of wachtwoord is incorrect.", "OK");
			}
		}
		finally
		{
			IsOverlayVisible = false;
			IsBackButtonDisabled = false; // Re-enable back button
		}
	}

	private static bool IsValidEmail(string email)
	{
		try
		{
			email = email.Trim();
			var addr = new System.Net.Mail.MailAddress(email);
			return addr.Address == email;
		}
		catch
		{
			return false;
		}
	}
}
