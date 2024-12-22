using System.ComponentModel;
using System.Windows.Input;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public class SignUpViewModel : INotifyPropertyChanged
	{
		private readonly SQLiteService _sqliteService = new SQLiteService();
		private string _name;
		private string _email;
		private string _password;
		private string _confirmPassword;
		private bool _isOverlayVisible;
		private bool _isBackButtonDisabled;

		public SignUpViewModel()
		{
			CreateAccountCommand = new Command(async () => await CreateAccount());
		}

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Email
		{
			get => _email;
			set
			{
				_email = value;
				OnPropertyChanged(nameof(Email));
			}
		}

		public string Password
		{
			get => _password;
			set
			{
				_password = value;
				OnPropertyChanged(nameof(Password));
			}
		}

		public string ConfirmPassword
		{
			get => _confirmPassword;
			set
			{
				_confirmPassword = value;
				OnPropertyChanged(nameof(ConfirmPassword));
			}
		}

		public bool IsOverlayVisible
		{
			get => _isOverlayVisible;
			set
			{
				_isOverlayVisible = value;
				OnPropertyChanged(nameof(IsOverlayVisible));
			}
		}

		public bool IsBackButtonDisabled
		{
			get => _isBackButtonDisabled;
			set
			{
				_isBackButtonDisabled = value;
				OnPropertyChanged(nameof(IsBackButtonDisabled));
			}
		}

		public ICommand CreateAccountCommand { get; }

		public event EventHandler<Host> NavigationRequested;

		private async Task CreateAccount()
		{
			if (string.IsNullOrEmpty(Name))
			{
				await App.Current.MainPage.DisplayAlert("Onjuiste invoer", "Geef een geldige naam.", "OK");
				return;
			}

			if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
			{
				await App.Current.MainPage.DisplayAlert("Onjuiste invoer", "Geef een geldig emailadres.", "OK");
				return;
			}

			Email = Email.Trim().ToLower();

			if (string.IsNullOrWhiteSpace(Password))
			{
				await App.Current.MainPage.DisplayAlert("Onjuiste invoer", "Wachtwoord mag niet leeg zijn.", "OK");
				return;
			}

			if (Password.Length < 8)
			{
				await App.Current.MainPage.DisplayAlert("Onjuiste invoer", "Wachtwoord moet minstens 8 tekens lang zijn.", "OK");
				return;
			}

			if (!Password.Any(ch => "@#$%&!".Contains(ch)))
			{
				await App.Current.MainPage.DisplayAlert("Onjuiste invoer", "Wachtwoord moet minstens 1 speciaal teken bevatten.", "OK");
				return;
			}

			if (Password != ConfirmPassword)
			{
				await App.Current.MainPage.DisplayAlert("Wachtwoord Mismatch", "Wachtwoorden komen niet overeen.", "OK");
				return;
			}

			IsOverlayVisible = true;
			IsBackButtonDisabled = true; // Disable back button during signup

			Host host = new Host(Email);
			bool exists = await host.CheckIfHostExistsAsync();

			if (exists)
			{
				await App.Current.MainPage.DisplayAlert("Account maken mislukt", "Dit emailadres bestaat al. Log in met je account.", "OK");
				IsOverlayVisible = false;
				IsBackButtonDisabled = false; // Re-enable back button
			}
			else
			{
				PasswordHasher passwordHasher = new PasswordHasher(Password);
				string hashedPassword = passwordHasher.HashPassword();

				host = new Host(Name, Email, hashedPassword);
				await host.AddHostAsync();
				int hostId = await host.GetHostPrimaryKey();

				Host savedHost = new Host(hostId, Name, Email, hashedPassword);
				await savedHost.SaveHostLocallyAsync();

				await App.Current.MainPage.DisplayAlert("Account", "Je account is gemaakt! Je wordt meteen ingelogd.", "OK");

				IsOverlayVisible = false;
				IsBackButtonDisabled = false; // Re-enable back button

				NavigationRequested?.Invoke(this, savedHost);
				await App.Current.MainPage.Navigation.PushAsync(new HostMainPage(savedHost));
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
