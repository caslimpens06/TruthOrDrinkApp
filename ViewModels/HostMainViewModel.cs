using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
using System.Windows.Input;
using TruthOrDrink.View;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class HostMainViewModel : ObservableObject
	{
		private readonly Host _host;
		private string _location;
		private Settings _settings;
		private readonly SQLiteService _sqliteService = new SQLiteService();

		public HostMainViewModel(Host host)
		{
			_host = host;
			NavigateToChooseGamePageCommand = new AsyncRelayCommand(NavigateToChooseGamePage);
			NavigateToProfilePageCommand = new AsyncRelayCommand(NavigateToProfilePage);
			LogoutCommand = new AsyncRelayCommand(Logout);
			NavigateToSettingsCommand = new RelayCommand(OnNavigateToSettings);

			LoadLocation();
		}

		public string HostName => _host.Name;

		public IAsyncRelayCommand NavigateToChooseGamePageCommand { get; }
		public IAsyncRelayCommand NavigateToProfilePageCommand { get; }
		public IAsyncRelayCommand LogoutCommand { get; }
		public ICommand NavigateToSettingsCommand { get; }

		public string Location
		{
			get { return _location; }
			set { SetProperty(ref _location, value); }
		}

		private async Task NavigateToChooseGamePage()
		{
			await App.Current.MainPage.Navigation.PushAsync(new HostChooseGamePage(_host));
		}

		private async Task NavigateToProfilePage()
		{
			await App.Current.MainPage.Navigation.PushAsync(new ProfilePage(_host));
		}

		private async void OnNavigateToSettings()
		{
			await App.Current.MainPage.Navigation.PushAsync(new SettingsPage(_host));
		}

		private async void LoadLocation()
		{
			_settings = await _sqliteService.GetSettingsAsync();
			Console.WriteLine(_settings.Country + _settings.Area);
			Location = _settings.Country + " - " + _settings.Area;
		}

		private async Task Logout()
		{
			await _sqliteService.ClearHostTableAsync();
			await _host.DeleteAllSessions();
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}