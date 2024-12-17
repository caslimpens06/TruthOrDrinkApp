using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;

namespace TruthOrDrink.ViewModels
{
	public class OfflineGameViewModel : ObservableObject
	{
		public OfflineGameViewModel()
		{
			JoinGame1Command = new AsyncRelayCommand(async () => await JoinGame(1));
			JoinGame2Command = new AsyncRelayCommand(async () => await JoinGame(2));
			JoinGame3Command = new AsyncRelayCommand(async () => await JoinGame(3));
			JoinGame4Command = new AsyncRelayCommand(async () => await JoinGame(4));
			LeaveGameCommand = new AsyncRelayCommand(LeaveGame);
		}

		public IAsyncRelayCommand JoinGame1Command { get; }
		public IAsyncRelayCommand JoinGame2Command { get; }
		public IAsyncRelayCommand JoinGame3Command { get; }
		public IAsyncRelayCommand JoinGame4Command { get; }
		public IAsyncRelayCommand LeaveGameCommand { get; }

		private async Task JoinGame(int gameId)
		{
			Session session = new Session(gameId);
			await App.Current.MainPage.Navigation.PushAsync(new GameJoinPage(session));
		}

		private async Task LeaveGame()
		{
			await App.Current.MainPage.DisplayAlert("Leave Game", "You have left the game.", "OK");
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}