using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Model;
using System.Threading.Tasks;

namespace TruthOrDrink.ViewModels
{
	public class ProfileViewModel : ObservableObject
	{
		private readonly SQLiteService _sqliteService = new SQLiteService();
		private readonly Host _originalHost;

		private bool _isNameReadOnly;
		private bool _isPasswordReadOnly;
		private string _confirmPassword;
		private string _password;

		public ProfileViewModel(Host host)
		{
			_originalHost = new Host(host.HostId, host.Name, host.Email, host.Password);
			Host = new Host(host.HostId, host.Name, host.Email, host.Password);

			Password = string.Empty;
			ConfirmPassword = string.Empty;

			IsNameReadOnly = true;
			IsPasswordReadOnly = true;

			EditNameCommand = new RelayCommand(EditName);
			EditPasswordCommand = new RelayCommand(EditPassword);
			SaveCommand = new AsyncRelayCommand(Save);
		}

		public Host Host { get; }

		public string Password
		{
			get => _password;
			set => SetProperty(ref _password, value);
		}

		public string ConfirmPassword
		{
			get => _confirmPassword;
			set => SetProperty(ref _confirmPassword, value);
		}

		public bool IsNameReadOnly
		{
			get => _isNameReadOnly;
			set => SetProperty(ref _isNameReadOnly, value);
		}

		public bool IsPasswordReadOnly
		{
			get => _isPasswordReadOnly;
			set => SetProperty(ref _isPasswordReadOnly, value);
		}

		public string NameButtonText => IsNameReadOnly ? "Bewerk" : "Klaar";
		public string PasswordButtonText => IsPasswordReadOnly ? "Bewerk" : "Klaar";

		public IRelayCommand EditNameCommand { get; }
		public IRelayCommand EditPasswordCommand { get; }
		public IAsyncRelayCommand SaveCommand { get; }

		private void EditName()
		{
			IsNameReadOnly = !IsNameReadOnly;
			OnPropertyChanged(nameof(NameButtonText));
		}

		private void EditPassword()
		{
			IsPasswordReadOnly = !IsPasswordReadOnly;
			OnPropertyChanged(nameof(PasswordButtonText));

			if (!IsPasswordReadOnly)
			{
				Password = string.Empty;
				ConfirmPassword = string.Empty;
			}
		}

		private async Task Save()
		{
			bool isNameChanged = Host.Name != _originalHost.Name;
			bool isPasswordChanged = !string.IsNullOrWhiteSpace(Password);
			bool isEmailChanged = Host.Email != _originalHost.Email;

			if (!isNameChanged && !isPasswordChanged && !isEmailChanged)
			{
				await App.Current.MainPage.DisplayAlert("Geen Wijzigingen", "Er zijn geen wijzigingen gedetecteerd.", "OK");
				return;
			}

			if (isNameChanged && string.IsNullOrEmpty(Host.Name))
			{
				await App.Current.MainPage.DisplayAlert("Ongeldige Invoer", "Naam mag niet leeg zijn.", "OK");
				return;
			}

			string finalPassword;

			if (isPasswordChanged)
			{
				if (Password != ConfirmPassword)
				{
					await App.Current.MainPage.DisplayAlert("Wachtwoord Mismatch", "De wachtwoorden komen niet overeen.", "OK");
					return;
				}

				if (Password.Length < 8)
				{
					await App.Current.MainPage.DisplayAlert("Ongeldige Invoer", "Wachtwoord moet minstens 8 tekens lang zijn.", "OK");
					return;
				}

				bool hasSpecialChar = Password.Any(ch => "@#$%&!".Contains(ch));
				if (!hasSpecialChar)
				{
					await App.Current.MainPage.DisplayAlert("Ongeldige Invoer", "Wachtwoord moet minstens één speciaal teken bevatten (@#$%&!).", "OK");
					return;
				}

				var passwordHasher = new PasswordHasher(Password);
				finalPassword = passwordHasher.HashPassword();
			}
			else
			{
				var passwordHasher = new PasswordHasher(_originalHost.Password);
				finalPassword = passwordHasher.HashPassword();
			}

			string finalEmail = Host.Email.Trim().ToLower();

			var updatedHost = new Host(Host.HostId, Host.Name, finalEmail, finalPassword);

			await _sqliteService.UpdateHostAsync(updatedHost);
			await updatedHost.UpdateHostCredentials();

			await App.Current.MainPage.DisplayAlert("Opgeslagen", "Je profiel is bijgewerkt.", "OK");

			IsNameReadOnly = true;
			IsPasswordReadOnly = true;

			OnPropertyChanged(nameof(NameButtonText));
			OnPropertyChanged(nameof(PasswordButtonText));

			Password = string.Empty;
			ConfirmPassword = string.Empty;
		}



	}
}
