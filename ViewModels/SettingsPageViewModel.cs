using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.ViewModels
{
	public partial class SettingsPageViewModel : ObservableObject
	{
		private int _maxPlayerCount;
		private bool _isPlayerNumberReadOnly = true;
		private Settings _settings;
		private Color _buttonColor = Colors.Gray;
		private readonly SQLiteService _sqliteService = new SQLiteService();
		

		public SettingsPageViewModel()
		{
			TogglePlayerEditCommand = new RelayCommand(TogglePlayerEdit);
			OpenInstagramCommand = new AsyncRelayCommand(OpenInstagram);
			OpenFacebookCommand = new AsyncRelayCommand(OpenFacebook);
			OpenLinkedInCommand = new AsyncRelayCommand(OpenLinkedIn);

			SetProperty(ref _maxPlayerCount, 1);

			LoadSettings();
		}



		public int MaxPlayerCount
		{
			get => _maxPlayerCount;
			set
			{
				if (value < 1 || value > 10)
				{
					SetProperty(ref _maxPlayerCount, 5);
					ShowError("Ongeldige Invoer", "Je kan maar 1 tot en met 10 deelnemers koppelen aan je sessie.");
					return;
				}

				// If the value is valid, update it
				SetProperty(ref _maxPlayerCount, value);
			}
		}

		public bool IsPlayerNumberReadOnly
		{
			get => _isPlayerNumberReadOnly;
			set
			{
				SetProperty(ref _isPlayerNumberReadOnly, value);
				OnPropertyChanged(nameof(PlayerNumberButtonText));

				ButtonColor = value ? Colors.Gray : Colors.Green; // Gray when read-only and green when editable
			}
		}

		public Color ButtonColor
		{
			get => _buttonColor;
			set
			{
				SetProperty(ref _buttonColor, value);
				OnPropertyChanged(nameof(ButtonColor));
			}
		}

		public string PlayerNumberButtonText => IsPlayerNumberReadOnly ? "Bewerk" : "Opslaan";

		public RelayCommand ToggleEmailEditCommand { get; }
		public RelayCommand TogglePlayerEditCommand { get; }

		public IAsyncRelayCommand OpenInstagramCommand { get; }
		public IAsyncRelayCommand OpenFacebookCommand { get; }
		public IAsyncRelayCommand OpenLinkedInCommand { get; }

		private async Task LoadSettings()
		{
			_settings = await _sqliteService.GetSettingsAsync();
			if (_settings != null)
			{
				MaxPlayerCount = _settings.MaxPlayerCount;
			}
		}
		private void TogglePlayerEdit()
		{
			if (!IsPlayerNumberReadOnly)
			{
				SavePlayerNumber();
			}
			IsPlayerNumberReadOnly = !IsPlayerNumberReadOnly;
		}

		private async Task OpenInstagram()
		{
			await OpenUrlAsync("https://www.instagram.com");
		}

		private async Task OpenFacebook()
		{
			await OpenUrlAsync("https://www.facebook.com");
		}

		private async Task OpenLinkedIn()
		{
			await OpenUrlAsync("https://www.linkedin.com");
		}

		private async void ShowError(string title, string message)
		{
			await App.Current.MainPage.DisplayAlert(title, message, "OK");
		}

		private async void SavePlayerNumber()
		{
			if (_maxPlayerCount < 1 || _maxPlayerCount > 11)
			{
				ShowError("Ongeldige Invoer", "Je kan maar 1 tot en met 10 deelnemers koppelen aan je sessie.");
				return;
			}
			else
			{
				Settings _settings = new Settings(_maxPlayerCount);
				bool succes = await _settings.SaveSettingsAsync();
				if (!succes)
				{
					ShowError("Systeemfout", "Voorkeuren konden niet opgeslagen worden.");
				}
				else
				{	
					ShowError("Gelukt", "Instellingen zijn opgeslagen.");
					App.Vibrate();
				}
			}
		}

		private async Task OpenUrlAsync(string url)
		{
			try
			{
				await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
			}
			catch
			{
				ShowError("Systeemfout", "Link openen mislukt.");
			}
		}
	}
}
