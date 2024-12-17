using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class OfflineModeViewModel : ObservableObject
	{
		public OfflineModeViewModel()
		{
			GoToOfflineGameCommand = new AsyncRelayCommand(GoToOfflineGame);
			CloseAppCommand = new AsyncRelayCommand(CloseApp);
		}

		public IAsyncRelayCommand GoToOfflineGameCommand { get; }
		public IAsyncRelayCommand CloseAppCommand { get; }

		private async Task GoToOfflineGame()
		{
			await App.Current.MainPage.Navigation.PushAsync(new OfflineGamePage());
		}

		private async Task CloseApp()
		{
			App.CloseApp();
		}
	}
}