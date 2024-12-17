using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Model;

namespace TruthOrDrink.ViewModels
{
	public class ProfileViewModel : ObservableObject
	{
		private readonly SQLiteService _sqliteService = new SQLiteService();
		private Host _host;

		public ProfileViewModel(Host host)
		{
			_host = host;
			Name = _host.Name;
			Email = _host.Email;
			IsNameReadOnly = true;
			IsPasswordReadOnly = true;
			EditNameCommand = new AsyncRelayCommand(EditName);
			ShowPasswordCommand = new AsyncRelayCommand(ShowPassword);
			SaveCommand = new AsyncRelayCommand(Save);
		}

		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }

		public bool IsNameReadOnly { get; private set; }
		public bool IsPasswordReadOnly { get; private set; }

		public string NameButtonText => IsNameReadOnly ? "Edit" : "Done";

		public IAsyncRelayCommand EditNameCommand { get; }
		public IAsyncRelayCommand ShowPasswordCommand { get; }
		public IAsyncRelayCommand SaveCommand { get; }

		private Task EditName()
		{
			IsNameReadOnly = !IsNameReadOnly;
			OnPropertyChanged(nameof(IsNameReadOnly));
			OnPropertyChanged(nameof(NameButtonText));
			return Task.CompletedTask; // Return a completed task
		}

		private async Task ShowPassword()
		{
			IsPasswordReadOnly = !IsPasswordReadOnly;
			OnPropertyChanged(nameof(IsPasswordReadOnly));
		}

		private async Task Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				await App.Current.MainPage.DisplayAlert("Invalid Input", "Please enter a valid name.", "OK");
				return;
			}

			if (string.IsNullOrWhiteSpace(Password))
			{
				await App.Current.MainPage.DisplayAlert("Invalid Input", "Password cannot be empty.", "OK");
				return;
			}

			if (Password.Length < 8)
			{
				await App.Current.MainPage.DisplayAlert("Invalid Input", "Password must be at least 8 characters long.", "OK");
				return;
			}

			bool hasSpecialChar = false;
			foreach (char ch in Password)
			{
				if ("@#$%&!".Contains(ch))
				{
					hasSpecialChar = true;
					break;
				}
			}

			if (!hasSpecialChar)
			{
				await App.Current.MainPage.DisplayAlert("Invalid Input", "Password must contain at least one special character.", "OK");
				return;
			}

			if (Password != ConfirmPassword)
			{
				await App.Current.MainPage.DisplayAlert("Password Mismatch", "Passwords do not match.", "OK");
				return;
			}

			// Save the updated host information
			var passwordHasher = new PasswordHasher(Password);
			string hashedPassword = passwordHasher.HashPassword();

			Host updatedHost = new Host(_host.HostId, Name, _host.Email, hashedPassword);
			await _sqliteService.UpdateHostAsync(updatedHost);
			await updatedHost.UpdateHostCredentials();

			await App.Current.MainPage.DisplayAlert("Saved", "Your profile has been updated.", "OK");
			IsNameReadOnly = true;
			IsPasswordReadOnly = true;
			OnPropertyChanged(nameof(IsNameReadOnly));
			OnPropertyChanged(nameof(NameButtonText));
		}
	}
}