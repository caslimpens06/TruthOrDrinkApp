using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
using Microsoft.Maui.Controls;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class HostMainViewModel : ObservableObject
	{
		private readonly Host _host;
		private readonly SQLiteService _sqliteService = new SQLiteService();

		public HostMainViewModel(Host host)
		{
			_host = host;
			NavigateToChooseGamePageCommand = new AsyncRelayCommand(NavigateToChooseGamePage);
			NavigateToProfilePageCommand = new AsyncRelayCommand(NavigateToProfilePage);
			LogoutCommand = new AsyncRelayCommand(Logout);
		}

		public string HostName => _host.Name;

		public IAsyncRelayCommand NavigateToChooseGamePageCommand { get; }
		public IAsyncRelayCommand NavigateToProfilePageCommand { get; }
		public IAsyncRelayCommand LogoutCommand { get; }

		private async Task NavigateToChooseGamePage()
		{
			// Navigate to HostChooseGamePage
			var page = new HostChooseGamePage(_host);
			await App.Current.MainPage.Navigation.PushAsync(page);
		}

		private async Task NavigateToProfilePage()
		{
			// Navigate to ProfilePage
			var page = new ProfilePage(_host);
			await App.Current.MainPage.Navigation.PushAsync(page);
		}

		private async Task Logout()
		{
			await _sqliteService.ClearHostTableAsync();
			await _host.DeleteAllSessions();
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}